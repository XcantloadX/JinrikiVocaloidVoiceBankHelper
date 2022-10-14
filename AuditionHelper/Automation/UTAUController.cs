using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PInvoke;
using JinrikiVocaloidVBHelper.Util;
using System.Threading;
using JinrikiVocaloidVBHelper.FileOperation;
using System.IO;
using System.Windows.Forms;

namespace JinrikiVocaloidVBHelper.Automation
{
    /// <summary>
    /// UTAU 自动化
    /// </summary>
    public class UTAUController : IDisposable
    {
        private Process process;
        /// <summary>
        /// 主窗口句柄
        /// </summary>
        private IntPtr mainWindowHandle;
        /// <summary>
        /// 音源设定窗口句柄
        /// </summary>
        public IntPtr VoiceBankSettingsWindowHandle
        {
            get
            {
                return Win32.SearchWindowsOfProcess(PID, "ThunderRT6FormDC", "音源设定")[0];
            }

        }
        /// <summary>
        /// 当前打开的项目名称
        /// </summary>
        public string ProjectName
        {
            get
            {
                string title = User32.GetWindowText(mainWindowHandle);
                return title.Split('-')[0].TrimEnd();
            }
        }
        /// <summary>
        /// 进程 ID
        /// </summary>
        public int PID { get; private set; }
        private IntPtr menuHandle;
        public const string BTN_OK = "确定";

        /// <summary>
        /// 启动 UTAU
        /// </summary>
        /// <param name="UTAUPath">UTAU 程序主体路径</param>
        public UTAUController(string UTAUPath)
        {
            process = Process.Start(new ProcessStartInfo()
            {
                FileName = UTAUPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
            });

            process.WaitForInputIdle(); //等待界面初始化
            PID = process.Id;

            Init();
            //User32.ShowWindow(mainWindowHandle, User32.WindowShowStyle.SW_HIDE); //隐藏主窗口
        }

        /// <summary>
        /// 以指定 PID 创建对象
        /// </summary>
        /// <param name="pid"></param>
        public UTAUController(int pid)
        {
            PID = pid;
            Init();
        }

        private void Init()
        {
            //寻找主窗口
            IntPtr[] result = (from hwnd in Win32.GetAllWindowsOfProcess(PID)
                            where User32.GetClassName(hwnd) == "ThunderRT6FormDC" && User32.GetWindowText(hwnd).Contains("UTAU")
                            select hwnd).ToArray();
            if(result.Length > 1)
            {
                throw new NotImplementedException("多窗口");
            }
            mainWindowHandle = result[0];

            if (!User32.GetWindowText(mainWindowHandle).Contains("UTAU"))
            {
                throw new Exception("无法获取 UTAU 主窗口句柄，请尝试关闭正在运行的其他 VB6 程序。");
            }

            menuHandle = User32.GetMenu(mainWindowHandle);
        }

        /// <summary>
        /// 启动 UTAU 并打开指定项目文件
        /// </summary>
        /// <param name="UTAUPath">UTAU 程序主体路径</param>
        /// <param name="projectPath">项目文件路径</param>
        public UTAUController(string UTAUPath, string projectPath) : this(UTAUPath)
        {
            
        }

        public void Open(string projectPath)
        {
            ClickMenu(mainWindowHandle, 0, 1); //文件 -> 打开
        }

        /// <summary>
        /// 刷新音源设定
        /// </summary>
        public void RefreshVoiceBank()
        {
            ClickMenu(mainWindowHandle, 6, 0); //工具 -> 音源设定
            Thread.Sleep(100);
            ClickMenu(VoiceBankSettingsWindowHandle, 0, 3); //文件 -> 音源设定
            Thread.Sleep(100);
            ClickMessageBox(BTN_OK); //自动点击信息框
            Thread.Sleep(300);
            Win32.FindAndClick(VoiceBankSettingsWindowHandle, IntPtr.Zero, "ThunderRT6CommandButton", BTN_OK);
        }

