using AuditionHelper.Core;
using JinrikiVocaloidVBHelper.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace JinrikiVocaloidVBHelper.Core
{
    public class AuditionKeyboardController : AuditionController
    {
        private static InputSimulator simulator = new InputSimulator();
        public const VirtualKeyCode ALT = (VirtualKeyCode)0x12;
        /// <summary>
        /// 打开文件等待时间因子。
        /// 设置为 0 表示使用固定等待时间。
        /// 设置为非 0 表示 等待时间 = OpenFileWaitTimeFactor * 文件大小(字节数)
        /// </summary>
        public static double OpenFileWaitTimeFactor = 0;
        /// <summary>
        /// Audition 窗口是否是激活状态
        /// </summary>
        public static bool IsAuditionActive
        {
            get
            {
                //判断激活窗口的进程名称
                IntPtr handle = GetForegroundWindow();
                uint pid = 0;
                GetWindowThreadProcessId(handle, out pid);
                if (pid == 0)
                    return false;
                string name = "";
                try
                {
                    name = Process.GetProcessById((int)pid).ProcessName;
                }
                catch
                {
                    return false;
                }

                return name.Contains("Adobe Audition") || name == Process.GetCurrentProcess().ProcessName;
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);


        public override void SaveSelection(string fileName, string filePath)
        {
            EnsureActived();

            //Shift + Alt + C 复制到新建
            PressKey(VirtualKeyCode.VK_C, VirtualKeyCode.SHIFT, ALT);
            Sleep();

            //全选
            PressKey(VirtualKeyCode.VK_A, VirtualKeyCode.CONTROL);
            Sleep();

            //Alt + R 打开收藏夹菜单
            PressKey(VirtualKeyCode.VK_R, ALT);
            Sleep();

            PressKey(VirtualKeyCode.RETURN);
            Sleep();

            //Ctrl + S
            PressKey(VirtualKeyCode.VK_S, VirtualKeyCode.CONTROL);
            Sleep();

            //输入名称
            InputString(fileName);
            Sleep();

            //输入保存路径
            PressKey(VirtualKeyCode.TAB);
            InputString(filePath);
            Sleep();

            //Tab x 5 ，空格 取消保存元数据 
            PressKey(VirtualKeyCode.TAB);
            PressKey(VirtualKeyCode.TAB);
            PressKey(VirtualKeyCode.TAB);
            PressKey(VirtualKeyCode.TAB);
            PressKey(VirtualKeyCode.TAB);
            PressKey(VirtualKeyCode.SPACE);
            Sleep();

            //回车 保存
            PressKey(VirtualKeyCode.RETURN);
            Sleep();

            //关闭文件
            PressKey(VirtualKeyCode.VK_W, VirtualKeyCode.CONTROL);
            Sleep();
        }

        public override void OpenFile(string path)
        {
            EnsureActived();

            //Ctrl + W 关闭当前文件
            simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_W);
            Sleep();

            //（是否保存对话框）
            //TAB 移动到否按钮
            simulator.Keyboard.ModifiedKeyStroke(null, VirtualKeyCode.TAB);
            Sleep();

            //回车确认
            simulator.Keyboard.ModifiedKeyStroke(null, VirtualKeyCode.RETURN);
            Sleep();

            //Ctrl + O 打开文件对话框
            simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_O);
            Sleep();

            //输入地址
            InputString(path);
            Sleep();

            //回车
            simulator.Keyboard.ModifiedKeyStroke(null, VirtualKeyCode.RETURN);

            double waitTime = 0f;
            if (OpenFileWaitTimeFactor != 0)
                waitTime = new System.IO.FileInfo(path).Length / OpenFileWaitTimeFactor;
            Sleep((int)(waitTime * 1000));
        }

        public override void Select(string startTime, string endTime)
        {
            Seek(startTime);
            Sleep();
            //I 设置为入点
            simulator.Keyboard.ModifiedKeyStroke(null, VirtualKeyCode.VK_I);
            Seek(endTime);
            Sleep();
            //O 设置为出点
            simulator.Keyboard.ModifiedKeyStroke(null, VirtualKeyCode.VK_O);
            Sleep();
            //移动回开头
            Seek(startTime);
            Sleep();
            //缩放为选区
            simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_S);
        }

        public override void Seek(string time)
        {
            ResetFocus();

            //Shift + TAB 移动到时间显示处
            simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.TAB);
            Sleep();

            //粘贴时间
            simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
            Sleep();
            simulator.Keyboard.ModifiedKeyStroke(null, VirtualKeyCode.BACK);
            Sleep();

            simulator.Keyboard.TextEntry(time.Replace(",", "."));
            Sleep();

            //Enter 确认
            simulator.Keyboard.ModifiedKeyStroke(null, VirtualKeyCode.RETURN);
            Sleep();

            ResetFocus();
        }

        /// <summary>
        /// 重置编辑面板焦点
        /// </summary>
        public void ResetFocus()
        {
            //Alt + 1 关闭编辑面板
            simulator.Keyboard.ModifiedKeyStroke((VirtualKeyCode)0x12, VirtualKeyCode.VK_1);
            //Alt + 1 再打开编辑面板
            simulator.Keyboard.ModifiedKeyStroke((VirtualKeyCode)0x12, VirtualKeyCode.VK_1);
        }


        private static void PressKey(VirtualKeyCode key, params VirtualKeyCode[] modifiers)
        {
            simulator.Keyboard.ModifiedKeyStroke(modifiers, key);
            
        }

        private static void InputString(string text)
        {
            simulator.Keyboard.TextEntry(text);
        }

        private static void Sleep(int time = 100)
        {
            System.Threading.Thread.Sleep(time);
        }

        public override void Select2(double startTime, double endTime)
        {
            Select(TimeConvert.Sec2SrtTime(startTime), TimeConvert.Sec2SrtTime(endTime));
        }

        public override void Seek2(double time)
        {
            Seek(TimeConvert.Sec2SrtTime(time));
        }
    }
}
