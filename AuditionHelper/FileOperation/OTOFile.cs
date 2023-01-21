using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinrikiVocaloidVBHelper.FileOperation
{
    /// <summary>
    /// 读写 UTAU oto.ini 文件
    /// </summary>
    public class OtoFile
    {
        /// <summary>
        /// 音源条目。文件名（不带后缀） => OTO 设定内容
        /// </summary>
        public Dictionary<string, OtoCharacterParams> Items { get; private set; }
        public string FilePath { get; set; }

        public OtoFile()
        {
            Items = new Dictionary<string, OtoCharacterParams>(50);
        }

        /// <summary>
        /// 从字符串内容创建 OtoFile
        /// </summary>
        /// <param name="str">oto.ini 文件内容</param>
        /// <returns></returns>
        public static OtoFile FromString(string str)
        {
            OtoFile instance = new OtoFile();
            string[] lines = str.Replace("\r", "").Split('\n');
            instance.Items = new Dictionary<string, OtoCharacterParams>(lines.Count());

            //解析并载入 oto.ini 文件
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string[] keywords = line.Split('=');
                if (keywords.Length < 2)
                {
                    MessageBox.Show(string.Format("存在无效的音源设定：{0}，已跳过。", line), "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }
                instance.Items.Add(System.IO.Path.GetFileNameWithoutExtension(keywords[0]), new OtoCharacterParams(keywords[1]));
            }

            return instance;
        }


        /// <summary>
        /// 保存文件
        /// </summary>
        public void Save()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                throw new Exception("文件路径为空");

            StringBuilder sb = new StringBuilder();
            foreach (var item in Items)
            {
                sb.AppendLine(item.Key + ".wav=" + item.Value.ToString());
            }

            System.IO.File.WriteAllText(FilePath, sb.ToString());
        }
    }

    public class OtoCharacterParams
    {
        /// <summary>
        /// 左边界（偏移）
        /// </summary>
        public double LeftBlank { get; set; }
        /// <summary>
        /// 右边界（终止）
        /// </summary>
        public double RightBlank { get; set; }
        /// <summary>
        /// 重叠
        /// </summary>
        public double Overlap { get; set; }
        /// <summary>
        /// 先行声音
        /// </summary>
        public double PreUtterance { get; set; }
        /// <summary>
        /// 固定（辅音部分）
        /// </summary>
        public double Consonant { get; set; }
        /// <summary>
        /// 别名（辅助记号）
        /// </summary>
        public string Alias { get; set; }

        public OtoCharacterParams()
        {
            Alias = "";
        }

        public OtoCharacterParams(string otoParam)
        {
            string[] keywords = otoParam.Split(',');
            Alias = keywords[0];
            try
            {
                LeftBlank = string.IsNullOrWhiteSpace(keywords[1]) ? 0 : double.Parse(keywords[1]);
                Consonant = string.IsNullOrWhiteSpace(keywords[2]) ? 0 : double.Parse(keywords[2]);
                RightBlank = string.IsNullOrWhiteSpace(keywords[3]) ? 0 : double.Parse(keywords[3]);
                PreUtterance = string.IsNullOrWhiteSpace(keywords[4]) ? 0 : double.Parse(keywords[4]);
                Overlap = string.IsNullOrWhiteSpace(keywords[5]) ? 0 : double.Parse(keywords[5]);
            }
            catch (FormatException e)
            {
                throw new IgnorableException("读入 oto.ini 文件时出现错误：\n转换参数为 double 类型失败\n" + e.Message);
            }

        }

        public override string ToString()
        {
            return $"{Alias},{LeftBlank},{Consonant},{RightBlank},{PreUtterance},{Overlap}";
        }
    }
}
