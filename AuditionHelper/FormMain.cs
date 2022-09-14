using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using System.Diagnostics;
using System.Text.RegularExpressions;
using JinrikiVocaloidVBHelper.Util;
using JinrikiVocaloidVBHelper.Core;
using AuditionHelper.Core;
using JinrikiVocaloidVBHelper.Audition;
using AuditionHelper.Audition;

namespace JinrikiVocaloidVBHelper
{
    public partial class FormMain : Form
    {
        private KeyboardHook kbd = new KeyboardHook();
        private InputSimulator simulator = new InputSimulator();
        private SrtLine[] result = null;
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
        public MaterialLibrary CurrentLibrary { get; set; }
        private FormFloat formFloat;
        /// <summary>
        /// 当前正在编辑的音源名称
        /// </summary>
        public string CurrentVoiceName
        {
            get { return txtSearch.Text; }
        }
        public SrtLine CurrentSrtLine { get { return result[Index]; } }
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
        public bool FullMatch
        {
            get { return checkBoxMatchFullWord.Checked; }
            set { checkBoxMatchFullWord.Checked = value; }
        }

        public AuditionController AuditionController { get; private set; }

        public const string LAST = "LAST";
        public const string SETTINGS = "Settings";
        public const string LAST_LIB = "lastLibrary";
        public const string LAST_SEARCH = "lastSearch";
        public const string LAST_INDEX = "lastIndex";
        public const string LAST_MATCH_FULL_WORD = "lastMatchFullWord";
        public const string LAST_MAIN_X = "mainX";
        public const string LAST_MAIN_Y = "mainY";
        public const string LAST_FLOAT_X = "floatX";
        public const string LAST_FLOAT_Y = "floatX";
        public const string ID_OpenFileWaitTimeFactor = "OpenFileWaitTimeFactor";

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            AuditionController.Dispose();

            //保存数据
            conf.Write(LAST_LIB, CurrentLibrary.ConfigPath, LAST);
            conf.Write(LAST_SEARCH, txtSearch.Text, LAST);
            conf.Write(LAST_INDEX, Index.ToString(), LAST);
            conf.Write(LAST_MAIN_X, Location.X.ToString(), LAST);
            conf.Write(LAST_MAIN_Y, Location.Y.ToString(), LAST);
            conf.Write(LAST_FLOAT_X, formFloat.Location.X.ToString(), LAST);
            conf.Write(LAST_FLOAT_Y, formFloat.Location.Y.ToString(), LAST);
            conf.Write(LAST_MATCH_FULL_WORD, (checkBoxMatchFullWord.Checked).ToString(), LAST);

            conf.Write(ID_OpenFileWaitTimeFactor, AuditionKeyboardController.OpenFileWaitTimeFactor.ToString(), SETTINGS);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

#if DEBUG
            VisualStudioDebugHelper.InstallExtesion();
#endif


            //载入上次数据
            CurrentLibrary = MaterialLibrary.Read(conf.Read(LAST_LIB, LAST));
            Index = conf.Read2<int>(LAST_INDEX, LAST);
            txtSearch.Text = conf.Read(LAST_SEARCH, LAST);
            Location = new Point(conf.Read2<int>(LAST_MAIN_X, LAST), conf.Read2<int>(LAST_MAIN_X, LAST));
            
            AuditionKeyboardController.OpenFileWaitTimeFactor = conf.Read2<float>(ID_OpenFileWaitTimeFactor, SETTINGS);
            FullMatch = conf.Read2<bool>(LAST_MATCH_FULL_WORD);
            
            //初始化
            AuditionController = new AuditionExtendScriptController();
            if(CurrentLibrary.SearchHelper != null)
            {
                result = CurrentLibrary.SearchHelper.SearchPinYin(txtSearch.Text, FullMatch);
                //按速度升序排序
                Array.Sort(result);
                Array.Reverse(result);
                RefreshList();
                btnSearch_Click(null, null);
            }

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
            //Ctrl + F 标记当前选区
            //kbd.RegisterHotKey(AuditionHelper.ModifierKeys.Control, Keys.F); 

            kbd.KeyPressed += Kbd_KeyPressed;

