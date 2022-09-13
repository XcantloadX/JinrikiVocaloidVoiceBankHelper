using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JinrikiVocaloidVBHelper.FileOperation;
using System.Xml.Serialization;

namespace JinrikiVocaloidVBHelper.Core
{
    /// <summary>
    /// 素材库操作
    /// </summary>
    public class MaterialLibrary
    {
        public const string CONFIG_FILE_NAME = "library.vbh";
        public string AudioPath { get; set; }
        public string VoicePath { get; set; }
        public string Name { get; set; }
        public string ConfigPath { get; set; }
        
        public static MaterialLibrary Empty
        {
            get
            {
                return new MaterialLibrary()
                {
                    AudioPath = "",
                    VoicePath = "",
                    Name = "",
                    ConfigPath = "",
                };
            }
        }
        private SearchHelper _searchHelper;
        [XmlIgnore]
        public SearchHelper SearchHelper {
            get
            {
                if (_searchHelper == null)
                    _searchHelper = new SearchHelper(AudioPath);
                return _searchHelper;
            }
        }

        public MaterialLibrary()
        {
        }

        /// <summary>
        /// 创建并保存新的素材库
        /// </summary>
        /// <param name="audioPath">音频素材路径</param>
        /// <param name="voicePath">音源路径</param>
        /// <returns></returns>
        public static MaterialLibrary Create(string audioPath, string voicePath)
        {
            if (!Directory.Exists(audioPath) || !Directory.Exists(voicePath))
                throw new DirectoryNotFoundException();

            MaterialLibrary lib = new MaterialLibrary();
            lib.AudioPath = audioPath;
            lib.VoicePath = voicePath;
            lib.ConfigPath = Path.Combine(voicePath, "character.txt");
            //读入 character.txt
            if (File.Exists(Path.Combine(voicePath, "character.txt")))
                lib.Name = CharacterReader.ReadFromFile(Path.Combine(voicePath, "character.txt")).Name;

            File.WriteAllText(Path.Combine(voicePath, CONFIG_FILE_NAME), XmlSerialize.ToXML(lib));
            return lib;
        }

        public static MaterialLibrary Read(string filePath)
        {
            if (!File.Exists(filePath))
                return Empty;
            return XmlSerialize.ToObject<MaterialLibrary>(File.ReadAllText(filePath));
        }
    }
}
