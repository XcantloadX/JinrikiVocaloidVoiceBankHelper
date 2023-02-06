using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JinrikiVocaloidVBHelper.Util;
using System.IO;
using System.Windows.Forms;

namespace JinrikiVocaloidVBHelper.Audition
{
    public abstract class AuditionController : IDisposable
    {
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public abstract void OpenFile(string path);
        public abstract void Select(string startTime, string endTime);
        public abstract void Select2(double startTime, double endTime);

        /// <summary>
        /// 转到指定时间
        /// </summary>
        /// <param name="time">00:00:04,866 这种格式的时间</param>
        public abstract void Seek(string time);

        /// <summary>
        /// 转到指定时间
        /// </summary>
        /// <param name="time">秒</param>
        public abstract void Seek2(double time);
        /// <summary>
        /// 保存当前选区为 UTAU 格式并自动重命名
        /// </summary>
        /// <param name="fileName">文件名称，比如 ha.wav</param>
        /// <param name="filePath">文件保存路径</param>
        public abstract void SaveSelection(string fileName, string filePath);


        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        /// <summary>
        /// 保证当前激活窗口为 Audition
        /// </summary>
        public static void EnsureActived()
        {
            Process[] processes = Process.GetProcesses();
            Process[] au = (from p in processes
                         where p.ProcessName.StartsWith("Adobe Audition")
                         select p).ToArray();

            if (au.Length == 0)
            {
                if(MessageBox.Show("Adobe Audition 未在运行。是否尝试自动启动 Adobe Audition？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    string exe = LocateAu();
                    if (exe != null)
                    {
                        Process.Start(exe);
                        System.Threading.Thread.Sleep(5000);
                        EnsureActived();
                        return;
                    }
                    else
                        MessageBox.Show("无法找到 Adobe Audition 安装路径。请手动启动。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            SetForegroundWindow(au[0].MainWindowHandle);
        }

        //定位 Audition 的位置
        private static string LocateAu()
        {
#if DEBUG
            return @"C:\Program Files\Adobe\Adobe Audition CC 2018\Adobe Audition CC.exe";
#endif

            string path = @"C:\Program Files\Adobe";
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] directories = dir.GetDirectories();
            foreach (var directory in directories)
            {
                if (directory.Name.StartsWith("Adobe Audition"))
                {
                    if(File.Exists(Path.Combine(directory.FullName, "Adobe Audition CC.exe"))) // 旧版
                        return Path.Combine(directory.FullName, "Adobe Audition CC.exe");
                    else if (File.Exists(Path.Combine(directory.FullName, "Adobe Audition.exe"))) // 2022
                        return Path.Combine(directory.FullName, "Adobe Audition.exe");
                }
                    
            }
            return null;
        }

        public virtual void Dispose()
        {
            
        }
    }
}
