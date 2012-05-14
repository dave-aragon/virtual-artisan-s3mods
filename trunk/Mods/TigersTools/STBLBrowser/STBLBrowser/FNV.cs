using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace STBLBrowser
{
    public partial class FNV : Form
    {
        public FNV()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UInt64 hash64 = STBLVault.FNV64(textBox1.Text);
            UInt32 hash32 = STBLVault.FNV32(textBox1.Text);
            UInt32 hash24 = STBLVault.FNV24(textBox1.Text);

            val24.Text = hash24.ToString("X6");
            val32.Text = hash32.ToString("X8");
            val64.Text = hash64.ToString("X16");

            byte[] bytes64 = BitConverter.GetBytes(hash64);
            byte[] bytes32 = BitConverter.GetBytes(hash32);
            byte[] bytes24 = BitConverter.GetBytes(hash24);

            val64Byte.Text = string.Format("{0:X2} {1:X2} {2:X2} {3:X2} {4:X2} {5:X2} {6:X2} {7:X2}", bytes64[0], bytes64[1], bytes64[2], bytes64[3], bytes64[4], bytes64[5], bytes64[6], bytes64[7]);
            val32Byte.Text = string.Format("{0:X2} {1:X2} {2:X2} {3:X2}", bytes32[0], bytes32[1], bytes32[2], bytes32[3]);
            val24Byte.Text = string.Format("{0:X2} {1:X2} {2:X2}", bytes24[0], bytes24[1], bytes24[2]);
        }
    }
}
