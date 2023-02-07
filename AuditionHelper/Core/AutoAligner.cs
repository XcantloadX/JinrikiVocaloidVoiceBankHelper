using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinrikiVocaloidVBHelper.Core
{
    /// <summary>
    /// 自动标注音频文件，以便单字试听音频
    /// </summary>
    public class AutoAligner
    {

        /// <summary>
        /// 按字幕分句切割音频文件
        /// </summary>
        private static bool SplitAudio(string audioPath, string outputPath, SubtitleLine[] subtitleLines)
        {
            int counter = 1;
            string ffmpegPath = Path.GetFullPath("tools\\ffmpeg.exe");
            StringBuilder sbBatFile = new StringBuilder(); //批处理文件内容
            StringBuilder sbCommandLine = new StringBuilder(); //单行命令

            sbBatFile.AppendLine("@echo off");
            sbCommandLine.Append(string.Format("{1} -i \"{0}\"", audioPath, ffmpegPath));
            foreach (SubtitleLine line in subtitleLines)
            {
                string tempWavFile = Path.GetFullPath("temp\\align\\" + counter + ".wav");
                sbCommandLine.Append(string.Format(" -ss {0} -to {1} {2}", line.StartTime2, line.EndTime2, tempWavFile));

                //由于命令行参数有长度限制，所以我们需要分成N次运行 ffmpeg
                //但是如果一个输出文件就运行一次 ffmpeg 会严重拖慢速度
                //所以暂时设定为每 500 个输出文件运行一次
                if (counter % 500 == 0)
                {
                    sbBatFile.AppendLine(sbCommandLine.ToString());
                    sbCommandLine.Clear();
                    sbCommandLine.Append(string.Format("{1} -i {0}", audioPath, ffmpegPath));
                }

                counter++;
            }

            if(counter % 500 != 0)
            {
                sbBatFile.AppendLine(sbCommandLine.ToString());
                sbCommandLine.Clear();
                sbCommandLine.Append(string.Format("tools\\ffmpeg.exe -i {0}", audioPath));
            }

            //运行 bat 文件
            bool succeed = true;
            File.WriteAllText("temp\\align\\split_audio.bat", sbBatFile.ToString(), ASCIIEncoding.Default);
            Process p = Process.Start(@"C:\Windows\System32\cmd.exe", "/c " + Path.GetFullPath("temp\\align\\split_audio.bat"));
            p.WaitForExit();
            if(p.ExitCode != 0)
            {
                MessageBox.Show("切分文件 " + audioPath + " 时出错。");
                succeed = false;
            }
            p.Close();
            p.Dispose();
            File.Delete(Path.GetFullPath("temp\\align\\split_audio.bat"));
            return succeed;
        }

        /// <summary>
        /// 读入字幕文件并分割每一句到单独的文件里
        /// </summary>
        /// <param name="outputPath">输出文件夹</param>
        /// <param name="subtitleLines">输入字幕</param>
        private static void SplitSubtitle(string outputPath, SubtitleLine[] subtitleLines)
        {
            
            for (int i = 0; i < subtitleLines.Length; i++)
            {
                File.WriteAllText(Path.Combine(outputPath, (i + 1).ToString() + ".lab"), subtitleLines[i].ContentPinYinWithTones);
            }
        }

        /// <summary>
        /// 读入某文件夹下所有的 TextGird 文件到指定的 LibraryAudio 对象下，LibraryAudio 里原有的数据将会被清空
        /// </summary>
        /// <param name="textgirdDir"></param>
        /// <param name="audio"></param>
        private static void ReadTextGirds(string textgirdDir, LibraryAudio audio)
        {
            audio.AlignData.Clear();
            string[] files = Directory.GetFiles(textgirdDir);
            for (int i = 0; i < audio.Subtitles.Count; i++)
            {
                string fileContents = File.ReadAllText(files[i]);
                audio.AlignData.Add(LibrarySubtitleLineAlign.ReadFromString(fileContents));
            }
        }

        /// <summary>
        /// 标注单个音频文件，标注结果会写回参数里。
        /// </summary>
        /// <param name="audio">音频文件</param>
        public static LibraryAudio Align(LibraryAudio audio)
        {
            CleanTempFiles();
            if (!Directory.Exists("temp\\align"))
                Directory.CreateDirectory("temp\\align");
            bool result = true;
            result = SplitAudio(audio.AudioPath, "temp\\align", audio.Subtitles.ToArray());
            if(!result)
            {
                MessageBox.Show("分割音频失败！");
                return audio;
            }
            SplitSubtitle("temp\\align", audio.Subtitles.ToArray());
            MFAHelper.AlignBatch("temp\\align", "temp\\align\\output");
            ReadTextGirds("temp\\align\\output", audio);
            audio.Save(); //保存结果
            return audio;
        }

        /// <summary>
        /// 标注多个音频文件，标注结果会写回参数里。
        /// </summary>
        /// <param name="audios">音频文件数组</param>
        public static void Align(LibraryAudio[] audios)
        {
            foreach (var audio in audios)
            {
                if (audio == null)
                    continue;
                Align(audio);
            }
        }

        /// <summary>
        /// 清除产生的临时文件
        /// </summary>
        public static void CleanTempFiles()
        {
            try
            {
                if(Directory.Exists("temp\\align"))
                    Directory.Delete("temp\\align", true);
            }
            catch (Exception)
            {

                throw;
            }
        }

//        / <summary>
//        / 自动对齐/切分
//        / </summary>
//        public void AutoAlign(string AudioPath)
//        {
//            throw new NotImplementedException();
//            if (AudioPath == "")
//            {
//                MessageBox.Show("请先载入素材库！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }
//            DirectoryInfo dir = new DirectoryInfo(AudioPath);
//            FileInfo[] files = dir.GetFiles();
//            foreach (var audioFile in files)
//            {
//                if (audioFile.Extension != ".mp3")
//                    continue;

//                Subtitle[] lines = SearchHelper.GetAllLinesByFileName(Path.ChangeExtension(file.FullName, ".srt")); TODO
//                SubtitleLine[] lines = { };
//                if (lines != null && lines.Length > 0)
//                {
//                    if (!Directory.Exists("temp"))
//                        Directory.CreateDirectory("temp");

//                    先转码、分割音频
//                    int i = 0;
//                    StringBuilder sb = new StringBuilder();
//                    sb.Append(string.Format("-i {0}", audioFile.FullName));
//                    foreach (var line in lines)
//                    {

//                        string tempWavFile = "temp\\" + i + ".wav";
//                        sb.Append(string.Format(" -ss {0} -to {1} {2}", line.StartTime2, line.EndTime2, tempWavFile));

//                        写出拼音
//                        string pinyin = line.ContentPinYinWithTones;
//                        File.WriteAllText(Path.ChangeExtension(tempWavFile, ".lab"), pinyin);

//                        由于命令行参数有长度限制，所以我们需要分成N次运行 ffmpeg
//                        但是如果一个输出文件就运行一次 ffmpeg 会严重拖慢速度
//                        所以暂时设定为每 500 个输出文件运行一次
//                        if (i % 500 == 0)
//                        {
//                            Process ffmpeg = Process.Start("tools\\ffmpeg.exe", sb.ToString());
//                            ffmpeg.WaitForExit();
//                            if (ffmpeg.ExitCode != 0)
//                            {
//                                TODO 扩充 IgnorableException 详细信息的显示，改掉这里
//                                MessageBox.Show("转码文件时出错。切分已终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
//#if DEBUG
//                                throw new Exception("ffmpeg 运行出错\n运行命令：" + sb.ToString());
//#endif
//                                return;
//                            }
//                            ffmpeg.Dispose();
//                            sb.Clear();
//                            sb.Append(string.Format("-i {0}", audioFile.FullName));
//                        }

//                        i++;
//                    }

//                    Process ffmpeg2 = Process.Start("tools\\ffmpeg.exe", sb.ToString());
//                    ffmpeg2.WaitForExit();
//                    if (ffmpeg2.ExitCode != 0)
//                    {
//                        MessageBox.Show("转码文件时出错。切分已终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
//#if DEBUG
//                        throw new Exception("ffmpeg 运行出错\n运行命令：" + sb.ToString());
//#endif
//                        return;
//                    }
//                    ffmpeg2.Dispose();
//                    sb.Clear();

//                    音频文件切分完成，开始标注
//                    MFAHelper.AlignBatch("temp", "temp\\out");
//                    标注结果输出到 temp\\out\\ 文件夹里

//                    然后读入所有结果合并保存
//                    DirectoryInfo outDir = new DirectoryInfo("temp\\out\\");
//                    FileInfo[] textGridPaths = outDir.GetFiles();
//                    LibraryAudio audioData = LibraryAudio.Read(Path.ChangeExtension(audioFile.FullName, ".srt")); //TODO
//                    foreach (var textGridPath in textGridPaths)
//                    {
//                        audioData.AlignData.Add(File.ReadAllText(textGridPath.FullName));
//                    }
//                    audioData.Save();
//                }

//                MessageBox.Show("切分完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

//                清除 wav 文件
//                /*try
//                {
//                    File.Delete(wavPath);
//                }
//                catch { }*/
//            }

//        }
    }
}
