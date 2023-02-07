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
using JinrikiVocaloidVBHelper.UI;

namespace JinrikiVocaloidVBHelper
{
    public partial class FormMain : Form
    {
        private KeyboardHook kbd = new KeyboardHook();
        private InputSimulator simulator = new InputSimulator();
        private SubtitleLine[] result = null;
        private IniFile conf = new IniFile("settings.ini");


        private int _index = 0;
        /// <summary>
        /// 当前下标
        /// </summary>
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                if (_index < listView1.Items.Count)
                    listView1.Items[value].Selected = true;
                lblIndex.Text = value.ToString();
            }
        }
        private string lastFile = "";
        public Library CurrentLibrary { get; set; }
        private FormFloat formFloat;
        /// <summary>
        /// 当前正在编辑的音源名称
        /// </summary>
        public string CurrentVoiceName
        {
            get { return txtSearch.Text; }
        }
        public SubtitleLine CurrentSubtitle { get { return result[Index]; } }
        [Obsolete]
        /// <summary>
        /// 下一个音源的文件名
        /// </summary>
        public string NextVoiceFileName
        {
            get
            {
                return GetNextVoiceFileName(CurrentVoiceName);
            }
        }
        /// <summary>
        /// 是否为全词匹配
        /// </summary>
        public bool FullMatch
        {
            get { return checkBoxMatchFullWord.Checked; }
            set { checkBoxMatchFullWord.Checked = value; }
        }

        public AuditionController AuditionController { get; private set; }
        public UTAUController UTAU { get; private set; }

        private Settings settings;
        private FilesPanel filesPanel = new FilesPanel(null);

        public FormMain()
        {
            //DEBUG
            //LibraryAudio a = LibraryAudio.Read(@"D:\工作区\B站视频存档\怒九笑-audio-only-去伴奏\【Bug惊魂5】笑爆！暴躁老兵带着他的乡下老板没心没肺的乡村生活.xml");
            //AutoAligner.Align(a);
            //a.Save();
            //SoundPlayer.PlayAudio(a.AudioPath, a.AlignData[0].Items[1].Start, a.AlignData[0].Items[1].End);
            ////-i 输入文件 -ss 开始时间 -t 播放长度

            //string args = string.Format("-i \"{0}\" -ss {1} -t {2} -autoexit", a.AudioPath, a.AlignData[0].Items[1].Start + a.Subtitles[0].StartSecond, a.AlignData[0].Items[1].End - a.AlignData[0].Items[1].Start);

            //Process p = new Process
            //{
            //    StartInfo = new ProcessStartInfo
            //    {
            //        Arguments = args,
            //        FileName = "tools\\ffplay.exe",
            //        CreateNoWindow = true,
            //        WindowStyle = ProcessWindowStyle.Hidden
            //    }
            //};
            //p.Start();
            //p.Dispose();
            InitializeComponent();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(AuditionController != null)
                AuditionController.Dispose();

            //保存数据
            settings.Last.LibraryPath = CurrentLibrary.CharacterPath;
            settings.Last.SearchContent = txtSearch.Text;
            settings.Last.Index = Index;
            settings.Last.MainWindowPosition = Location;
            if(formFloat != null)
                settings.Last.FloatToolWindowPosition = formFloat.Location;
            settings.Last.IsMatchFullWord = checkBoxMatchFullWord.Checked;
            settings.OpenFileWaitTimeFactor = AuditionKeyboardController.OpenFileWaitTimeFactor;
            settings.Save();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //载入上次数据
            settings = new Settings("settings.ini");


            Index = settings.Last.Index;
            txtSearch.Text = settings.Last.SearchContent;
            Location = settings.Last.MainWindowPosition;

            AuditionKeyboardController.OpenFileWaitTimeFactor = settings.OpenFileWaitTimeFactor;
            FullMatch = settings.Last.IsMatchFullWord;

            //初始化
            AuditionController = new AuditionExtendScriptController();

            try
            {
                //注册热键
                kbd.RegisterHotKey(Util.ModifierKeys.Control, Keys.Right); //下一个位置
                kbd.RegisterHotKey(Util.ModifierKeys.Control, Keys.Left); //上一个位置
                                                                          //Ctrl + R 重载当前选区
                kbd.RegisterHotKey(Util.ModifierKeys.Control, Keys.R);
                //Ctrl + Shift + R 重载当前文件
                kbd.RegisterHotKey(Util.ModifierKeys.Control | Util.ModifierKeys.Shift, Keys.R);
                //Ctrl + Alt + R 打开指定文件并重载当前选区
                kbd.RegisterHotKey(Util.ModifierKeys.Control | Util.ModifierKeys.Alt, Keys.R);
                //Ctrl + E 设置下标
                kbd.RegisterHotKey(Util.ModifierKeys.Control, Keys.E);
                //Ctrl + Alt + G 清理无用文件
                kbd.RegisterHotKey(Util.ModifierKeys.Control | Util.ModifierKeys.Alt, Keys.G);
                //Ctrl + Space 试听效果
                kbd.RegisterHotKey(Util.ModifierKeys.Control, Keys.Space);
            }
            catch (InvalidOperationException)
            {
#if RELEASE
                throw new IgnorableException("注册热键失败，可能其他软件已占用该快捷键。");
#endif
            }


            kbd.KeyPressed += Kbd_KeyPressed;

            //UI 初始化
            LoadRecentProjectsMenu();

            formFloat = new FormFloat(this);
            formFloat.Location = settings.Last.FloatToolWindowPosition;
            formFloat.Show();

            //载入文件面板
            filesPanel.TopLevel = false;
            filesPanel.BringToFront();
            filesPanel.Dock = DockStyle.Fill;
            filesPanel.Show();
            tabPage3.Controls.Add(filesPanel);

            //载入编辑面板
        }

        /// <summary>
        /// 将最近项目载入到菜单栏里
        /// </summary>
        private void LoadRecentProjectsMenu()
        {
            menuRecentProject.DropDownItems.Clear();
            foreach (var project in settings.Last.RecentProjects)
            {
                if (!string.IsNullOrWhiteSpace(project))
                {
                    ToolStripItem item = menuRecentProject.DropDownItems.Add(project);
                    item.Click += (object sender, EventArgs args) =>
                    {
                        OpenLibrary(item.Text);
                    };
                }
            }
        }

        private void Kbd_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            Sleep(400); //避免太快，用户还没来得及放开上一个快捷键

            //下一个
            if (e.Key == Keys.Right && e.Modifier == Util.ModifierKeys.Control)
            {
                MoveNext();
                LoadCurrent();
            }
            //上一个
            else if (e.Key == Keys.Left && e.Modifier == Util.ModifierKeys.Control)
            {
                MovePrev();
                LoadCurrent();
            }
            //重载当前选区
            else if (e.Key == Keys.R && e.Modifier == Util.ModifierKeys.Control)
            {
                LoadCurrent();
            }
            //重载当前文件
            else if (e.Key == Keys.R && e.Modifier == (Util.ModifierKeys.Shift | Util.ModifierKeys.Control))
            {
                OpenFile(System.IO.Path.ChangeExtension(result[Index].FilePath, ".mp3"));
                LoadCurrent();
            }
            //打开指定文件并载入当前选区
            else if (e.Key == Keys.R && e.Modifier == (Util.ModifierKeys.Alt | Util.ModifierKeys.Control))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "mp3 文件|*.mp3|所有文件|*.*";
                dialog.Title = "选择要打开的代替音频文件";
                if(dialog.ShowDialog() == DialogResult.OK)
                {
                    OpenFile(dialog.FileName, true);
                    LoadCurrent();
                }
                dialog.Dispose();
            }
            //跳到指定位置
            else if (e.Key == Keys.E && e.Modifier == Util.ModifierKeys.Control)
            {
                string input = "";
                UIHelper.ShowInputDialog(ref input);
                int newIndex = -1;
                int.TryParse(input, out newIndex);
                if(newIndex != -1)
                {
                    Index = newIndex;
                    LoadCurrent();
                }
            }
            //清理无用文件
            else if (e.Key == Keys.G && e.Modifier == (Util.ModifierKeys.Alt | Util.ModifierKeys.Control))
            {
                toolStripMenuItem3_Click(null, null);
            }
            //试听效果
            else if (e.Key == Keys.Space && e.Modifier == Util.ModifierKeys.Control)
            {
                PreviewCurrent();
            }
        }   

        /// <summary>
        /// 向后移动下标
        /// </summary>
        private void MoveNext()
        {
            
            if (result == null || result.Length == 0)
                return;
            Index++;
            if (Index > result.Length)
                Index = 0;
        }

        /// <summary>
        /// 向前移动下标
        /// </summary>
        private void MovePrev()
        {
            if (result == null || result.Length == 0)
                return;
            Index--;
            if (Index < 0)
                Index = result.Length;
        }

        /// <summary>
        /// 选中当前 index
        /// </summary>
        private void LoadCurrent()
        {
            AuditionController.EnsureActived();
            formFloat.UpdateUI();

            //检测重复打开的情况
            string audioPath = System.IO.Path.ChangeExtension(result[Index].FilePath, ".mp3");
            if (lastFile != audioPath)
                OpenFile(audioPath);

            AuditionController.Select(result[Index].StartTime, result[Index].EndTime);
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="alternativeMode">代替模式。为真时将不会改变 lastFile 的值。</param>
        private void OpenFile(string path, bool alternativeMode=false)
        {
            if(!alternativeMode)
                lastFile = path;

            AuditionController.OpenFile(path);
        }

        private void Sleep(int time=100)
        {
            System.Threading.Thread.Sleep(time);
        }


#region UI 事件
        //搜索
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Index = 0;
            if(CurrentLibrary == null)
            {
                MessageBox.Show("未打开任何素材库或素材库无效！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            result = CurrentLibrary.SearchPinYin(txtSearch.Text, FullMatch);
            //按速度升序排序
            Array.Sort(result);
            Array.Reverse(result);
            RefreshList();
        }

        /// <summary>
        /// 刷新列表，在修改 result 变量之后调用
        /// </summary>
        private void RefreshList()
        {
            listView1.Items.Clear();
            listView1.BeginUpdate();
            int i = 0;
            foreach (var line in result)
            {
                ListViewItem item = listView1.Items.Add(i.ToString()); //序号

                item.SubItems.Add(Math.Round(line.Speed, 1).ToString()); //语速
                if(txtSearch.Text == "")
                    item.SubItems.Add(line.ContentPinYin); //拼音
                else
                    item.SubItems.Add(line.ContentPinYin.Replace(txtSearch.Text, string.Format("[{0}]", txtSearch.Text))); //拼音
                item.SubItems.Add(""); //标签
                i++;
            }
            listView1.EndUpdate();
        }

        //设置音源文件夹
        private void btnOpenOutPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = CurrentLibrary.VoiceBankPath;
            dialog.Description = "打开音源文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CurrentLibrary.VoiceBankPath = dialog.SelectedPath;
            }
            dialog.Dispose();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 打开浮动工具栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (formFloat.IsDisposed || formFloat == null)
            {
                formFloat = new FormFloat(this);
                formFloat.Show();
            }
            else
            {
                formFloat.Show();
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            Index = listView1.SelectedIndices[0];
            LoadCurrent();
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                //-i 输入文件 -ss 开始时间 -t 播放长度

                int selectedIndex = listView1.SelectedIndices[0];
                string args = string.Format("-i \"{0}\" -ss {1} -t {2} -autoexit", Path.ChangeExtension(result[selectedIndex].FilePath, ".mp3"), result[selectedIndex].StartTime.Replace(",", "."), result[selectedIndex].Duration);

                Process p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        Arguments = args,
                        FileName = "tools\\ffplay.exe",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                p.Start();
                p.Dispose();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                listView1_DoubleClick(null, null);
            }
        }

        private void 关于ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("UTAU 人力音源制作助手 v0.0.1\n本程序以 GPLv2 协议开源。", "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 新建素材库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string voice = "", audio = "";

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择音源文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                voice = dialog.SelectedPath;
            }
            else { return; }
            dialog.Dispose();

            if (File.Exists(Path.Combine(voice, Library.CONFIG_FILE_NAME)))
            {
                DialogResult r = MessageBox.Show("此文件夹下已有素材库配置文件，是否覆盖？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.No)
                    return;
            }

            FolderBrowserDialog dialog2 = new FolderBrowserDialog();
            dialog2.Description = "选择素材文件夹";
            if (dialog2.ShowDialog() == DialogResult.OK)
            {
                audio = dialog2.SelectedPath;
            }
            else { return; }
            dialog2.Dispose();

            Library lib = Library.Create(audio, voice);
            OpenLibrary(lib);
            lib.Save();
        }

        private void 打开素材库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "素材库配置文件(*.vbh)|*.vbh";
            dialog.Title = "请选择素材库配置文件（默认放在音源文件夹中）";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                OpenLibrary(dialog.FileName);
            }
        }


#endregion

        private void 自动切分ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("即将扫描所有音频文件并对其进行逐字标注，耗时可能较长，是否继续？\n原有的标注数据将被覆盖。", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AutoAligner.Align(CurrentLibrary.Audios.ToArray());
            }
        }

        private void 打开PraatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MFAHelper.RunPraat();
        }

        //刷新音源
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (UTAU == null)
                UTAU = UTAUController.GetInstance();
            if(UTAU != null)
                UTAU.RefreshVoiceBank();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (CurrentLibrary.VoiceBankPath == "")
                throw new IgnorableException("未打开任何素材库或无法找到 oto.ini 文件。");
            UTAUController.CleanVoiceBank(Path.Combine(CurrentLibrary.VoiceBankPath, "oto.ini"));
        }
    }
}
