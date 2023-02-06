using System;
using System.Windows.Forms;
using JinrikiVocaloidVBHelper.Core;
using System.IO;

namespace JinrikiVocaloidVBHelper.UI
{
    /// <summary>
    /// 媒体文件列表面板
    /// </summary>
    public partial class FilesPanel : Form
    {
        public Library Library { get; set; }

        public FilesPanel(Library lib)
        {
            Library = lib;
            InitializeComponent();
        }

        private void FilesPanel_Load(object sender, EventArgs e)
        {
            FillList();
        }

        /// <summary>
        /// 读入文件列表并填充
        /// </summary>
        public void FillList()
        {
            if (Library == null || Library.AudioPath == "")
                return;
            listViewFiles.BeginUpdate();

            listViewFiles.Items.Clear(); //先清空原有数据
            DirectoryInfo dir = new DirectoryInfo(Library.AudioPath);
            FileInfo[] files = dir.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension != ".mp3")
                    continue;
                //读入文件信息
                ListViewItem item = listViewFiles.Items.Add(file.Name); //
                item.SubItems.Add("");
            }

            listViewFiles.EndUpdate();
        }

        private void listViewFiles_MouseDown(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (e.Button == MouseButtons.Right && listView.Bounds.Contains(e.Location) && listView.FocusedItem != null)
            {
                listView.ContextMenuStrip = contextMenuStrip1;
            }
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process fileopener = new System.Diagnostics.Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + Path.Combine(Library.AudioPath, listViewFiles.FocusedItem.Text) + "\"";
            fileopener.Start();
        }

        private void 在资源管理器中显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process fileopener = new System.Diagnostics.Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "/select,\"" + Path.Combine(Library.AudioPath, listViewFiles.FocusedItem.Text) + "\"";
            fileopener.Start();
        }
    }
}
