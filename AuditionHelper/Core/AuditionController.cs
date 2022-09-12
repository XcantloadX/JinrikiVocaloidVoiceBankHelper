using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JinrikiVocaloidVBHelper.Util;

namespace JinrikiVocaloidVBHelper.Core
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
        /// <param name="fileName">不加后缀名</param>
        /// <param name="filePath">文件保存路径</param>
        /// </summary>
        public abstract void SaveSelection(string fileName, string filePath);


        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        /// <summary>
        /// 保证当前激活窗口为 Audition
        /// </summary>
        public static void EnsureActived()
        {
            //TODO 支持 audition cs.exe
            var processes = Process.GetProcessesByName("Adobe Audition CC");
            if (processes.Length == 0) throw new Exception("找不到 Adobe Audition CC.exe。请检查你使用的 AU 版本是否为 CC 版。");

            SetForegroundWindow(processes[0].MainWindowHandle);
        }

        public virtual void Dispose()
        {
            
        }
    }
}
