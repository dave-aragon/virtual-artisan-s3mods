using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace STBLBrowser
{
    public partial class NewString : Form
    {
        public NewString()
        {
            InitializeComponent();
            idInput1.KeyName = string.Empty;
        }

        public event KeyNameNeededHandler KeyNameNeeded
        {
            add
            {
                idInput1.KeyNameNeeded += value;
            }
            remove
            {
                idInput1.KeyNameNeeded -= value;
            }
        }

        public string Value
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        public string KeyName
        {
            get
            {
                return idInput1.KeyName;
            }
            set
            {
                idInput1.KeyName = value;
            }
        }

        public UInt64 KeyId
        {
            get
            {
                return idInput1.KeyID;
            }
            set
            {
                idInput1.KeyID = value;
            }
        }
    }
}
