using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinrikiVocaloidVBHelper.Util
{
    public class TimeConvert
    {
        /// <summary>
        /// 将秒转换为 srt 字幕时间格式
        /// </summary>
        public static string Sec2SrtTime(double sec)
        {
            TimeSpan t = new TimeSpan(0, 0, 0, 0, (int)(sec * 1000));
            return t.ToString("hh':'mm':'ss','fff", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 将 srt 字幕时间格式转换为秒
        /// </summary>
        public static double SrtTime2Sec(string strTime)
        {
            return TimeSpan.ParseExact(strTime, "hh':'mm':'ss','fff", System.Globalization.CultureInfo.InvariantCulture).TotalSeconds;
        }
    }
}
