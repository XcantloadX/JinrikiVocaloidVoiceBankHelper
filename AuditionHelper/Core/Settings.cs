using JinrikiVocaloidVBHelper.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Drawing.Point;

namespace JinrikiVocaloidVBHelper.Core
{

    public class Settings
    {
        private const string SETTINGS = "Settings";
        private const string ID_OpenFileWaitTimeFactor = "OpenFileWaitTimeFactor";

        public Shortcuts Shortcut { get; set; }
        public Last Last { get; set; }
        public string SettingFilePath { get; set; }

        private IniFile iniFile;
        public double OpenFileWaitTimeFactor;

        /// <param name="filePath">设置文件路径</param>
        public Settings(string filePath)
        {
            SettingFilePath = filePath;
            if(System.IO.File.Exists(filePath))
                Read();
        }

        public void Read()
        {
            iniFile = new IniFile(SettingFilePath);
            Shortcut = new Shortcuts(iniFile);
            Last = new Last(iniFile);
            OpenFileWaitTimeFactor = iniFile.Read2<float>(ID_OpenFileWaitTimeFactor, SETTINGS);
        }

        public void Save()
        {
            iniFile.Write(ID_OpenFileWaitTimeFactor, OpenFileWaitTimeFactor.ToString(), SETTINGS);

            Shortcut.Save();
            Last.Save();
        }
    }

    public class Shortcuts : SettingsBase
    {
        private const string SHORTCUTS = "Shortcuts";
        public Dictionary<string, int> ShortcutRecords;

        public Shortcuts(IniFile iniFile) : base(iniFile)
        {
        }

        public override void Read()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }

    public class Last : SettingsBase
    {
        private const string LAST = "Last";
        private const string LAST_LIB = "lastLibrary";
        private const string LAST_SEARCH = "lastSearch";
        private const string LAST_INDEX = "lastIndex";
        private const string LAST_MATCH_FULL_WORD = "lastMatchFullWord";
        private const string LAST_MAIN_X = "mainX";
        private const string LAST_MAIN_Y = "mainY";
        private const string LAST_FLOAT_X = "floatX";
        private const string LAST_FLOAT_Y = "floatY";
        private const string LAST_RECENT_0 = "recentProject0";
        private const string LAST_RECENT_1 = "recentProject1";
        private const string LAST_RECENT_2 = "recentProject2";
        private const string LAST_RECENT_3 = "recentProject3";
        private const string LAST_RECENT_4 = "recentProject4";

        /// <summary>
        /// 上次打开的库
        /// </summary>
        public string LibraryPath;
        /// <summary>
        /// 上次的搜索内容
        /// </summary>
        public string SearchContent;
        /// <summary>
        /// 上次选中的下标
        /// </summary>
        public int Index;
        /// <summary>
        /// 上次是否为全词匹配
        /// </summary>
        public bool IsMatchFullWord;
        /// <summary>
        /// 上次主窗口的位置
        /// </summary>
        public Point MainWindowPosition;
        /// <summary>
        /// 上次浮动窗口的位置
        /// </summary>
        public Point FloatToolWindowPosition;
        /// <summary>
        /// 最近打开的项目
        /// </summary>
        public List<string> RecentProjects = new List<string>(new string[] { "", "", "", "", "" });

        public Last(IniFile iniFile) : base(iniFile)
        {
        }

        public override void Read()
        {
            LibraryPath = IniFile.Read(LAST_LIB, LAST);
            Index = IniFile.Read2<int>(LAST_INDEX, LAST);
            SearchContent = IniFile.Read(LAST_SEARCH, LAST);
            MainWindowPosition = new Point(IniFile.Read2<int>(LAST_MAIN_X, LAST), IniFile.Read2<int>(LAST_MAIN_Y, LAST));
            FloatToolWindowPosition = new Point(IniFile.Read2<int>(LAST_FLOAT_X, LAST), IniFile.Read2<int>(LAST_FLOAT_Y, LAST));
            RecentProjects[0] = IniFile.Read2<string>(LAST_RECENT_0, LAST);
            RecentProjects[1] = IniFile.Read2<string>(LAST_RECENT_1, LAST);
            RecentProjects[2] = IniFile.Read2<string>(LAST_RECENT_2, LAST);
            RecentProjects[3] = IniFile.Read2<string>(LAST_RECENT_3, LAST);
            RecentProjects[4] = IniFile.Read2<string>(LAST_RECENT_4, LAST);
            IsMatchFullWord = IniFile.Read2<bool>(LAST_MATCH_FULL_WORD);
        }

        public override void Save()
        {
            IniFile.Write(LAST_LIB, LibraryPath, LAST);
            IniFile.Write(LAST_SEARCH, SearchContent, LAST);
            IniFile.Write(LAST_INDEX, Index.ToString(), LAST);
            IniFile.Write(LAST_MAIN_X, MainWindowPosition.X.ToString(), LAST);
            IniFile.Write(LAST_MAIN_Y, MainWindowPosition.Y.ToString(), LAST);
            IniFile.Write(LAST_FLOAT_X, FloatToolWindowPosition.X.ToString(), LAST);
            IniFile.Write(LAST_FLOAT_Y, FloatToolWindowPosition.Y.ToString(), LAST);
            IniFile.Write(LAST_MATCH_FULL_WORD, IsMatchFullWord.ToString(), LAST);
            IniFile.Write(LAST_RECENT_0, RecentProjects[0], LAST);
            IniFile.Write(LAST_RECENT_1, RecentProjects[1], LAST);
            IniFile.Write(LAST_RECENT_2, RecentProjects[2], LAST);
            IniFile.Write(LAST_RECENT_3, RecentProjects[3], LAST);
            IniFile.Write(LAST_RECENT_4, RecentProjects[4], LAST);
        }
    }

    public abstract class SettingsBase
    {
        protected IniFile IniFile { get; set; }
        public SettingsBase() { }
        public SettingsBase(IniFile iniFile)
        {
            this.IniFile = iniFile;
            Read();
        }
        public abstract void Save();
        public abstract void Read();
    }
}
