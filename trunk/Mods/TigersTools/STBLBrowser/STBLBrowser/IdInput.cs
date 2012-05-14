using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace STBLBrowser
{
    public delegate string KeyNameNeededHandler(UInt64 key);
    public partial class IdInput : UserControl
    {
        public IdInput()
        {
            InitializeComponent();
            byID.Checked = true;
        }

        public UInt64 mCurKey = 0;
        public bool mIsValid = false;

        public event KeyNameNeededHandler KeyNameNeeded;
        [Browsable(false)]
        [DefaultValue((UInt64)0)]
        public UInt64 KeyID
        {
            get
            {
                UInt64 ret;
                if (!TryParseId(out ret))
                    return 0;
                return ret;
            }
            set
            {
                textBox2.Text = string.Format("0x{0:X15}", value);
                byID.Checked = true;
                ResolveName();
            }
        }
        [Browsable(false)]
        [DefaultValue(null)]
        public string KeyName
        {
            get
            {
                if (byName.Checked && !string.IsNullOrEmpty(textBox1.Text))
                    return textBox1.Text;
                else if (!string.IsNullOrEmpty(textBox1.Text) && STBLVault.FNV64(textBox1.Text) == KeyID)
                    return textBox1.Text;
                else
                    return null;
            }
            set
            {
                textBox1.Text = value;
                if (!string.IsNullOrEmpty(value))
                {
                    UInt64 newval = STBLVault.FNV64(value);
                    textBox2.Text = string.Format("0x{0:X16}", newval);
                    SetKey(newval);
                    SetValid(true);
                }
                else
                    SetValid(false);

                byName.Checked = true;
            }
        }

        [Browsable(false)]
        public bool IsKeyValid
        {
            get
            {
                if (byName.Checked && string.IsNullOrEmpty(textBox1.Text))
                    return false;

                UInt64 ignore;
                return TryParseId(out ignore);
            }
        }

        private bool TryParseId(out UInt64 val)
        {
            if (textBox2.Text.ToLower().StartsWith("0x"))
                return UInt64.TryParse(textBox2.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out val);
            else
                return UInt64.TryParse(textBox2.Text, out val);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!byName.Checked)
                return;
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                UInt64 newkeyval = STBLVault.FNV64(textBox1.Text);
                textBox2.Text = string.Format("0x{0:X16}", newkeyval);
                SetValid(true);
                SetKey(newkeyval);
            }
            else
                SetValid(false);
        }

        private void byName_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.ReadOnly = !byName.Checked;
            textBox2.ReadOnly = !byID.Checked;
            SetValid(IsKeyValid);
            if (mIsValid)
                SetKey(KeyID);
            if (byName.Checked)
                textBox1.Select();
            else
                textBox2.Select();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = char.ToLower(e.KeyChar);
            bool hexok;

            if (ch == 'v' - 'a' + 1)
            {
                e.Handled = true;

                IDataObject dobj = Clipboard.GetDataObject() as IDataObject;
                string pastedata = dobj.GetData(typeof(string)) as string;

                if (string.IsNullOrEmpty(pastedata))
                    return;

                string text = string.Empty;
                if (textBox2.SelectionStart > 0)
                    text += textBox2.Text.Substring(0, textBox2.SelectionStart);
                text += pastedata;
                if (textBox2.SelectionStart + textBox2.SelectionLength < textBox2.Text.Length)
                    text += textBox2.Text.Substring(textBox2.SelectionStart + textBox2.SelectionLength);

                string lowertext = text.ToLower();
                int idx = 0;
                if (lowertext.StartsWith("0x"))
                {
                    hexok = true;
                    idx = 2;
                }
                else
                    hexok = false;

                while (idx < lowertext.Length)
                {
                    if (lowertext[idx] < '0' || lowertext[idx] > '9')
                    {
                        if (!hexok || lowertext[idx] < 'a' || lowertext[idx] > 'f')
                        {
                            return;
                        }
                    }
                    idx++;
                }

                int start = textBox2.SelectionStart;
                textBox2.Text = text;
                textBox2.SelectionStart = start + pastedata.Length;
                textBox2.SelectionLength = 0;
                
                // Paste
                return;
            }

            if (char.IsControl(ch))
                return;

            hexok = textBox2.Text.ToLower().StartsWith("0x");

            if (hexok && textBox1.SelectionStart < 2)
            {
                if (textBox2.SelectionStart + textBox2.SelectionLength < 2)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (ch >= '0' && ch <= '9')
                return;

            if (ch == 'x' && textBox2.SelectionStart == 1 && textBox2.Text.StartsWith("0") && !textBox2.Text.ToLower().StartsWith("0x"))
                return;

            if (!hexok || ch < 'a' || ch > 'f')
            {
                e.Handled = true;
                return;
            }
        }

        void ResolveName()
        {
            UInt64 value;
            string newname = string.Empty;
            if (KeyNameNeeded != null && TryParseId(out value))
            {
                foreach (KeyNameNeededHandler handler in KeyNameNeeded.GetInvocationList())
                {
                    string ret = handler(value);
                    if (ret != null)
                    {
                        newname = ret;
                        break;
                    }
                }
            }
            textBox1.Text = newname;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!byID.Checked)
                return;
            ResolveName();
            if (IsKeyValid)
            {
                SetValid(true);
                SetKey(KeyID);
            }
            else
                SetValid(false);
        }

        public event EventHandler IsValidChanged;
        public event EventHandler KeyChanged;

        private void SetValid(bool valid)
        {
            if (mIsValid == valid)
                return;
            mIsValid = valid;
            if (IsValidChanged != null)
                IsValidChanged(this, EventArgs.Empty);
        }

        private void SetKey(UInt64 key)
        {
            if (mCurKey == key)
                return;
            mCurKey = key;
            if (KeyChanged != null)
                KeyChanged(this, EventArgs.Empty);
        }
    }
}
