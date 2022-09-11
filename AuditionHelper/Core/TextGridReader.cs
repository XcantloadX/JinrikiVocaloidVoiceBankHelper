using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AuditionHelper.Core
{
    /// <summary>
    /// Praat TextGrid 文件读取。Praat TextGrid file reader.
    /// <para>https://www.fon.hum.uva.nl/praat/manual/TextGrid_file_formats.html</para>
    /// </summary>
    public class TextGridReader
    {

        public static Item[] ReadFromString(string contents)
        {
            contents = contents.Replace("\r\n", "\n");
            string[] lines = contents.Split('\n');
            //判断文件格式
            if (lines[0] != "File type = \"ooTextFile\"" || lines[1] != "Object class = \"TextGrid\"")
                throw new InvalidTextGridFormatException();

            //获取数据开始位置
            int i = 0;
            while(i < lines.Length)
            {
                if (lines[i].Trim() == "item [1]:")
                    break;
                i++;
            }

            //读入数据
            i += 5;
            Group g = Regex.Match(lines[i], @"intervals: size = (\d+)").Groups[1];
            i++;
            int count = int.Parse(g.Value);
            /*
            intervals [1]:
                xmin = 0 
                xmax = 0.03 
                text = "" 
            */
            List<Item> items = new List<Item>(count);
            for (int j = 0; j < count; j++)
            {
                int baseIndex = i + j * 4;
                items.Add(new Item {
                    Type = ItemType.Word,
                    Text = Regex.Match(lines[baseIndex + 3], "text = \"(.+)\" ").Groups[1].Value,
                    Start = double.Parse(Regex.Match(lines[baseIndex + 1], @"xmin = (.+) ").Groups[1].Value),
                    End = double.Parse(Regex.Match(lines[baseIndex + 2], @"xmax = (.+) ").Groups[1].Value),
                });
            }
            return items.ToArray();
        }

        public class Item
        {
            public ItemType Type { get; set; }
            public string Text { get; set; }
            public double Start { get; set; }
            public double End { get; set; }
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
    }

    
    public class InvalidTextGridFormatException : Exception { }
}
