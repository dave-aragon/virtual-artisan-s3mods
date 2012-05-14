using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace STBLBrowser
{
    public partial class FindById : Form
    {
        public FindById()
        {
            InitializeComponent();
            idInput1.IsValidChanged += new EventHandler(ValidChanged);
            button1.Enabled = idInput1.IsKeyValid;
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

        public UInt64 KeyID
        {
            get
            {
                return idInput1.KeyID;
            }
        }

        private void ValidChanged(object sender, EventArgs evt)
        {
            button1.Enabled = idInput1.IsKeyValid;
        }
    }
}
