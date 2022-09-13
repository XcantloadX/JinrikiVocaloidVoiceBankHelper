using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JinrikiVocaloidVBHelper.FileOperation
{
    /// <summary>
    /// UTAU 音源库 character.txt 读取器
    /// </summary>
    public class CharacterReader
    {
        /// <summary>
        /// 从指定文件里读取 UTAU character.txt 信息。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Character ReadFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            Character c = new Character();
            foreach (var line in lines)
            {
                string[] keywords = line.Split('=');
                switch (keywords[0])
                {
                    case "name":
                        c.Name = keywords[1];
                        break;
                    case "image":
                        c.Image = keywords[1];
                        break;
                    case "author":
                        c.Author = keywords[1];
                        break;
                    default:
                        break;
                }
            }

            return c;
        }
    }

    public class Character
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Author { get; set; }
    }
}
