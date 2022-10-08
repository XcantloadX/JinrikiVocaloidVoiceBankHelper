using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System
{
    public class IgnorableException : Exception
    {
        public MessageBoxIcon Icon { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public IgnorableException(string message) : base(message)
        {
            Icon = MessageBoxIcon.Error;
            Title = "错误";
            Text = message;
        }

        public IgnorableException(string title, string text, MessageBoxIcon icon)
        {
            Icon = icon;
            Title = title;
            Text = text;
        }
    }
}
