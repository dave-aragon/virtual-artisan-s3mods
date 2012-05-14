using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace STBLBrowser
{
    public partial class TextSearch : Form
    {
        public TextSearch()
        {
            InitializeComponent();
        }

        public event GetKeyDataEnumerator KeyNameEnumeratorNeeded;
        public event GetKeyDataEnumerator KeyValueEnumeratorNeeded;
        public event EventHandler<MatchFoundEventArgs> MatchFound;

        string mCurrentSearchString;
        Regex mCurrentSearchExpression;
        IEnumerator<KeyValuePair<UInt64, string>> mCurrentSearchTerms;

        private void resetSearch(object sender, EventArgs e)
        {
            ResetSearch();
        }

        public void ResetSearch()
        {
            mCurrentSearchString = null;
            mCurrentSearchExpression = null;
            if (mCurrentSearchTerms != null)
                mCurrentSearchTerms.Dispose();
            mCurrentSearchTerms = null;
            button1.Enabled = !string.IsNullOrEmpty(textBox1.Text);
            checkMatchWholeWord.Enabled = !checkUseRegex.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoFindNext();
        }

        private bool DoFindNext(UInt64 key)
        {
            if (MatchFound == null)
                return true;

            MatchFoundEventArgs args = new MatchFoundEventArgs(key);
            MatchFound(this, args);
            return args.Handled;
        }

        public void DoFindNext()
        {
            bool firstsearch = false;
            if (mCurrentSearchTerms == null)
            {
                if (searchKeys.Checked)
                {
                    if (KeyNameEnumeratorNeeded == null)
                        throw new InvalidOperationException();

                    mCurrentSearchTerms = KeyNameEnumeratorNeeded().GetEnumerator();
                }
                else
                {
                    if (KeyValueEnumeratorNeeded == null)
                        throw new InvalidOperationException();

                    mCurrentSearchTerms = KeyValueEnumeratorNeeded().GetEnumerator();
                }
                firstsearch = true;
            }
            if (checkUseRegex.Checked)
            {
                if (mCurrentSearchExpression == null)
                {
                    if (string.IsNullOrEmpty(textBox1.Text))
                        return;

                    firstsearch = true;
                    try
                    {
                        if (checkMatchCase.Checked)
                            mCurrentSearchExpression = new Regex(textBox1.Text);
                        else
                            mCurrentSearchExpression = new Regex(textBox1.Text, RegexOptions.IgnoreCase);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Error in regular expression:\n{0}", ex.Message), "Regular Expression Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                while (mCurrentSearchTerms.MoveNext())
                {
                    if (mCurrentSearchExpression.IsMatch(mCurrentSearchTerms.Current.Value))
                    {
                        if (DoFindNext(mCurrentSearchTerms.Current.Key))
                            return;
                    }
                }
            }
            else
            {
                if (mCurrentSearchString == null)
                {
                    if (string.IsNullOrEmpty(textBox1.Text))
                        return;

                    firstsearch = true;

                    mCurrentSearchString = textBox1.Text;
                }

                StringComparison comparer;
                if (checkMatchCase.Checked)
                    comparer = StringComparison.CurrentCulture;
                else
                    comparer = StringComparison.CurrentCultureIgnoreCase;

                while (mCurrentSearchTerms.MoveNext())
                {
                    if (mCurrentSearchTerms.Current.Value.IndexOf(mCurrentSearchString, comparer) >= 0)
                    {
                        if (DoFindNext(mCurrentSearchTerms.Current.Key))
                            return;
                    }
                }
            }

            if (firstsearch)
                MessageBox.Show("Search term not found", "Search Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No more matches found", "Search Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            mCurrentSearchTerms.Dispose();
            mCurrentSearchTerms = null;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (Visible)
            {
                e.Cancel = true;
                Hide();
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    Close();
                    return true;
                case Keys.F3:
                case Keys.Enter:
                    if (button1.Enabled)
                    {
                        DoFindNext();
                        return true;
                    }
                    break;
            }
            return base.ProcessDialogKey(keyData);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            textBox1.Focus();
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = textBox1.Text.Length;
        }
    }
    public delegate IEnumerable<KeyValuePair<UInt64, string>> GetKeyDataEnumerator();
    public class MatchFoundEventArgs : EventArgs
    {
        public MatchFoundEventArgs(UInt64 key)
        {
            Key = key;
        }
        public UInt64 Key;
        public bool Handled;
    }
}
