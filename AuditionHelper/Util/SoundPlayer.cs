using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JinrikiVocaloidVBHelper.Util
{



    public class SoundPlayer
    {
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        /// <summary>
        /// 播放音频。阻塞式函数。
        /// </summary>
        /// <param name="filePath">音频文件路径</param>
        public static void PlaySound(string filePath)
        {
            mciSendString($"open \"{filePath}\" alias aaaaa", null, 0, IntPtr.Zero);
            mciSendString($"play aaaaa wait", null, 0, IntPtr.Zero);
            mciSendString($"close aaaaa", null, 0, IntPtr.Zero);
        }
    }
}
