using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JinrikiVocaloidVoiceBankHelper.Core
{
    /// <summary>
    /// 字幕搜索助手
    /// </summary>
    internal class SearchHelper
    {
        //字幕文件内容
        private Dictionary<string, SrtLine[]> subtitles;
        public int Count { get { return subtitles.Count; } }

        public SearchHelper(string path)
        {
            //读入所有文件
            if (string.IsNullOrEmpty(path))
            {
                subtitles = new Dictionary<string, SrtLine[]>(0);
                return;
            }
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            subtitles = new Dictionary<string, SrtLine[]>(files.Length);
            foreach (var file in files)
            {
                if (file.Extension != ".srt")
                    continue;
                subtitles.Add(file.FullName, SrtReader.ReadAllFile(file.FullName));
            }
        }

        /// <summary>
        /// 搜索所有字幕文件的内容
        /// </summary>
        /// <param name="content">要搜索的内容</param>
        /// <returns>搜索结果。文件路径 => 字幕</returns>
        public SrtLine[] SearchContent(string content)
        {
            List<SrtLine> srtLines = new List<SrtLine>();
            foreach (var kvPair in subtitles)
            {
                foreach (var line in kvPair.Value)
                {
                    if (line.Content.Split(' ').Contains(content))
                    {
                        line.FilePath = kvPair.Key;
                        srtLines.Add(line);
                    }
                }
            }

            return srtLines.ToArray();
        }

        /// <summary>
        /// 搜索所有字幕文件的拼音
        /// </summary>
        /// <param name="content">要搜索的拼音</param>
        /// <returns>搜索结果。文件路径 => 字幕</returns>
        public SrtLine[] SearchPinYin(string pinyin, bool fullMatch=true)
        {
            List<SrtLine> srtLines = new List<SrtLine>();
            foreach (var kvPair in subtitles)
            {
                foreach (var line in kvPair.Value)
                {
                    if (
                        (fullMatch && line.ContentPinYin.Split(' ').Contains(pinyin)) ||
                        (!fullMatch && line.ContentPinYin.Contains(pinyin))
                        )
                    {
                        line.FilePath = kvPair.Key;
                        srtLines.Add(line);
                    }
                }
            }

            return srtLines.ToArray();
        }

        public string GetPathByIndex(int index)
        {
            return subtitles.Keys.ToArray()[index];
        }
    }
}
