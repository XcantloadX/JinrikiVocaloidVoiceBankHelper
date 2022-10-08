using JinrikiVocaloidVBHelper.Automation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinrikiVocaloidVBHelper
{
    public partial class DialogChooseUTAU : Form
    {
        private int selectedIndex = -1;
        private static Process defaultProcess = null;

        public DialogChooseUTAU()
        {
            InitializeComponent();
        }

        private void DialogChooseUTAU_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = SystemIcons.Question.ToBitmap();
        }

        public static Process ShowBox(Process[] processes)
        {
            //判断默认选项
            if (defaultProcess != null && !defaultProcess.HasExited)
            {
                return defaultProcess;
            }

            DialogChooseUTAU dialog = new DialogChooseUTAU();
            processes.ToList().ForEach(process => dialog.listBox1.Items.Add(new UTAUController(process.Id).ProjectName));
            if(processes.Length > 0)
                dialog.listBox1.SelectedIndex = 0; //默认选中第一个
            dialog.ShowDialog();

            //处理默认
            if (dialog.checkBoxAsDefault.Checked)
            {
                defaultProcess = processes[dialog.selectedIndex];
            }
            return processes[dialog.selectedIndex];
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            selectedIndex = listBox1.SelectedIndex;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
