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
        public Dictionary<string, OTOSettings> Items { get; private set; }

        public OtoFile(string fileContent)
        {
            string[] lines = fileContent.Replace("\r", "").Split('\n');
            Items = new Dictionary<string, OTOSettings>(lines.Count());

            //解析并载入 oto.ini 文件
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string[] keywords = line.Split('=');
                if(keywords.Length < 2)
                {
                    MessageBox.Show(string.Format("存在无效的音源设定：{0}，已跳过。", line), "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }
                Items.Add(System.IO.Path.GetFileNameWithoutExtension(keywords[0]), new OTOSettings(keywords[1]));
            }
        }
    }

    public class OTOSettings
    {
        public OTOSettings(string utauOTOSetting)
        {

        }
    }
}
