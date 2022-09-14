using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JinrikiVocaloidVBHelper.FileOperation;
using System.IO;

namespace JinrikiVocaloidVBHelper.Core
{
    /// <summary>
    /// 素材库里的单个音频
    /// </summary>
    public class LibraryAudioData
    {
        /// <summary>
        /// 该素材的标签
        /// </summary>
        public List<string> Tags { get; private set; }
        /// <summary>
        /// 素材的数据文件路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 该素材的标注数据
        /// </summary>
        public List<string> AlignData { get; private set; }

        /// <summary>
        /// 创建一个空的实例。
        /// </summary>
        public LibraryAudioData() { }

        /// <summary>
        /// 素材文件路径。
        /// </summary>
        /// <param name="dataPath"></param>
        public LibraryAudioData(string dataPath)
        {
            Tags = new List<string>(5);
            AlignData = new List<string>(50);
            Path = System.IO.Path.ChangeExtension(dataPath, ".vbad");
        }

        /// <summary>
        /// 保存数据到本地
        /// </summary>
        public void Save()
        {
            File.WriteAllText(Path, XmlSerialize.ToXML(this));
        }
    }
}
