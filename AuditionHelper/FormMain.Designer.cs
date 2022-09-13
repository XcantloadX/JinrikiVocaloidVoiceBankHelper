namespace JinrikiVocaloidVBHelper
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("1110");
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxMatchFullWord = new System.Windows.Forms.CheckBox();
            this.lblWaitTimeFactor = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblOutPath = new System.Windows.Forms.Label();
            this.btnOpenOutPath = new System.Windows.Forms.Button();
            this.lblAudioPath = new System.Windows.Forms.Label();
            this.lblIndex = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnOpenAudio = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建素材库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开素材库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.剪切剪映字幕ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开浮动工具栏ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.选项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.访问官网ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSpeed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxMatchFullWord);
            this.panel1.Controls.Add(this.lblWaitTimeFactor);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblOutPath);
            this.panel1.Controls.Add(this.btnOpenOutPath);
            this.panel1.Controls.Add(this.lblAudioPath);
            this.panel1.Controls.Add(this.lblIndex);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtSearch);
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Controls.Add(this.btnOpenAudio);
            this.panel1.Location = new System.Drawing.Point(6, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(815, 164);
            this.panel1.TabIndex = 0;
            // 
            // checkBoxMatchFullWord
            // 
            this.checkBoxMatchFullWord.AutoSize = true;
            this.checkBoxMatchFullWord.Location = new System.Drawing.Point(12, 125);
            this.checkBoxMatchFullWord.Name = "checkBoxMatchFullWord";
            this.checkBoxMatchFullWord.Size = new System.Drawing.Size(89, 19);
            this.checkBoxMatchFullWord.TabIndex = 11;
            this.checkBoxMatchFullWord.Text = "全词匹配";
            this.checkBoxMatchFullWord.UseVisualStyleBackColor = true;
            // 
            // lblWaitTimeFactor
            // 
            this.lblWaitTimeFactor.AutoSize = true;
            this.lblWaitTimeFactor.Location = new System.Drawing.Point(568, 81);
            this.lblWaitTimeFactor.Name = "lblWaitTimeFactor";
            this.lblWaitTimeFactor.Size = new System.Drawing.Size(15, 15);
            this.lblWaitTimeFactor.TabIndex = 10;
            this.lblWaitTimeFactor.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(465, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "等待时间因子";
            // 
            // lblOutPath
            // 
            this.lblOutPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOutPath.AutoSize = true;
            this.lblOutPath.Location = new System.Drawing.Point(143, 50);
            this.lblOutPath.Name = "lblOutPath";
            this.lblOutPath.Size = new System.Drawing.Size(55, 15);
            this.lblOutPath.TabIndex = 8;
            this.lblOutPath.Text = "label4";
            // 
            // btnOpenOutPath
            // 
            this.btnOpenOutPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnOpenOutPath.Location = new System.Drawing.Point(12, 44);
            this.btnOpenOutPath.Name = "btnOpenOutPath";
            this.btnOpenOutPath.Size = new System.Drawing.Size(125, 26);
            this.btnOpenOutPath.TabIndex = 7;
            this.btnOpenOutPath.Text = "设置音源文件夹";
            this.btnOpenOutPath.UseVisualStyleBackColor = true;
            this.btnOpenOutPath.Click += new System.EventHandler(this.btnOpenOutPath_Click);
            // 
            // lblAudioPath
            // 
            this.lblAudioPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAudioPath.AutoSize = true;
            this.lblAudioPath.Location = new System.Drawing.Point(143, 18);
            this.lblAudioPath.Name = "lblAudioPath";
            this.lblAudioPath.Size = new System.Drawing.Size(55, 15);
            this.lblAudioPath.TabIndex = 6;
            this.lblAudioPath.Text = "label3";
            // 
            // lblIndex
            // 
            this.lblIndex.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblIndex.AutoSize = true;
            this.lblIndex.Location = new System.Drawing.Point(384, 81);
            this.lblIndex.Name = "lblIndex";
            this.lblIndex.Size = new System.Drawing.Size(23, 15);
            this.lblIndex.TabIndex = 5;
            this.lblIndex.Text = "-1";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(311, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "当前下标";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(12, 75);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(197, 25);
            this.txtSearch.TabIndex = 3;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSearch.Location = new System.Drawing.Point(232, 75);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(64, 26);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "搜索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnOpenAudio
            // 
            this.btnOpenAudio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnOpenAudio.Location = new System.Drawing.Point(12, 12);
            this.btnOpenAudio.Name = "btnOpenAudio";
            this.btnOpenAudio.Size = new System.Drawing.Size(125, 26);
            this.btnOpenAudio.TabIndex = 0;
            this.btnOpenAudio.Text = "设置音频文件夹";
            this.btnOpenAudio.UseVisualStyleBackColor = true;
            this.btnOpenAudio.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.工具ToolStripMenuItem,
            this.关于ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(849, 28);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建素材库ToolStripMenuItem,
            this.打开素材库ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 新建素材库ToolStripMenuItem
            // 
            this.新建素材库ToolStripMenuItem.Name = "新建素材库ToolStripMenuItem";
            this.新建素材库ToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.新建素材库ToolStripMenuItem.Text = "新建素材库...";
            // 
            // 打开素材库ToolStripMenuItem
            // 
            this.打开素材库ToolStripMenuItem.Name = "打开素材库ToolStripMenuItem";
            this.打开素材库ToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.打开素材库ToolStripMenuItem.Text = "打开素材库...";
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // 工具ToolStripMenuItem
            // 
            this.工具ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.剪切剪映字幕ToolStripMenuItem,
            this.打开浮动工具栏ToolStripMenuItem,
            this.toolStripSeparator1,
            this.选项ToolStripMenuItem});
            this.工具ToolStripMenuItem.Name = "工具ToolStripMenuItem";
            this.工具ToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.工具ToolStripMenuItem.Text = "工具";
            // 
            // 剪切剪映字幕ToolStripMenuItem
            // 
            this.剪切剪映字幕ToolStripMenuItem.Name = "剪切剪映字幕ToolStripMenuItem";
            this.剪切剪映字幕ToolStripMenuItem.Size = new System.Drawing.Size(189, 26);
            this.剪切剪映字幕ToolStripMenuItem.Text = "剪映字幕处理";
            // 
            // 打开浮动工具栏ToolStripMenuItem
            // 
            this.打开浮动工具栏ToolStripMenuItem.Name = "打开浮动工具栏ToolStripMenuItem";
            this.打开浮动工具栏ToolStripMenuItem.Size = new System.Drawing.Size(189, 26);
            this.打开浮动工具栏ToolStripMenuItem.Text = "打开浮动工具栏";
            this.打开浮动工具栏ToolStripMenuItem.Click += new System.EventHandler(this.打开浮动工具栏ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(186, 6);
            // 
            // 选项ToolStripMenuItem
            // 
            this.选项ToolStripMenuItem.Name = "选项ToolStripMenuItem";
            this.选项ToolStripMenuItem.Size = new System.Drawing.Size(189, 26);
            this.选项ToolStripMenuItem.Text = "选项...";
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.访问官网ToolStripMenuItem,
            this.关于ToolStripMenuItem1});
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.关于ToolStripMenuItem.Text = "帮助";
            // 
            // 访问官网ToolStripMenuItem
            // 
            this.访问官网ToolStripMenuItem.Name = "访问官网ToolStripMenuItem";
            this.访问官网ToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.访问官网ToolStripMenuItem.Text = "访问官网";
            // 
            // 关于ToolStripMenuItem1
            // 
            this.关于ToolStripMenuItem1.Name = "关于ToolStripMenuItem1";
            this.关于ToolStripMenuItem1.Size = new System.Drawing.Size(181, 26);
            this.关于ToolStripMenuItem1.Text = "关于";
            this.关于ToolStripMenuItem1.Click += new System.EventHandler(this.关于ToolStripMenuItem1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 31);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(837, 601);
            this.tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listView1);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(829, 572);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "搜索";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderIndex,
            this.columnHeaderSpeed,
            this.columnHeaderName,
            this.columnHeaderTag});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.listView1.Location = new System.Drawing.Point(6, 176);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(817, 390);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.listView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyUp);
            // 
            // columnHeaderIndex
            // 
            this.columnHeaderIndex.Text = "序号";
            this.columnHeaderIndex.Width = 40;
            // 
            // columnHeaderSpeed
            // 
            this.columnHeaderSpeed.Text = "语速";
            this.columnHeaderSpeed.Width = 50;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "拼音";
            this.columnHeaderName.Width = 400;
            // 
            // columnHeaderTag
            // 
            this.columnHeaderTag.Text = "标签";
            this.columnHeaderTag.Width = 150;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(829, 572);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "编辑";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 637);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabControl1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "UATU 音源制作助手";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOpenAudio;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblOutPath;
        private System.Windows.Forms.Button btnOpenOutPath;
        private System.Windows.Forms.Label lblAudioPath;
        private System.Windows.Forms.Label lblIndex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblWaitTimeFactor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxMatchFullWord;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新建素材库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开素材库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 剪切剪映字幕ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开浮动工具栏ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem 选项ToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStripMenuItem 访问官网ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderIndex;
        private System.Windows.Forms.ColumnHeader columnHeaderSpeed;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderTag;
    }
}

