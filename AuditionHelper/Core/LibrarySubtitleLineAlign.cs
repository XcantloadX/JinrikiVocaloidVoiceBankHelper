using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JinrikiVocaloidVBHelper.Core
{
    /// <summary>
    /// 单行字幕对应的标注数据
    /// </summary>
    [DataContract]
    public class LibrarySubtitleLineAlign
    {

        /// <summary>
        /// 本行标注数据所属的字幕编号
        /// </summary>
        [DataMember]
        public int Number { get; set; }
        /// <summary>
        /// 本行标注数据所含的内容（单词列表/发音列表）
        /// </summary>
        [DataMember]
        public List<Item> Items { get; set; }

        /// <summary>
        /// TextGird 文件中的一个项
        /// </summary>
        [DataContract]
        public class Item
        {
            /// <summary>
            /// 项类型
            /// </summary>
            [DataMember]
            public ItemType Type { get; set; }
            /// <summary>
            /// 项内容
            /// </summary>
            [DataMember]
            public string Text { get; set; }
            /// <summary>
            /// 项开始时间（秒）
            /// </summary>
            [DataMember]
            public double Start { get; set; }
            /// <summary>
            /// 项结束时间（秒）
            /// </summary>
            [DataMember]
            public double End { get; set; }
        }

        /// <summary>
        /// 从本地 TextGird 文件读入数据并创建一个 LibraryAudioAlign 实例
        /// </summary>
        /// <param name="textgirdPath">TextGird 文件路径</param>
        /// <returns>读入后的数据</returns>
        public static LibrarySubtitleLineAlign ReadFromString(string contents)
        {
            contents = contents.Replace("\r\n", "\n");
            string[] lines = contents.Split('\n');
            //判断文件格式
            if (lines[0] != "File type = \"ooTextFile\"" || lines[1] != "Object class = \"TextGrid\"")
                throw new InvalidTextGridFormatException();

            //获取数据开始位置
            int i = 0;
            while (i < lines.Length)
            {
                if (lines[i].Trim() == "item [1]:")
                    break;
                i++;
            }

            //读入数据
            i += 5;
            Group g = Regex.Match(lines[i], @"intervals: size = (\d+)").Groups[1]; //确定数组大小
            i++;
            int count = int.Parse(g.Value);
            /*
            每部分格式：
            intervals [1]:
                xmin = 0 
                xmax = 0.03 
                text = "" 
            */
            List<Item> items = new List<Item>(count);
            for (int j = 0; j < count; j++)
            {
                int baseIndex = i + j * 4;
                items.Add(new Item
                {
                    Type = ItemType.Word,
                    Text = Regex.Match(lines[baseIndex + 3], "text = \"(.+)\" ").Groups[1].Value,
                    Start = double.Parse(Regex.Match(lines[baseIndex + 1], @"xmin = (.+) ").Groups[1].Value),
                    End = double.Parse(Regex.Match(lines[baseIndex + 2], @"xmax = (.+) ").Groups[1].Value),
                });
            }

            //解析 .textgird 文件完毕
            LibrarySubtitleLineAlign align = new LibrarySubtitleLineAlign();
            align.Items = items;
            return align;
        }

        public enum ItemType
        {
            /// <summary>
            /// 单词
            /// </summary>
            Word,
            /// <summary>
            /// 发音
            /// </summary>
            Phone
        }

        public class InvalidTextGridFormatException : Exception { }
    }


}
