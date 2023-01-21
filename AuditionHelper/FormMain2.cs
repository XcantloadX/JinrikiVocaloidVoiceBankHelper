using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using WindowsInput;
using System.Diagnostics;
using System.Text.RegularExpressions;
using JinrikiVocaloidVBHelper.Util;
using JinrikiVocaloidVBHelper.Core;
using JinrikiVocaloidVBHelper.Audition;
using JinrikiVocaloidVBHelper.Automation;
using System.Collections.Generic;
using JinrikiVocaloidVBHelper.FileOperation;

namespace JinrikiVocaloidVBHelper
{
    public partial class FormMain
    {
        /// <summary>
        /// 取指定发音名的下一个文件名。比如现在有 ba.wav ba1.wav，那么传入 ba 则会返回 ba2.wav
        /// </summary>
        /// <param name="voiceName">发音名</param>
        /// <returns></returns>
        public string GetNextVoiceFileName(string voiceName)
        {
            DirectoryInfo dir = new DirectoryInfo(CurrentLibrary.VoicePath);
            FileInfo[] files = dir.GetFiles();
            //找出所有 a.wav a1.wav a2.wav 等文件
            var wavFiles =
                (from file in files
                 where Regex.IsMatch(file.Name, string.Format(@"{0}\d*.wav", voiceName)) && file.Name.EndsWith("wav")
                 orderby file.Name ascending
                 select file.Name).ToArray();

            if (wavFiles.Length <= 0) //之前没有这个字
            {
                return voiceName + ".wav";
            }
            else if (wavFiles[0] == voiceName) //之前只有一个，即 a.wav
            {
                return voiceName + "1.wav";
            }
            else
            {
                return voiceName + wavFiles.Length + ".wav";
            }
        }

        /// <summary>
        /// 预览当前选区
        /// </summary>
        public void PreviewCurrent()
        {
            //TODO 加上模板选择、参数选择等 UI
            //TODO 自动估算 OTO

            //保存选区
            if (!Directory.Exists("temp/tempVoiceBank"))
                Directory.CreateDirectory("temp/tempVoiceBank");
            AuditionController.SaveSelection("temp.wav", "temp/tempVoiceBank");
            //创建 oto.ini 文件
            OtoFile oto = new OtoFile();
            oto.FilePath = Path.GetFullPath("temp/tempVoiceBank/oto.ini");
            oto.Items.Add("temp", new OtoCharacterParams() { });
            oto.Save();
            //渲染
            string path = UTAUController.PreviewSingle(
                Path.GetFullPath("temp/tempVoiceBank"),
                @"D:\PortableApps\UTAU\moresampler.exe", //tool
                @"D:\PortableApps\UTAU\moresampler.exe", //resampler
                "temp", //字
                1200, //音长
                "C4", //音高
                "e" //flag
           );
            SoundPlayer.PlaySound(path);
        }

        /// <summary>
        /// 打开指定的素材库
        /// </summary>
        /// <param name="lib">素材库类实例</param>
        public void OpenLibrary(MaterialLibrary lib)
        {
            CurrentLibrary = lib;
            btnSearch_Click(null, null);
            if (string.IsNullOrEmpty(CurrentLibrary.Name))
                this.Text = new DirectoryInfo(CurrentLibrary.VoicePath).Name + " - UTAU 音源制作助手";
            else
                this.Text = CurrentLibrary.Name + " - UTAU 音源制作助手";

            //更新最近项目
            //先判断新打开的项目是否已经存在于列表中
            List<string> list = settings.Last.RecentProjects;
            if (list.Contains(lib.LibraryConfigPath))
            {
                list.Remove(lib.LibraryConfigPath);
                list.Insert(0, lib.LibraryConfigPath);
            }
            //否则删除最后一个，把新的插到第一个
            else
            {
                list.Insert(0, lib.LibraryConfigPath);
                list.RemoveAt(5);
            }

            LoadRecentProjectsMenu();

        }

        /// <summary>
        /// 打开指定的素材库
        /// </summary>
        /// <param name="lib">素材库配置文件路径</param>
        public void OpenLibrary(string libConfigPath)
        {
            OpenLibrary(MaterialLibrary.Read(libConfigPath));

            //刷新UI
            filesPanel.Library = CurrentLibrary;
            filesPanel.FillList();
        }

    }
}
