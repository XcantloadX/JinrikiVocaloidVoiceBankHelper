using hyjiacan.py4n;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinrikiVocaloidVBHelper.Core
{
    /// <summary>
    /// 单行字幕数据
    /// </summary>
    public class SubtitleLine : IComparable<SubtitleLine>
    {
        /// <summary>
        /// 本行字幕所属的 .srt 文件路径
        /// </summary>
        public string FilePath { get; internal set; }
        /// <summary>
        /// 字幕编号
        /// </summary>
        public int Number { get; internal set; }
        /// <summary>
        /// 开始时间。（00:00:01,233）
        /// </summary>
        public string StartTime { get; internal set; }
        /// <summary>
        /// 开始时间的另一种更通用的格式。（00:00:01.233）
        /// </summary>
        public string StartTime2 { get { return StartTime.Replace(',', '.'); } }
        /// <summary>
        /// 结束时间。（00:00:01,233）
        /// </summary>
        public string EndTime { get; internal set; }
        /// <summary>
        /// 结束时间的另一种更通用的格式。（00:00:01.233）
        /// </summary>
        public string EndTime2 { get { return EndTime.Replace(',', '.'); } }
        /// <summary>
        /// 字幕内容
        /// </summary>
        public string Content { get; internal set; }
        /// <summary>
        /// 字幕内容的拼音
        /// </summary>
        public string ContentPinYin
        {
            get
            {
                PinyinFormat format = PinyinFormat.WITHOUT_TONE | PinyinFormat.LOWERCASE;
                return Pinyin4Net.GetPinyin(Content, format);
            }
        }
        /// <summary>
        /// 字幕内容带声调的拼音
        /// </summary>
        public string ContentPinYinWithTones
        {
            get
            {
                PinyinFormat format = PinyinFormat.WITH_TONE_NUMBER | PinyinFormat.LOWERCASE;
                return Pinyin4Net.GetPinyin(Content, format);
            }
        }
        /// <summary>
        /// 字幕的速度，即该句字幕字数 / 该句总时长
        /// </summary>
        public double Speed
        {
            get
            {
                return Content.Replace(" ", string.Empty).Length / Duration.TotalSeconds;
            }
        }
        /// <summary>
        /// 字幕时长
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                TimeSpan start = TimeSpan.ParseExact(StartTime, "hh':'mm':'ss','fff", System.Globalization.CultureInfo.InvariantCulture);
                TimeSpan end = TimeSpan.ParseExact(EndTime, "hh':'mm':'ss','fff", System.Globalization.CultureInfo.InvariantCulture);
                return end - start;
            }
        }
        /// <summary>
        /// 比较两行字幕的速度
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SubtitleLine other)
        {
            return other.Speed.CompareTo(Speed);
        }
    }
}
