using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace STBLBrowser.ImportExport
{
    internal partial class ExportCSV : Form, IExport
    {
        public ExportCSV()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = !checkBox1.Checked;
        }

        private STBLVault mData;
        private string mFilename;

        public STBLVault STBLData
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;
                comboBox1.Items.Clear();
                for (int loop = 0; loop < (int)Languages.Language_Count; loop++)
                {
                    if (mData.HasEntriesForLanguage((Languages)loop, STBLVault.StringSource.Loaded | STBLVault.StringSource.Modified))
                    {
                        comboBox1.Items.Add((Languages)loop);
                    }
                }
                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
        }

        public bool Export(IWin32Window owner, string filename)
        {
            if (comboBox1.Items.Count == 0)
            {
                MessageBox.Show(owner, "No loaded or modified data to export", "No data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (comboBox1.Items.Count == 1)
            {
                checkBox1.Checked = false;
                checkBox1.Enabled = false;
            }
            else
                checkBox1.Enabled = true;

            if (ShowDialog(owner) != DialogResult.OK)
                return false;

            try
            {
                if (checkBox1.Checked)
                {
                    foreach (Languages lang in comboBox1.Items)
                    {
                        string langfilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), string.Format("{0}_{1}{2}", System.IO.Path.GetFileNameWithoutExtension(filename), Locales.Names[(int)lang], System.IO.Path.GetExtension(filename)));
                        Export(langfilename, lang);
                    }
                }
                else
                {
                    Export(filename, (Languages)comboBox1.Items[comboBox1.SelectedIndex]);
                }
                mFilename = filename;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, string.Format("Error exporting data:\n{0}", ex.Message), "Write Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        public void Export(IWin32Window owner)
        {
            if (checkBox1.Checked)
            {
                foreach (Languages lang in comboBox1.Items)
                {
                    string langfilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(mFilename), string.Format("{0}_{1}{2}", System.IO.Path.GetFileNameWithoutExtension(mFilename), Locales.Names[(int)lang], System.IO.Path.GetExtension(mFilename)));
                    Export(langfilename, lang);
                }
            }
            else
            {
                Export(mFilename, (Languages)comboBox1.Items[comboBox1.SelectedIndex]);
            }
        }

        public void Export(string filename, Languages lang)
        {
            StringBuilder line = new StringBuilder();
            System.IO.StreamWriter writer = new System.IO.StreamWriter(filename, false, Encoding.UTF8);
            bool quotestrings = quoteStrings.Checked;
            bool quoteall = quoteAll.Checked;
            bool nonames = idNumber.Checked;
            bool barehex = hexNums.Checked;
            char seperator;
            if (tabDelim.Checked || textBox2.Text.Length < 1)
                seperator = '\t';
            else
                seperator = textBox2.Text[0];
            using (writer)
            {
                foreach (KeyValuePair<UInt64, string> entry in mData.GetEntriesForLanguage(lang, STBLVault.StringSource.Loaded | STBLVault.StringSource.Modified))
                {
                    bool keydone = false;
                    string keyname;
                    if (!nonames && STBLVault.LookupKey(entry.Key, out keyname))
                    {
                        if (keyname.IndexOf(seperator) < 0 && keyname.IndexOf('\n') < 0)
                        {
                            if (quotestrings)
                            {
                                writer.Write('"');
                                QuoteString(writer, keyname);
                                writer.Write('"');
                            }
                            else
                            {
                                writer.Write(keyname);
                            }
                            keydone = true;
                        }
                    }
                    if (!keydone)
                    {
                        if (quoteall)
                            writer.Write('"');
                        if (!barehex)
                            writer.Write("0x");
                        writer.Write(entry.Key.ToString("X16"));
                        if (quoteall)
                            writer.Write('"');
                    }
                    writer.Write(seperator);
                    if (quotestrings)
                    {
                        writer.Write('"');
                        QuoteString(writer, entry.Value);
                        writer.WriteLine('"');
                    }
                    else
                    {
                        NonQuoteString(writer, entry.Value);
                        writer.WriteLine();
                    }
                }
            }
        }

        private static readonly char[] EscapedChars = new char[] { '\\', '"' };
        private void QuoteString(System.IO.StreamWriter writer, string p)
        {
            int idx = p.IndexOfAny(EscapedChars);
            int curidx = 0;
            while (idx >= 0)
            {
                if (curidx != idx)
                    writer.Write(p.Substring(curidx, idx - curidx));
                writer.Write('\\');
                curidx = idx;
                idx = p.IndexOfAny(EscapedChars, curidx + 1);
            }
            if (curidx < p.Length)
                writer.Write(p.Substring(curidx));
        }

        private void NonQuoteString(System.IO.StreamWriter writer, string p)
        {
            int idx = p.IndexOf('\n');
            int curidx = 0;
            while (idx >= 0)
            {
                if (idx != curidx)
                    writer.Write(p.Substring(curidx, idx - curidx));
                writer.Write(' ');
                curidx = idx + 1;
                idx = p.IndexOf('\n', curidx);
            }
            if (curidx < p.Length)
                writer.Write(p.Substring(curidx));
        }
    }
}
