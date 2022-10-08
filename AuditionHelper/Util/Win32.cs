using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JinrikiVocaloidVBHelper.Util
{
    public class Win32
    {
        /// <summary>
        /// 取指定进程的所有窗口
        /// </summary>
        /// <param name="pid">进程 ID</param>
        /// <returns></returns>
        public static IntPtr[] GetAllWindowsOfProcess(int pid)
        {
            List<IntPtr> windows = new List<IntPtr>(10);
            User32.EnumWindows((IntPtr hwnd, IntPtr lParam) =>
            {
                int newPid = 0;
                User32.GetWindowThreadProcessId(hwnd, out newPid);
                if (newPid == pid)
                    windows.Add(hwnd);
                return true;
            }, IntPtr.Zero);

            return windows.ToArray();
        }

        /// <summary>
        /// 搜索指定进程的所有符合条件的窗口
        /// </summary>
        /// <param name="pid">进程 ID</param>
        /// <param name="title">标题</param>
        /// <param name="className">窗口类名</param>
        /// <returns>结果</returns>
        public static IntPtr[] SearchWindowsOfProcess(int pid, string title, string className)
        {
            /*return (from hwnd in Win32.GetAllWindowsOfProcess(pid)
                    where Regex.IsMatch(User32.GetClassName(hwnd), title) && Regex.IsMatch(User32.GetWindowText(hwnd), className)
                    select hwnd).ToArray();*/

            IntPtr[] allWindows = Win32.GetAllWindowsOfProcess(pid);
            List<IntPtr> result = new List<IntPtr>(10);
            foreach (var window in allWindows)
            {
                if (Regex.IsMatch(User32.GetClassName(window), title) && Regex.IsMatch(User32.GetWindowText(window), className))
                    result.Add(window);
            }
            return result.ToArray();
        }

        public static void Click(IntPtr hWnd)
        {
            User32.SendMessage(hWnd, User32.WindowMessage.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            System.Threading.Thread.Sleep(2);
            User32.SendMessage(hWnd, User32.WindowMessage.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// 寻找指定控件并点击
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <param name="childAfter"></param>
        /// <param name="className"></param>
        /// <param name="windowTitle"></param>
        public static void FindAndClick(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle)
        {
            IntPtr hwnd = User32.FindWindowEx(parentHandle, childAfter, className, windowTitle);
            Click(hwnd);
        }
        //public static IntPtr[] 
    }
}
