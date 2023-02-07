using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace JinrikiVocaloidVBHelper.Core
{
    /// <summary>
    /// MFA 辅助类
    /// </summary>
    public class MFAHelper
    {
        public const string VVTALK_PATH = "tools\\VvTalk/";
        public const string RUNNER_PATH = "tools\\mfa_runner.bat";
        public const string TEMP_PATH = "tools\\temp";
        public static readonly string TEMP_BAT_PATH = Path.GetFullPath(Path.Combine(TEMP_PATH, "temp.bat"));

        /// <summary>
        /// 给指定音频文件对齐（标注）
        /// </summary>
        /// <param name="filePath">音频文件路径。必须是 wav 文件，最好是 16 位单声道</param>
        /// <param name="pinyin">拼音，需要带数字声调。如 ke3 yi3 a1（可以啊）</param>
        /// <param name="outPath">输出文件夹</param>
        public static void Align(string filePath, string pinyin, string outPath)
        {
            //写入临时文件
            if (!Directory.Exists(TEMP_PATH))
                Directory.CreateDirectory(TEMP_PATH);
            File.WriteAllText(Path.Combine(TEMP_PATH, "temp1.lab"), pinyin);
            File.Copy(filePath, Path.Combine(TEMP_PATH, "temp1.wav"));
            string bat = File.ReadAllText(RUNNER_PATH)
                .Replace("{{root}}", Path.GetFullPath(VVTALK_PATH))
                .Replace("{{in}}", Path.GetFullPath(TEMP_PATH))
                .Replace("{{out}}", Path.GetFullPath(outPath));
            File.WriteAllText(TEMP_BAT_PATH, bat);

            //运行
            Process p = Process.Start(@"C:\Windows\System32\cmd.exe", "/c " + TEMP_BAT_PATH);
            p.WaitForExit();

            //清理
            try
            {
                File.Delete(Path.Combine(TEMP_PATH, "temp1.lab"));
                File.Delete(Path.Combine(TEMP_PATH, "temp1.wav"));
                File.Delete(TEMP_BAT_PATH);
            }
            catch { }
            
        }

        /// <summary>
        /// 给指定音频文件对齐（标注）
        /// </summary>
        /// <param name="filePath">音频文件路径。必须是 wav 文件，最好是 16 位单声道</param>
        /// <param name="pinyin">拼音，需要带数字声调。如 ke3 yi3 a1（可以啊）</param>
        /// <param name="outPath">输出文件夹</param>
        public static void AlignBatch(string inPath, string outPath)
        {
            string bat = File.ReadAllText(RUNNER_PATH)
                .Replace("{{root}}", Path.GetFullPath(VVTALK_PATH))
                .Replace("{{in}}", Path.GetFullPath(inPath))
                .Replace("{{out}}", Path.GetFullPath(outPath));
            File.WriteAllText(TEMP_BAT_PATH, bat);

            //运行
            Process p = Process.Start(@"C:\Windows\System32\cmd.exe", "/c " + TEMP_BAT_PATH);
            p.WaitForExit();
        }

        public static void RunPraat()
        {
            Process p = Process.Start("tools\\Praat.exe");
            //p.WaitForExit();
        }
    }
}
