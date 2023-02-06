using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JinrikiVocaloidVBHelper.FileOperation;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Windows.Forms;
using AuditionHelper.Core;
using System.Runtime.Serialization;

namespace JinrikiVocaloidVBHelper.Core
{
    /// <summary>
    /// 媒体库操作
    /// </summary>
    [DataContract]
    public class Library
    {
        #region 各种路径
        public const string CONFIG_FILE_NAME = "library.vbh";
        /// <summary>
        /// 素材库文件夹路径
        /// </summary>
        [XmlIgnore]
        public string LibraryConfigPath { get; set; }
        /// <summary>
        /// 源素材音频文件路径
        /// </summary>
        [DataMember]
        public string AudioPath { get; set; }
        /// <summary>
        /// 音源文件夹路径
        /// </summary>
        [DataMember]
        public string VoiceBankPath { get; set; }
        /// <summary>
        /// UTAU 音源配置文件（character.txt）路径
        /// </summary>
        [DataMember]
        public string CharacterPath { get; set; }
        #endregion
        #region 素材库数据
        /// <summary>
        /// 音源名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 素材库中的音频文件
        /// </summary>
        [XmlIgnore]
        public List<LibraryAudio> Audios { get; private set; }
        #endregion

        public Library()
        {
        }

        /// <summary>
        /// 创建新的素材库
        /// </summary>
        /// <param name="audioPath">音频素材路径</param>
        /// <param name="voicebankPath">音源路径</param>
        /// <returns></returns>
        public static Library Create(string audioPath, string voicebankPath)
        {
            if (!Directory.Exists(audioPath) || !Directory.Exists(voicebankPath))
                throw new DirectoryNotFoundException();

            Library lib = new Library();
            //配置基础路径
            lib.AudioPath = audioPath;
            lib.VoiceBankPath = voicebankPath;
            lib.CharacterPath = Path.Combine(voicebankPath, "character.txt");
            //读入 character.txt
            if (File.Exists(Path.Combine(voicebankPath, "character.txt")))
                lib.Name = CharacterReader.ReadFromFile(Path.Combine(voicebankPath, "character.txt")).Name;
            //创建 LibraryAudio
            lib.ReadAudiosFromFile();
            return lib;
        }

