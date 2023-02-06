using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NPinyin;
using hyjiacan.py4n;

namespace JinrikiVocaloidVBHelper.Core
{
    /// <summary>
    /// srt 字幕文件读取器
    /// </summary>
    public class SubtitleReader
    {
        /// <summary>
        /// 读取 srt 字符串里的字幕信息
        /// </summary>
        /// <param name="srtString">srt 文件内容</param>
        /// <returns>读取结果</returns>
        public static SubtitleLine[] ReadAllFromString(string srtString)
        {
            
            string[] lines = srtString.Replace("\r", string.Empty).Split('\n');
            List<SubtitleLine> Subtitles = new List<SubtitleLine>((int)(Math.Ceiling((double)lines.Length % 4)));

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
                SubtitleLine line = new SubtitleLine();

                //第一行
                line.Number = int.Parse(lines[i]);
                //第二行
                string[] times = lines[i + 1].Split(new string[] { " --> " }, StringSplitOptions.None);
                line.StartTime = times[0];
                line.EndTime = times[1];
                //第三行
                line.Content = lines[i + 2];
                //第四行（空白）
                Subtitles.Add(line);
            }
            return Subtitles.ToArray();
        }

        /// <summary>
        /// 读取 srt 文件里的字幕信息
        /// </summary>
        /// <param name="srtPath">srt 文件路径</param>
        /// <returns>读取结果</returns>
        public static SubtitleLine[] ReadAllFromFile(string srtPath)
        {
            return ReadAllFromString(File.ReadAllText(srtPath));
        }
    }

    
}
