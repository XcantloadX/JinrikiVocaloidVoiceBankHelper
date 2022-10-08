using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinrikiVocaloidVBHelper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (object sender, System.Threading.ThreadExceptionEventArgs e) => HandleException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => HandleException(e.ExceptionObject as Exception);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        private static void HandleException(Exception e)
        {
            if (e is IgnorableException)
            {
                IgnorableException ex = e as IgnorableException;
                MessageBox.Show(ex.Text, ex.Title, MessageBoxButtons.OK, ex.Icon);
            }
        }
    
    }
}