        public void Dispose()
        {
            process.Kill();
            process.Dispose();
        }

        /// <summary>
        /// 点击菜单栏
        /// </summary>
        /// <param name="menuIndex">主菜单下标</param>
        /// <param name="subMenuIndex">下拉菜单下标</param>
        private void ClickMenu(IntPtr windowHandle, int menuIndex, int subMenuIndex)
        {
            IntPtr subHandle = User32.GetSubMenu(menuHandle, menuIndex);
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_COMMAND, new IntPtr(User32.GetMenuItemId(subHandle, subMenuIndex)), IntPtr.Zero);
        }

        /// <summary>
        /// 自动点击信息框的确定按钮
        /// </summary>
        /// <param name="btnName">按钮名称</param>
        private void ClickMessageBox(string btnName)
        {
            IntPtr[] allWindows = Win32.GetAllWindowsOfProcess(PID);
            IntPtr msgboxHwnd = IntPtr.Zero;

            //寻找信息框的指定按钮
            foreach (var window in allWindows)
            {
                if (User32.GetWindowText(window) != "歌声合成の软件 - UTAU") //信息框标题
                    continue;
                msgboxHwnd = User32.FindWindowEx(window, IntPtr.Zero, "Button", btnName);
                if (msgboxHwnd != IntPtr.Zero)
                    break;
            }

            if (msgboxHwnd == IntPtr.Zero)
                throw new Exception("无法找到指定的信息框按钮：" + btnName);
            Win32.Click(msgboxHwnd);
        }

        /// <summary>
        /// 自动寻找 UTAU 窗口并创建实例
        /// </summary>
        public static UTAUController GetInstance()
        {
            Process[] processes = Process.GetProcessesByName("utau");
            Process uatuProcess = null;
            if (processes.Length > 1)
                uatuProcess = DialogChooseUTAU.ShowBox(Process.GetProcessesByName("utau"));
            else if (processes.Length == 0)
            {
                throw new IgnorableException("提示", "未找到 UTAU 窗口，请先启动 UTAU。", System.Windows.Forms.MessageBoxIcon.Information);
            }
            else
                uatuProcess = processes[0];

            return new UTAUController(uatuProcess.Id);
        }

        /// <summary>
        /// 清理无用音源文件
        /// </summary>
        /// <param name="otoFilePath">oto.ini 文件路径</param>
        public static void CleanVoiceBank(string otoFilePath)
        {
            OtoFile oto = new OtoFile(File.ReadAllText(otoFilePath));

            //枚举音源文件夹里所有的文件
            DirectoryInfo dir = new FileInfo(otoFilePath).Directory;
            FileInfo[] files = dir.GetFiles();
            List<FileInfo> toDelete = new List<FileInfo>(10);
            foreach (var file in files)
            {
                if (file.Extension != ".wav" && file.Extension != ".frq" && file.Extension != ".llsm")
                    continue;

                switch (file.Extension)
                {
                    case ".wav":
                        if (!oto.Items.ContainsKey(file.Name.Replace(".wav", "")))
                            toDelete.Add(file);
                        break;
                    case ".frq":
                        if (!oto.Items.ContainsKey(file.Name.Replace("_wav.frq", "")))
                            toDelete.Add(file);
                        break;
                    case ".llsm":
                        if (!oto.Items.ContainsKey(file.Name.Replace(".wav.llsm", "")))
                            toDelete.Add(file);
                        break;
                    default:
                        break;
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("即将删除以下文件");
            foreach (var file in toDelete)
            {
                sb.AppendLine(file.Name);
            }
            sb.AppendLine("是否继续？");
            if(MessageBox.Show(sb.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {

                foreach (var file in toDelete)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format("无法删除文件 {0}。\n错误信息：\n{1}", file.Name, e.ToString()), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            MessageBox.Show("删除完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