        //清空 Audios 并从本地文件读入
        private void ReadAudiosFromFile()
        {
            if (Audios == null)
                Audios = new List<LibraryAudio>();
            Audios.Clear();
            DirectoryInfo dir = new DirectoryInfo(AudioPath);
            FileInfo[] files = dir.GetFiles();
            StringBuilder failedFileList = new StringBuilder(300); //读入失败的文件列表
            foreach (var file in files)
            {
                if (file.Extension != ".mp3")
                    continue;

                //如果已经有 xml 配置文件了
                string audioXml = Path.ChangeExtension(file.FullName, ".xml"); //音频的元数据文件
                LibraryAudio audio = null;
                if (File.Exists(audioXml))
                {
                    try
                    {
                        audio = LibraryAudio.Read(audioXml);
                    }
                    catch(FileNotFoundException)
                    {
                        failedFileList.AppendLine(Path.ChangeExtension(file.Name, ".srt"));
                    }
                }
                //没有就立即创建
                else
                {
                    audio = LibraryAudio.Create(file.FullName);
                    audio.Save();
                }

                Audios.Add(audio);
            }
            //失败提示
            if (failedFileList.Length > 0)
                MessageBox.Show($"无法找到下列字幕文件（已跳过导入）！\n\n" + failedFileList.ToString(), "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 从指定配置文件读入素材库
        /// </summary>
        /// <param name="filePath">配置文件路径（library.vbh）</param>
        /// <returns>读取结果</returns>
        public static Library Read(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("给定的配置文件不存在。", filePath);
            Library lib = XmlSerialize.ToObject<Library>(File.ReadAllText(filePath)); //读入 library.xml
            lib.LibraryConfigPath = filePath;
            //读入音频文件（Audios）
            lib.ReadAudiosFromFile();
            return lib;
        }

        /// <summary>
        /// 保存当前素材库
        /// </summary>
        public void Save()
        {
            File.WriteAllText(Path.Combine(VoiceBankPath, CONFIG_FILE_NAME), XmlSerialize.ToXML(this));
        }

        /// <summary>
        /// 获取所有字幕数据
        /// </summary>
        /// <returns></returns>
        public SubtitleLine[] GetAllSubtitles()
        {
            if(Audios == null || Audios.Count <= 0)
                return new SubtitleLine[]{ };
            List<SubtitleLine> subtitles = new List<SubtitleLine>(100);
            foreach (LibraryAudio audio in Audios)
            {
                if(audio != null && audio.Subtitles != null)
                    subtitles.AddRange(audio.Subtitles);
            }
            return subtitles.ToArray();
        }

        /// <summary>
        /// 搜索所有字幕文件的内容
        /// </summary>
        /// <param name="content">要搜索的内容</param>
        /// <returns>搜索结果。</returns>
        public SubtitleLine[] SearchContent(string content)
        {
            SubtitleLine[] subtitles = GetAllSubtitles();
            List<SubtitleLine> results = new List<SubtitleLine>();

            foreach (SubtitleLine line in subtitles)
            {
                if (line.Content.Split(' ').Contains(content))
                {
                    results.Add(line);
                }
            }
            
            return results.ToArray();
        }

        /// <summary>
        /// 搜索所有字幕文件的拼音
        /// </summary>
        /// <param name="content">要搜索的拼音</param>
        /// <returns>搜索结果。</returns>
        public SubtitleLine[] SearchPinYin(string pinyin, bool fullMatch = true)
        {
            SubtitleLine[] subtitles = GetAllSubtitles();
            List<SubtitleLine> results = new List<SubtitleLine>();

            foreach (SubtitleLine line in subtitles)
            {
                if (
                    (fullMatch && line.ContentPinYin.Split(' ').Contains(pinyin)) ||
                    (!fullMatch && line.ContentPinYin.Contains(pinyin))
                    )
                {
                    results.Add(line);
                }
            }
            
            return results.ToArray();
        }

        /// <summary>
        /// 获得单个素材音频的数据文件
        /// </summary>
        /// <returns></returns>
        public LibraryAudio GetAudioConfigData(string audioFileName)
        {
            throw new NotImplementedException();
            //.vbad == voice bank (raw) audio data
            string path = Path.Combine(AudioPath, Path.ChangeExtension(audioFileName, ".vbad"));
            if (File.Exists(path))
            {
                return XmlSerialize.ToObject<LibraryAudio>(File.ReadAllText(path));
            }
            else
                return null;
        }

        /// <summary>
        /// 自动对齐/切分
        /// </summary>
        public void AutoAlign()
        {
            throw new NotImplementedException();
            if (AudioPath == "")
            {
                MessageBox.Show("请先载入素材库！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DirectoryInfo dir = new DirectoryInfo(AudioPath);
            FileInfo[] files = dir.GetFiles();
            foreach (var audioFile in files)
            {
                //TODO 支持其他格式
                if (audioFile.Extension != ".mp3")
                    continue;

                //Subtitle[] lines = SearchHelper.GetAllLinesByFileName(Path.ChangeExtension(file.FullName, ".srt")); TODO
                SubtitleLine[] lines = { };
                if (lines != null && lines.Length > 0)
                {
                    if (!Directory.Exists("temp"))
                        Directory.CreateDirectory("temp");

                    //先转码、分割音频
                    int i = 0;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Format("-i {0}", audioFile.FullName));
                    foreach (var line in lines)
                    {
                        
                        string tempWavFile = "temp\\" + i + ".wav";
                        sb.Append(string.Format(" -ss {0} -to {1} {2}", line.StartTime2, line.EndTime2, tempWavFile));

                        //写出拼音
                        string pinyin = line.ContentPinYinWithTones;
                        File.WriteAllText(Path.ChangeExtension(tempWavFile, ".lab"), pinyin);

                        //由于命令行参数有长度限制，所以我们需要分成N次运行 ffmpeg
                        //但是如果一个输出文件就运行一次 ffmpeg 会严重拖慢速度
                        //所以暂时设定为每 500 个输出文件运行一次
                        if(i % 500 == 0)
                        {
                            Process ffmpeg = Process.Start("tools\\ffmpeg.exe", sb.ToString());
                            ffmpeg.WaitForExit();
                            if (ffmpeg.ExitCode != 0)
                            {
                                //TODO 扩充 IgnorableException 详细信息的显示，改掉这里
                                MessageBox.Show("转码文件时出错。切分已终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
#if DEBUG
                                throw new Exception("ffmpeg 运行出错\n运行命令：" + sb.ToString());
#endif
                                return;
                            }
                            ffmpeg.Dispose();
                            sb.Clear();
                            sb.Append(string.Format("-i {0}", audioFile.FullName));
                        }

                        i++;
                    }

                    Process ffmpeg2 = Process.Start("tools\\ffmpeg.exe", sb.ToString());
                    ffmpeg2.WaitForExit();
                    if (ffmpeg2.ExitCode != 0)
                    {
                        MessageBox.Show("转码文件时出错。切分已终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
#if DEBUG
                        throw new Exception("ffmpeg 运行出错\n运行命令：" + sb.ToString());
#endif
                        return;
                    }
                    ffmpeg2.Dispose();
                    sb.Clear();

                    //音频文件切分完成，开始标注
                    MFAHelper.AlignBatch("temp", "temp\\out");
                    //标注结果输出到 temp\\out\\ 文件夹里

                    //然后读入所有结果合并保存
                    DirectoryInfo outDir = new DirectoryInfo("temp\\out\\");
                    FileInfo[] textGridPaths = outDir.GetFiles();
                    LibraryAudio audioData = LibraryAudio.Read(Path.ChangeExtension(audioFile.FullName, ".srt")); //TODO
                    foreach (var textGridPath in textGridPaths)
                    {
                        audioData.AlignData.Add(File.ReadAllText(textGridPath.FullName));
                    }
                    audioData.Save();
                }

                MessageBox.Show("切分完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //清除 wav 文件
                /*try
                {
                    File.Delete(wavPath);
                }
                catch { }*/
            }
        }
    }
}
