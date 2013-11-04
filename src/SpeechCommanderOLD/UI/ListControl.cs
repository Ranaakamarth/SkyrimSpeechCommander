using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpeechCommander.UI
{
    public partial class ListControl : UserControl
    {
        public BindingSource Source;

        public ListControl()
        {
            InitializeComponent();
        }

        private void Add_bttn_Click(object sender, EventArgs e)
        {
            string text = this.InputField_tb.Text.Trim();
            if (text.Length > 0)
            {
                //if (!this.Datasource.Any(item => item.ToString() == text))
                //{
                //    this.Datasource.Add(text
                //}
            }
        }
    }
}