            formFloat = new FormFloat(this);
            formFloat.Show();
            formFloat.Location = new Point(conf.Read2<int>(LAST_FLOAT_X, LAST), conf.Read2<int>(LAST_FLOAT_Y, LAST));
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

        }   

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "打开源音频文件夹";
            dialog.SelectedPath = CurrentLibrary.AudioPath;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CurrentLibrary.AudioPath = dialog.SelectedPath;
                lblAudioPath.Text = CurrentLibrary.AudioPath;
            }
            dialog.Dispose();
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

        /// <summary>
        /// 取指定发音名的下一个文件名。比如现在有 ba.wav ba1.wav，那么传入 ba 则会返回 ba2.wav
        /// </summary>
        /// <param name="voiceName">发音名</param>
        /// <returns></returns>
        public string GetNextVoiceFileName(string voiceName)
        {
            DirectoryInfo dir = new DirectoryInfo(CurrentLibrary.VoicePath);
            FileInfo[] files = dir.GetFiles();
            var voices =
                (from file in files
                 where Regex.IsMatch(file.Name, string.Format(@"{0}\d*.wav", voiceName)) && file.Name.EndsWith("wav")
                 orderby file.Name ascending
                 select file.Name).ToArray();
            return voiceName + (voices.Length == 0 ? "" : (voices.Length + 1).ToString());
        }


        #region 操作方法

        /// <summary>
        /// 打开指定的素材库
        /// </summary>
        /// <param name="lib"></param>
        public void OpenLibrary(MaterialLibrary lib)
        {
            CurrentLibrary = lib;
            btnSearch_Click(null, null);
            if(string.IsNullOrEmpty(CurrentLibrary.Name))
                this.Text = new DirectoryInfo(CurrentLibrary.VoicePath).Name + " - UATU 音源制作助手";
            else
                this.Text = CurrentLibrary.Name + " - UATU 音源制作助手";
        }

        public void OpenLibrary(string libConfigPath)
        {
            OpenLibrary(MaterialLibrary.Read(libConfigPath));
        }

        #endregion


        #region UI 事件
        //搜索
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Index = 0;
            if(CurrentLibrary == null || CurrentLibrary.SearchHelper == null)
            {
                MessageBox.Show("未打开任何素材库或素材库无效！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            result = CurrentLibrary.SearchHelper.SearchPinYin(txtSearch.Text, FullMatch);
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
            dialog.SelectedPath = CurrentLibrary.VoicePath;
            dialog.Description = "打开音源文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CurrentLibrary.VoicePath = dialog.SelectedPath;
                lblOutPath.Text = CurrentLibrary.VoicePath;
            }
            dialog.Dispose();
        }

        private void btnRecordWaitTimeFactor_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            MessageBox.Show("确定后将会自动打开一个文件，你需要在文件载入完成之后立即按下 Ctrl + Alt + N 以结束录制。");
            kbd.RegisterHotKey(Util.ModifierKeys.Control | Util.ModifierKeys.Alt, Keys.N);
            kbd.KeyPressed += (object sender2, KeyPressedEventArgs e2) =>
            {
                if (stopwatch.IsRunning && e2.Key == Keys.N && e2.Modifier == (Util.ModifierKeys.Control | Util.ModifierKeys.Alt))
                {
                    stopwatch.Stop();
                    AuditionKeyboardController.OpenFileWaitTimeFactor = new FileInfo(result[0].FilePath).Length / stopwatch.Elapsed.TotalSeconds;
                    UIHelper.ShowBalloon("音源辅助工具", "等待时间因子已保存：" + AuditionKeyboardController.OpenFileWaitTimeFactor);
                    stopwatch.Reset();
                }
            };

            stopwatch.Start();
            OpenFile(Path.ChangeExtension(result[0].FilePath, ".mp3"), true);
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
            if (formFloat != null)
            {
                formFloat.Dispose();
                formFloat = new FormFloat(this);
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

                //上面代码的等价写法
                //Process process = new Process();
                //process.StartInfo.Arguments = args;
                //process.StartInfo.FileName = "ffplay.exe";
                //process.StartInfo.CreateNoWindow = true;
                //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //process.Start();
                //process.Dispose();
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

            if (File.Exists(Path.Combine(voice, MaterialLibrary.CONFIG_FILE_NAME)))
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

            MaterialLibrary lib = MaterialLibrary.Create(audio, voice);
            OpenLibrary(lib);
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
            if(MessageBox.Show("即将扫描所有音频文件并对其进行逐字切分，耗时可能较长，是否继续？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CurrentLibrary.AutoAlign();
            }
        }

        private void 打开PraatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MFAHelper.RunPraat();
        }
    }
}
