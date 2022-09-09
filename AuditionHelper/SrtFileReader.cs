using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NPinyin;

namespace AuditionHelper
{
    /// <summary>
    /// srt 字幕文件读取器
    /// </summary>
    public class SrtReader
    {
        
        /// <summary>
        /// 读取 srt 字符串里的字幕信息
        /// </summary>
        /// <param name="content">srt 文件内容</param>
        /// <returns>读取结果</returns>
        public static SrtLine[] ReadAll(string content)
        {
            
            string[] lines = content.Replace("\r", string.Empty).Split('\n');
            List<SrtLine> srtLines = new List<SrtLine>((int)(Math.Ceiling((double)lines.Length % 4)));

            /*
1
00:00:09,600 --> 00:00:13,633
我的老家哎就住在这个屯

2
00:00:14,400 --> 00:00:18,766
我是这个屯里土生土长的人啊
             */

            for (int i = 0; i < lines.Length; i += 4)
            {
                if (lines[i] == string.Empty)
                    continue;
                SrtLine line = new SrtLine();

                //第一行
                line.Number = int.Parse(lines[i]);
                //第二行
                string[] times = lines[i + 1].Split(new string[] { " --> " }, StringSplitOptions.None);
                line.StartTime = times[0];
                line.EndTime = times[1];
                //第三行
                line.Content = lines[i + 2];
                //第四行（空白）
                srtLines.Add(line);
            }
            return srtLines.ToArray();
        }

        public static SrtLine[] ReadAllFile(string path)
        {
            return ReadAll(File.ReadAllText(path));
        }
    }

    /// <summary>
    /// 单行字幕
    /// </summary>
    public class SrtLine : IComparable<SrtLine>
    {
        public string FilePath { get; internal set; }
        /// <summary>
        /// 字幕编号
        /// </summary>
        public int Number { get; internal set; }
        public string StartTime { get; internal set; }
        public string EndTime { get; internal set; }
        /// <summary>
        /// 字幕内容
        /// </summary>
        public string Content { get; internal set; }
        /// <summary>
        /// 字幕内容的拼音
        /// </summary>
        public string ContentPinYin
        {
            get
            {
                //return Pinyin.GetPinyin(Content);
                return TinyPinyin.PinyinHelper.GetPinyin(Content).ToLower();
            }
        }
        public double Speed
        {
            get
            {
                return Content.Replace(" ", string.Empty).Length / Duration.TotalSeconds;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                TimeSpan start = TimeSpan.ParseExact(StartTime, "hh':'mm':'ss','fff", System.Globalization.CultureInfo.InvariantCulture);
                TimeSpan end = TimeSpan.ParseExact(EndTime, "hh':'mm':'ss','fff", System.Globalization.CultureInfo.InvariantCulture);
                return end - start;
            }
        }

        public int CompareTo(SrtLine other)
        {
            return other.Speed.CompareTo(Speed);
        }
    }
}
