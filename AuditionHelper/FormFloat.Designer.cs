namespace JinrikiVocaloidVBHelper
{
    partial class FormFloat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblFile = new System.Windows.Forms.Label();
            this.lblContent = new System.Windows.Forms.Label();
            this.btnSaveSelection = new System.Windows.Forms.Button();
            this.btnSaveSelectionAs = new System.Windows.Forms.Button();
            this.timerTopWindowDetector = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lblFile
            // 
            this.lblFile.Location = new System.Drawing.Point(12, 9);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(927, 23);
            this.lblFile.TabIndex = 0;
            this.lblFile.Text = "未打开文件";
            // 
            // lblContent
            // 
            this.lblContent.Location = new System.Drawing.Point(12, 42);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(505, 23);
            this.lblContent.TabIndex = 1;
            this.lblContent.Text = "(空)";
            // 
            // btnSaveSelection
            // 
            this.btnSaveSelection.Location = new System.Drawing.Point(523, 38);
            this.btnSaveSelection.Name = "btnSaveSelection";
            this.btnSaveSelection.Size = new System.Drawing.Size(92, 23);
            this.btnSaveSelection.TabIndex = 2;
            this.btnSaveSelection.Text = "保存选区";
            this.btnSaveSelection.UseVisualStyleBackColor = true;
            this.btnSaveSelection.Click += new System.EventHandler(this.btnSaveSelection_Click);
            // 
            // btnSaveSelectionAs
            // 
            this.btnSaveSelectionAs.Location = new System.Drawing.Point(621, 38);
            this.btnSaveSelectionAs.Name = "btnSaveSelectionAs";
            this.btnSaveSelectionAs.Size = new System.Drawing.Size(115, 23);
            this.btnSaveSelectionAs.TabIndex = 3;
            this.btnSaveSelectionAs.Text = "保存选区为...";
            this.btnSaveSelectionAs.UseVisualStyleBackColor = true;
            this.btnSaveSelectionAs.Click += new System.EventHandler(this.btnSaveSelectionAs_Click);
            // 
            // timerTopWindowDetector
            // 
            this.timerTopWindowDetector.Enabled = true;
            this.timerTopWindowDetector.Tick += new System.EventHandler(this.timerTopWindowDetector_Tick);
            // 
            // FormFloat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 74);
            this.Controls.Add(this.btnSaveSelectionAs);
            this.Controls.Add(this.btnSaveSelection);
            this.Controls.Add(this.lblContent);
            this.Controls.Add(this.lblFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormFloat";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "音源制作 工具栏";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormFloat_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.Button btnSaveSelection;
        private System.Windows.Forms.Button btnSaveSelectionAs;
        private System.Windows.Forms.Timer timerTopWindowDetector;
    }
}