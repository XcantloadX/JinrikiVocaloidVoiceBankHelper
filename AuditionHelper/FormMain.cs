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

namespace JinrikiVocaloidVBHelper
{
    public partial class FormMain : Form
    {
        private SearchHelper helper;
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
                if(_index < listFiles.Items.Count)
                    listFiles.SelectedIndex = value;
                lblIndex.Text = value.ToString();
            }
        }
        private string lastFile = "";
        /// <summary>
        /// 音源文件夹
        /// </summary>
        public string VoicePath
        {
            get { return lblOutPath.Text; }
            set { lblOutPath.Text = value; }
        }
        public string AudioPath
        {
            get { return lblAudioPath.Text; }
            set { lblAudioPath.Text = value; }
        }
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
        public const string LAST_AUDIO_PATH = "lastAudioPath";
        public const string LAST_VOICE_PATH = "lastVoicePath";
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
            conf.Write(LAST_AUDIO_PATH, AudioPath, LAST);
            conf.Write(LAST_VOICE_PATH, VoicePath, LAST);
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
            formFloat = new FormFloat(this);
            formFloat.Show();
            
            //载入上次数据
            AudioPath = conf.Read(LAST_AUDIO_PATH, LAST);
            Index = conf.Read2<int>(LAST_INDEX, LAST);
            VoicePath = conf.Read(LAST_VOICE_PATH, LAST);
            txtSearch.Text = conf.Read(LAST_SEARCH, LAST);
            Location = new Point(conf.Read2<int>(LAST_MAIN_X, LAST), conf.Read2<int>(LAST_MAIN_X, LAST));
            formFloat.Location = new Point(conf.Read2<int>(LAST_FLOAT_X, LAST), conf.Read2<int>(LAST_FLOAT_Y, LAST));
            AuditionKeyboardController.OpenFileWaitTimeFactor = conf.Read2<float>(ID_OpenFileWaitTimeFactor, SETTINGS);
            FullMatch = conf.Read2<bool>(LAST_MATCH_FULL_WORD);
            
            //初始化
            helper = new SearchHelper(AudioPath);
            AuditionController = new AuditionExtendScriptController();
            result = helper.SearchPinYin(txtSearch.Text, FullMatch);
            //按速度升序排序
            Array.Sort(result);
            Array.Reverse(result);
            RefreshList();
            
            btnSearch_Click(null, null);

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
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AudioPath = dialog.SelectedPath;
                lblAudioPath.Text = AudioPath;
                helper = new SearchHelper(AudioPath);
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
            AuditionKeyboardController.EnsureActived();
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
            DirectoryInfo dir = new DirectoryInfo(VoicePath);
            FileInfo[] files = dir.GetFiles();
            var voices =
                (from file in files
                 where Regex.IsMatch(file.Name, string.Format(@"{0}\d*.wav", voiceName)) && file.Name.EndsWith("wav")
                 orderby file.Name ascending
                 select file.Name).ToArray();
            return voiceName + (voices.Length == 0 ? "" : (voices.Length + 1).ToString());
        }

#region UI 事件
        //搜索
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Index = 0;
            result = helper.SearchPinYin(txtSearch.Text, FullMatch);
            //按速度升序排序
            Array.Sort(result);
            Array.Reverse(result);
            RefreshList();
        }

        private void RefreshList()
        {
            listFiles.Items.Clear();
            int i = 0;
            foreach (var line in result)
            {
                listFiles.Items.Add(i + "|" + Math.Round(line.Speed, 1) + " | " + line.ContentPinYin.Replace(txtSearch.Text, string.Format("[{0}]", txtSearch.Text)));
                i++;
            }
            
        }

        //设置音源文件夹
        private void btnOpenOutPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "打开音源文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                VoicePath = dialog.SelectedPath;
                lblOutPath.Text = VoicePath;
            }
            dialog.Dispose();
        }


        #endregion

        private void btnRecordWaitTimeFactor_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            MessageBox.Show("确定后将会自动打开一个文件，你需要在文件载入完成之后立即按下 Ctrl + Alt + N 以结束录制。");
            kbd.RegisterHotKey(Util.ModifierKeys.Control | Util.ModifierKeys.Alt, Keys.N);
            kbd.KeyPressed += (object sender2, KeyPressedEventArgs e2) =>
            {
                if(stopwatch.IsRunning && e2.Key == Keys.N && e2.Modifier == (Util.ModifierKeys.Control | Util.ModifierKeys.Alt))
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

        private void listFiles_DoubleClick(object sender, EventArgs e)
        {
            Index = listFiles.SelectedIndex;
            LoadCurrent();
        }

        private void listFiles_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                //-i 输入文件 -ss 开始时间 -t 播放长度

                string args = string.Format("-i \"{0}\" -ss {1} -t {2} -autoexit", Path.ChangeExtension(result[listFiles.SelectedIndex].FilePath, ".mp3"), result[listFiles.SelectedIndex].StartTime.Replace(",", "."), result[listFiles.SelectedIndex].Duration);

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
            else if(e.KeyChar == '\n')
            {
                listFiles_DoubleClick(null, null);
            }
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
            if(formFloat != null)
            {
                formFloat.Dispose();
                formFloat = new FormFloat(this);
                formFloat.Show();
            }
        }
    }
}
