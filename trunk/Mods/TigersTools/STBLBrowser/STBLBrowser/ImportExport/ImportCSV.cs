using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace STBLBrowser.ImportExport
{
    public partial class ImportCSV : Form
    {
        public static int sLastLanguage = 0;
        public ImportCSV()
        {
            InitializeComponent();
            for (int loop = 0; loop < (int)Languages.Language_Count; loop++)
            {
                string langname = ((Languages)loop).ToString();
                int idx = langname.IndexOf('_');
                if (idx > 0)
                    langname = string.Format("{0} ({1})", langname.Substring(0, idx), langname.Substring(idx + 1).Replace('_', ' '));
                comboBox1.Items.Add(langname);
            }
            comboBox1.SelectedIndex = sLastLanguage;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            sLastLanguage = comboBox1.SelectedIndex;
        }

        public Languages ImportLanguage
        {
            get
            {
                return (Languages)comboBox1.SelectedIndex;
            }
        }

        private void idKey_CheckedChanged(object sender, EventArgs e)
        {
            quoteAll.Enabled = hexNums.Enabled = !idKey.Checked;
        }

        private void otherDelim_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = otherDelim.Checked;
        }

        private string FormatContext(Queue<string> context, int line, StreamReader sr)
        {
            StringBuilder ret = new StringBuilder();
            ret.AppendFormat("Line {0}:", line);
            ret.AppendLine();
            while (context.Count > 1)
            {
                ret.AppendFormat("\t{0}", context.Dequeue());
                ret.AppendLine();
            }
            ret.AppendFormat("-->\t{0}", context.Dequeue());
            ret.AppendLine();
            for (int loop = 0; loop < 2; loop++)
            {
                string extracontext = sr.ReadLine();
                if (extracontext == null)
                    break;
                ret.AppendFormat("\t{0}", extracontext);
                ret.AppendLine();
            }
            return ret.ToString();
        }

        public IEnumerable<KeyValuePair<UInt64, string>> ImportStrings(IWin32Window owner, Stream file)
        {
            StreamReader sr = new StreamReader(file);
            using (sr)
            {
                string line;
                char sepchar = '\t';
                if (whiteSpaceDelim.Checked)
                    sepchar = '\0';
                else if (otherDelim.Checked && textBox2.Text.Length > 0)
                    sepchar = textBox2.Text[0];
                bool hexnum = hexNums.Checked;
                bool quotestrings = quoteStrings.Checked;
                bool quoteall = quoteAll.CanFocus;
                bool keyids = (idNumber.Checked || idDetect.Checked);
                bool keynames = (idKey.Checked || idDetect.Checked);
                StringBuilder keybuilder = new StringBuilder();
                int curline = 0;
                Queue<string> context = new Queue<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    context.Enqueue(line);
                    if (context.Count > 3)
                        context.Dequeue();
                    curline++;
                    line = line.Trim();
                    if (line == string.Empty)
                        continue;
                    int idx1 = -1;
                    int idx2 = -1;
                    if (sepchar != '\0')
                        idx1 = line.IndexOf(sepchar);
                    else
                        for (idx1 = 0; idx1 < line.Length && !char.IsWhiteSpace(line[idx1]); idx1++)
                            ;
                    if (quotestrings && keynames || quoteall)
                        idx2 = line.IndexOf('"');

                    if (idx1 < 0 || idx1 == line.Length)
                    {
                        MessageBox.Show(owner, string.Format("Invalid line in text file:\n{0}", FormatContext(context, curline, sr)), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        yield break;
                    }

                    bool wasquoted = false;
                    string key;
                    if ((quoteall || quotestrings && keynames) && idx2 >= 0 && idx2 < idx1)
                    {
                        wasquoted = true;
                        int curidx = idx2 + 1;
                        int idx3 = line.IndexOf('\\', curidx);
                        int idx4 = line.IndexOf('"', curidx);
                        if (idx3 >= 0 && idx3 < idx4)
                        {
                            keybuilder.Length = 0;
                            while (idx3 >= 0 && idx3 < idx4)
                            {
                                keybuilder.Append(line.Substring(curidx, idx3 - curidx));
                                curidx = idx3 + 1;
                                idx3 = line.IndexOf('\\', curidx + 1);
                                if (idx4 == curidx)
                                    idx4 = line.IndexOf('"', curidx + 1);
                            }
                            if (idx4 < 0)
                            {
                                MessageBox.Show(owner, string.Format("Invalid line in text file:\n{0}", FormatContext(context, curline, sr)), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                yield break;
                            }
                            keybuilder.Append(line.Substring(curidx, idx4 - curidx));
                            key = keybuilder.ToString();
                        }
                        else
                        {
                            if (idx4 < 0)
                            {
                                MessageBox.Show(owner, string.Format("Invalid line in text file:\n{0}", FormatContext(context, curline, sr)), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                yield break;
                            }
                            key = line.Substring(curidx, idx4 - curidx);
                        }
                        if (sepchar != '\0')
                            idx1 = line.IndexOf(sepchar, idx4 + 1);
                        else
                            for (idx1 = idx4 + 1; idx1 < line.Length && !char.IsWhiteSpace(line[idx1]); idx1++)
                                ;
                        if (idx1 < 0 || idx1 >= line.Length)
                        {
                            MessageBox.Show(owner, string.Format("Invalid line in text file:\n{0}", FormatContext(context, curline, sr)), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            yield break;
                        }
                    }
                    else
                    {
                        key = line.Substring(0, idx1);
                    }
                    string value;
                    if (quotestrings)
                    {
                        int idx3 = line.IndexOf('"', idx1 + 1);
                        if (idx3 < 0)
                        {
                            MessageBox.Show(owner, string.Format("Strings must be quoted:\n{0}", FormatContext(context, curline, sr)), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            yield break;
                        }
                        int curidx = idx3 + 1;
                        idx3 = line.IndexOf('"', curidx);
                        int idx4 = line.IndexOf('\\', curidx);
                        StringBuilder valuestr = new StringBuilder();
                        while (idx3 < 0 || idx4 >= 0 && idx4 < idx3)
                        {
                            if (idx4 > 0)
                            {
                                valuestr.Append(line.Substring(curidx, idx4 - curidx));
                                curidx = idx4 + 1;
                                idx4 = line.IndexOf('\\', curidx + 1);
                                if (curidx == idx3)
                                    idx3 = line.IndexOf('"', curidx + 1);
                            }
                            else
                            {
                                valuestr.AppendLine(line.Substring(curidx));
                                curidx = 0;
                                line = sr.ReadLine();
                                context.Enqueue(line);
                                if (context.Count > 3)
                                    context.Dequeue();
                                curline++;
                                if (line == null)
                                {
                                    MessageBox.Show(owner, string.Format("End of file looking for end of string quote", FormatContext(context, curline, sr)), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    yield break;
                                }
                                curidx = 0;
                                idx4 = line.IndexOf('\\');
                                idx3 = line.IndexOf('"');
                            }
                        }
                        valuestr.Append(line.Substring(curidx, idx3 - curidx));
                        value = valuestr.ToString();
                    }
                    else
                        value = line.Substring(idx1 + 1);

                    UInt64 id = 0;
                    if (!keyids)
                    {
                        id = STBLVault.FNV64(key);
                        if (STBLVault.LookupKey(id) == null)
                            STBLVault.AddNewKey(key);
                    }
                    else if (!keynames)
                    {
                        bool ishex = hexnum;
                        string parsekey = key.ToLower();
                        if (parsekey.StartsWith("0x"))
                        {
                            ishex = true;
                            parsekey = parsekey.Substring(2);
                        }
                        if (ishex && !UInt64.TryParse(key, System.Globalization.NumberStyles.HexNumber, null, out id)
                            || !ishex && !UInt64.TryParse(key, out id))
                        {
                            MessageBox.Show(owner, string.Format("Error parsing key ID {0}\n{1}", key, FormatContext(context, curline, sr)), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            yield break;
                        }
                    }
                    else
                    {
                        if (!quoteall && wasquoted)
                        {
                            id = STBLVault.FNV64(key);
                            if (STBLVault.LookupKey(id) == null)
                                STBLVault.AddNewKey(key);
                        }
                        else
                        {
                            bool ishex = hexnum || (!quoteall && !wasquoted && quotestrings);
                            string parsekey = key.ToLower();
                            if (parsekey.StartsWith("0x"))
                            {
                                ishex = true;
                                parsekey = parsekey.Substring(2);
                            }
                            if (ishex && !UInt64.TryParse(parsekey, System.Globalization.NumberStyles.HexNumber, null, out id)
                                || !ishex && !UInt64.TryParse(parsekey, out id))
                            {
                                if (quotestrings && wasquoted)
                                {
                                    MessageBox.Show(owner, string.Format("Error parsing key ID {0}\n{1}", key, FormatContext(context, curline, sr)), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    yield break;
                                }
                                id = STBLVault.FNV64(key);
                                if (STBLVault.LookupKey(id) == null)
                                    STBLVault.AddNewKey(key);
                            }
                        }
                    }

                    yield return new KeyValuePair<UInt64, string>(id, value);
                }
            }
        }
    }
}
