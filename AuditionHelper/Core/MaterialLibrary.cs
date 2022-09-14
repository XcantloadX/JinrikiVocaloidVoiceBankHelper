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

        /// <summary>
        /// 获得单个素材音频的数据文件
        /// </summary>
        /// <returns></returns>
        public LibraryAudioData GetAudioConfigData(string audioFileName)
        {
            //.vbad == voice bank (raw) audio data
            string path = Path.Combine(AudioPath, Path.ChangeExtension(audioFileName, ".vbad"));
            if (File.Exists(path))
            {
                return XmlSerialize.ToObject<LibraryAudioData>(File.ReadAllText(path));
            }
            else
                return null;
        }

        /// <summary>
        /// 自动对齐/切分
        /// </summary>
        public void AutoAlign()
        {
            if(AudioPath == "")
            {
                MessageBox.Show("请先载入素材库！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DirectoryInfo dir = new DirectoryInfo(AudioPath);
            FileInfo[] files = dir.GetFiles();
            foreach (var file in files)
            {
                //TODO 支持其他格式
                if (file.Extension != ".mp3")
                    continue;

                SrtLine[] lines = SearchHelper.GetAllLinesByFileName(Path.ChangeExtension(file.FullName, ".srt"));
                if(lines != null && lines.Length > 0)
                {
                    if (!Directory.Exists("temp"))
                        Directory.CreateDirectory("temp");

                    //先转码、分割音频
                    int i = 0;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Format("-i {0}", file.FullName));
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
                                MessageBox.Show("转码文件时出错。切分已终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
#if DEBUG
                                throw new Exception("ffmpeg 运行出错\n运行命令：" + sb.ToString());
#endif
                                return;
                            }
                            ffmpeg.Dispose();
                            sb.Clear();
                            sb.Append(string.Format("-i {0}", file.FullName));
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
                    LibraryAudioData audioData = new LibraryAudioData(file.FullName);
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
