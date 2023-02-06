using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JinrikiVocaloidVBHelper.FileOperation;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace JinrikiVocaloidVBHelper.Core
{
    /// <summary>
    /// 素材库里的单个音频
    /// </summary>
    [DataContract]
    public class LibraryAudio
    {
        /// <summary>
        /// 该素材的标签
        /// </summary>
        [DataMember]
        public List<string> Tags { get; internal set; }
        /// <summary>
        /// 字幕数据
        /// </summary>
        [XmlIgnore]
        public List<SubtitleLine> Subtitles { get; internal set; }
        /// <summary>
        /// 素材的基础路径（不带后缀名）
        /// </summary>
        [DataMember]
        public string BasePath { get; set; }
        /// <summary>
        /// 素材的 xml 配置路径
        /// </summary>
        public string ConfigPath { get { return Path.ChangeExtension(BasePath, ".xml"); } }
        /// <summary>
        /// 素材的 mp3 音频文件路径
        /// </summary>

        public string AudioPath { get { return Path.ChangeExtension(BasePath, ".mp3"); } }
        /// <summary>
        /// 素材的 srt 字幕文件路径
        /// </summary>
        public string SubtitlePath { get { return Path.ChangeExtension(BasePath, ".srt"); } }
        /// <summary>
        /// 该素材的标注数据
        /// </summary>
        [DataMember]
        public List<string> AlignData { get; internal set; }

        /// <summary>
        /// 创建一个空的实例。
        /// </summary>
        public LibraryAudio() { }

        /// <summary>
        /// 创建一个新的 LibraryAudio 实例
        /// </summary>
        /// <param name="audioPath">.mp3 文件路径</param>
        /// <returns></returns>
        public static LibraryAudio Create(string audioPath)
        {
            LibraryAudio audio = new LibraryAudio();
            if (!File.Exists(audioPath))
                throw new FileNotFoundException("指定的音频文件不存在", audioPath);

            string basePath = Path.ChangeExtension(audioPath, "");
            audio.Tags = new List<string>(5);
            audio.AlignData = new List<string>(50);
            audio.BasePath = basePath;

            return audio;
        }

        /// <summary>
        /// 从 .xml 元数据文件中反序列化
        /// <param name="confPath">.xml 元数据文件路径</param>
        /// </summary>
        public static LibraryAudio Read(string confPath)
        {
            LibraryAudio audio = XmlSerialize.ToObject<LibraryAudio>(File.ReadAllText(confPath));
            //读入字幕文件
            if (File.Exists(audio.SubtitlePath))
                audio.Subtitles = new List<SubtitleLine>(SubtitleReader.ReadAllFromFile(audio.SubtitlePath));
            else
                throw new FileNotFoundException("未找到字幕文件。", audio.SubtitlePath);
            //写入文件信息
            foreach (SubtitleLine subtitle in audio.Subtitles)
            {
                subtitle.FilePath = audio.SubtitlePath;
            }
            return audio;
        }

        /// <summary>
        /// 保存数据到本地
        /// </summary>
        public void Save()
        {
            File.WriteAllText(ConfigPath, XmlSerialize.ToXML(this));
        }

       
    }
}
