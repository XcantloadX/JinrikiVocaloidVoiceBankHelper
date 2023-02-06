using JinrikiVocaloidVBHelper.Audition;
using JinrikiVocaloidVBHelper.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinrikiVocaloidVBHelper
{
    public partial class FormFloat : Form
    {
        private FormMain mainForm;
        private string originalTitle;
        private AuditionController controller;

        public FormFloat(FormMain mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            originalTitle = Text;
            controller = mainForm.AuditionController;
        }

        private void btnSaveSelection_Click(object sender, EventArgs e)
        {
            string fileName = mainForm.NextVoiceFileName;
            controller.SaveSelection(fileName, mainForm.CurrentLibrary.VoiceBankPath);
            UIHelper.ShowBalloon("已保存文件", fileName);
        }

        private void btnSaveSelectionAs_Click(object sender, EventArgs e)
        {
            string name = "";
            if(UIHelper.ShowInputDialog(ref name, "请输入发音名（不带 .wav，不带数字序号）") == DialogResult.OK)
            {
                string fileName = mainForm.GetNextVoiceFileName(name);
                controller.SaveSelection(fileName, mainForm.CurrentLibrary.VoiceBankPath);
                UIHelper.ShowBalloon("已保存文件", fileName);
            }

        }

        public void UpdateUI()
        {
            Text = string.Format("{0}  正在编辑\"{1}\" 下标：{2}", originalTitle, mainForm.CurrentVoiceName, mainForm.Index);
            lblContent.Text = mainForm.CurrentSubtitle.ContentPinYin;
            lblFile.Text = System.IO.Path.GetFileName(mainForm.CurrentSubtitle.FilePath);
        }

        private void FormFloat_Load(object sender, EventArgs e)
        {

        }

        private void timerTopWindowDetector_Tick(object sender, EventArgs e)
        {
            Visible = AuditionKeyboardController.IsAuditionActive;
        }
    }
}
