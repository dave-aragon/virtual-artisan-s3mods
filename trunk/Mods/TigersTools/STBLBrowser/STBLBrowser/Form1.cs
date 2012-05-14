using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using Microsoft.Win32;

using DBPF;

namespace STBLBrowser
{
    public partial class Form1 : Form
    {
        public Form1()
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
            comboBox1.SelectedIndex = 0;
            mStubbleEntries = new STBLVault();
            PopulateTreeRoot();
        }

        private STBLVault mStubbleEntries;

        private void PopulateTreeRoot()
        {
            treeView1.Nodes.Clear();

            List<string> nodelabels = new List<string>();
            List<UInt64> unlabeled = new List<ulong>();

            foreach (KeyValuePair<UInt64, string> entry in mStubbleEntries.GetEntriesForLanguage(mStubbleEntries.CurrentLanguage, STBLVault.StringSource.Display))
            {
                string label;
                if (STBLVault.LookupKey(entry.Key, out label))
                    nodelabels.Add(label);
                else
                    unlabeled.Add(entry.Key);
            }

            nodelabels.Sort();
            unlabeled.Sort();
            PopulateTree(nodelabels, treeView1.Nodes);

            if (unlabeled.Count > 0)
            {
                TreeNode unknode = NewTreeNode(UnknownId);
                if (unlabeled.Count > 256)
                    unknode.Nodes.Add("!").Tag = unlabeled;
                else
                    unknode.Tag = unlabeled;
                treeView1.Nodes.Add(unknode);
            }
        }

        private static readonly string UnknownId = "(Unknown ID)";

        private TreeNode NewTreeNode(string label)
        {
            TreeNode ret = new TreeNode(label);
            ret.Name = label;
            return ret;
        }

        private TreeNode NewTreeNode(TreeNodeCollection nodes, string label)
        {
            TreeNode ret = nodes.Add(label);
            ret.Name = label;
            return ret;
        }

        private void PopulateTree(List<ulong> unlabeled, TreeNodeCollection treeNodeCollection)
        {
            // All the bits that any of the values have as 1
            UInt64 mask = 0;
            // All the bits that any of the values have as 0
            UInt64 invmask = 0;
            foreach (UInt64 id in unlabeled)
            {
                mask |= id;
                invmask |= ~id;
            }
            // All the bits that have been seen as both 1 and 0 - bits that are not the same
            mask &= invmask;
            int maskbits = 0;
            for (maskbits = 60; maskbits > 8; maskbits -= 4)
            {
                if ((mask & ~((1UL << maskbits) - 1)) > 0)
                    break;
            }

            string labelfmtstring = "{0:X" + ((64 - maskbits) / 4).ToString() + "}";
            TreeNode lastnode = null;
            List<UInt64> lastnodechildren = null;
            UInt64 lastnodelabel = ~0UL;

            foreach (UInt64 id in unlabeled)
            {
                UInt64 thislabel = (id >> maskbits);
                if (lastnode == null || thislabel != lastnodelabel)
                {
                    if (lastnodechildren != null && lastnodechildren.Count > 256)
                        lastnode.Nodes.Add("!").Tag = lastnodechildren;
                    else if (lastnode != null)
                        lastnode.Tag = lastnodechildren;
                    lastnodelabel = thislabel;
                    lastnode = NewTreeNode(string.Format(labelfmtstring, lastnodelabel));
                    lastnodechildren = new List<UInt64>();
                    treeNodeCollection.Add(lastnode);
                }
                lastnodechildren.Add(id);
            }
            if (lastnodechildren != null && lastnodechildren.Count > 256)
                lastnode.Nodes.Add("!").Tag = lastnodechildren;
            else
                lastnode.Tag = lastnodechildren;
        }

        private void PopulateTree(List<string> labels, TreeNodeCollection treenodes)
        {
            TreeNode lastnode = null;
            List<string> lastnodeentities = null;
            List<string> lastnodechildren = null;
            foreach (string label in labels)
            {
                string thislabel;
                string remainingtext;

                bool issubtree = SplitNameText(label, out thislabel, out remainingtext);

                int compareresult = 0;
                if (lastnode == null || (compareresult = StringComparer.CurrentCultureIgnoreCase.Compare(lastnode.Text, thislabel)) != 0)
                {
                    lastnode = null;
                    lastnodeentities = null;
                    lastnodechildren = null;
                    lastnode = treenodes[thislabel];
                    if (lastnode == null)
                    {
                        if (compareresult > 0)
                            lastnode = InsertNewNode(treenodes, thislabel);
                        else
                        {
                            lastnode = NewTreeNode(thislabel);
                            treenodes.Add(lastnode);
                        }
                    }
                    else
                    {
                        lastnodeentities = lastnode.Tag as List<string>;
                        if (lastnode.Nodes.Count > 0)
                            lastnodechildren = lastnode.Nodes[0].Tag as List<string>;
                    }
                }
                if (!issubtree)
                {
                    if (lastnodeentities == null)
                    {
                        lastnodeentities = new List<string>();
                        lastnode.Tag = lastnodeentities;
                    }
                    lastnodeentities.Add(remainingtext);
                }
                else
                {
                    if (lastnodechildren == null)
                    {
                        lastnodechildren = new List<string>();
                        lastnode.Nodes.Add("!").Tag = lastnodechildren;
                    }
                    lastnodechildren.Add(remainingtext);
                }
            }
        }

        private static bool SplitNameText(string label, out string thislabel, out string remainingtext)
        {
            bool issubtree = false;
            int idx1 = label.IndexOf('/');
            int idx2 = label.LastIndexOf(':');

            if (idx1 < 0)
                idx1 = label.Length;
            // Don't allow : as first character
            if (idx2 <= 0)
                idx2 = Math.Max(label.LastIndexOf('/'), idx1);

            thislabel = label.Substring(0, Math.Min(idx1, idx2));
            remainingtext = null;
            if (idx2 <= idx1)
            {
                if (idx1 == idx2)
                {
                    if (idx1 != label.Length)
                        remainingtext = label.Substring(idx2);
                }
                else
                {
                    string entry = label.Substring(idx2 + 1);
                    if (entry.StartsWith(":") || entry.StartsWith("/"))
                        remainingtext = entry.Substring(1);
                    else
                        remainingtext = entry;
                }
            }
            else
            {
                issubtree = true;
                remainingtext = label.Substring(idx1 + 1);
            }

            return issubtree;
        }

        private Languages SelectedLanguage
        {
            get
            {
                return (Languages)comboBox1.SelectedIndex;
            }
        }


        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (mForceSelectCell.X >= 0)
            {
                Point newcell = mForceSelectCell;
                mForceSelectCell = new Point(-1, -1);
                dataGridView1.CurrentCell = dataGridView1.Rows[newcell.Y].Cells[dataGridView1.CurrentCellAddress.X];
            }
            if (treeView1.SelectedNode == null)
                return;
            switch (e.ColumnIndex)
            {
                case 0:
                    List<string> nodentries = treeView1.SelectedNode.Tag as List<string>;
                    if (nodentries != null)
                    {
                        if (e.RowIndex < nodentries.Count)
                            e.Value = nodentries[e.RowIndex];
                    }
                    else
                    {
                        List<UInt64> nodeids = treeView1.SelectedNode.Tag as List<UInt64>;
                        if (nodeids != null && e.RowIndex < nodeids.Count)
                            e.Value = string.Format("0x{0:X16}", nodeids[e.RowIndex]);
                    }
                    break;
                case 1:
                    e.Value = GetValueOf(e.RowIndex);
                    break;
            }
        }

        private string GetValueOf(int index)
        {
            UInt64 rowkeyhash = GetRowKey(index);
            StubbleEntry entry;
            if (mStubbleEntries.TryGetValue(rowkeyhash, out entry) != STBLVault.StringSource.None)
            {
                if (index == dataGridView1.CurrentCellAddress.Y)
                {
                    if (entry.Source.SourceTable != DBPFReference.MinValue)
                        label2.Text = string.Format("{0} : 0x{1:X2}-{2:X6}-{3:X8}", Path.GetFileNameWithoutExtension(entry.Source.SourceFile), entry.Source.SourceTable.Instance >> 56, (entry.Source.SourceTable.Instance >> 32) & 0xFFFFFFUL, entry.Source.SourceTable.Instance & 0xFFFFFFFFUL);
                    else if (entry.Source.SourceFile != null)
                        label2.Text = string.Format("{0} : Unassigned", Path.GetFileNameWithoutExtension(entry.Source.SourceFile));
                    else
                        label2.Text = "Unassigned";
                    label3.Text = string.Format("String ID 0x{0:X16}", rowkeyhash);
                }
                return entry.Value;
            }
            if (index == dataGridView1.CurrentCellAddress.Y)
            {
                label2.Text = string.Empty;
                label3.Text = string.Empty;
            }
            return "";
        }

        private UInt64 GetRowKey(int index)
        {
            try
            {
                List<string> nodentries = treeView1.SelectedNode.Tag as List<string>;
                if (nodentries != null && index < nodentries.Count)
                {
                    string entry = nodentries[index];
                    if (entry == null)
                        entry = treeView1.SelectedNode.FullPath;
                    else
                    {
                        if (entry.StartsWith(":") || entry.StartsWith("/"))
                            entry = treeView1.SelectedNode.FullPath + entry;
                        else
                            entry = string.Format("{0}:{1}", treeView1.SelectedNode.FullPath, entry);
                    }
                    return STBLVault.FNV64(entry);
                }
                List<UInt64> nodeids = treeView1.SelectedNode.Tag as List<UInt64>;
                if (nodeids != null && index < nodeids.Count)
                {
                    return nodeids[index];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return 0;
        }

        private string GetRowKeyName(int index)
        {
            List<string> nodentries = treeView1.SelectedNode.Tag as List<string>;
            if (nodentries != null && index < nodentries.Count)
            {
                string entry = nodentries[index];
                if (entry == null)
                    entry = treeView1.SelectedNode.FullPath;
                else
                {
                    if (entry.StartsWith(":") || entry.StartsWith("/"))
                        entry = treeView1.SelectedNode.FullPath + entry;
                    else
                        entry = string.Format("{0}:{1}", treeView1.SelectedNode.FullPath, entry);
                }
                return entry;
            }
            return string.Empty;
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "!" && e.Node.Nodes[0].Name != "!")
            {
                TreeNode toexpand = e.Node;

                ExpandUnfinishedNode(toexpand);
            }
        }

        private void ExpandUnfinishedNode(TreeNode toexpand)
        {
            List<string> nodelist = toexpand.Nodes[0].Tag as List<string>;
            if (nodelist != null)
            {
                toexpand.Nodes.Clear();
                PopulateTree(nodelist, toexpand.Nodes);
            }
            else
            {
                List<UInt64> idlist = toexpand.Nodes[0].Tag as List<UInt64>;
                if (idlist != null && idlist.Count > 256)
                {
                    toexpand.Nodes.Clear();
                    PopulateTree(idlist, toexpand.Nodes);
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            UpdateDataGridList(node);
        }

        private void UpdateDataGridList(TreeNode node)
        {
            List<string> nodeentries = null;
            List<UInt64> nodeids = null;

            if (node != null)
            {
                nodeentries = node.Tag as List<string>;
                nodeids = node.Tag as List<UInt64>;
            }

            int entries = 0;
            if (nodeentries != null)
                entries = nodeentries.Count;
            else if (nodeids != null)
                entries = nodeids.Count;
            SetRowCount(entries + (dataGridView1.AllowUserToAddRows? 1: 0));
            dataGridView1.Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mStubbleEntries == null)
                return;
            mStubbleEntries.SetLanguage((Languages)comboBox1.SelectedIndex);

            RepopulateTree();
        }

        private void RepopulateTree()
        {
            if (mSearchDialog != null)
                mSearchDialog.ResetSearch();

            string curpath = null;
            UInt64 id = 0;
            if (treeView1.SelectedNode != null)
            {
                curpath = treeView1.SelectedNode.FullPath;
                id = GetRowKey(dataGridView1.CurrentCellAddress.Y);
            }
            treeView1.SelectedNode = null;
            PopulateTreeRoot();
            if (curpath != null)
            {
                TreeNodeCollection collection = treeView1.Nodes;
                TreeNode curnode = null;
                foreach (string part in curpath.Split('/'))
                {
                    TreeNode[] next = collection.Find(part, false);
                    if (next.Length == 1)
                    {
                        curnode = next[0];
                        collection = curnode.Nodes;
                        curnode.Expand();
                    }
                }
                if (curnode != null)
                    treeView1.SelectedNode = curnode;

                int index;
                FindNodeForId(id, false, out curnode, out index);
                if (curnode != null)
                {
                    treeView1.SelectedNode = curnode;
                    UpdateDataGridList(curnode);
                    dataGridView1.CurrentCell = dataGridView1.Rows[index].Cells[1];
                }
            }
            UpdateDataGridList(treeView1.SelectedNode);
        }

        void FindNodeForId(UInt64 id, bool createnew, out TreeNode node, out int index)
        {
            node = null;
            index = 0;

            string keyname;
            if (STBLVault.LookupKey(id, out keyname))
            {
                TreeNodeCollection collection = treeView1.Nodes;
                TreeNode foundnode = null;
                string thispart;
                string nextpart;
                while (SplitNameText(keyname, out thispart, out nextpart))
                {
                    foundnode = GetChildNode(createnew, collection, thispart);
                    if (foundnode == null)
                        return;
                    collection = foundnode.Nodes;
                    keyname = nextpart;
                }

                if (thispart == null)
                    thispart = nextpart;
                else if (nextpart != null)
                {
                    foundnode = GetChildNode(createnew, collection, thispart);
                    if (foundnode == null)
                        return;
                    thispart = nextpart;
                }

                if (foundnode == null)
                {
                    if (!createnew)
                        return;

                    foundnode = InsertNewNode(collection, thispart);
                }

                List<string> entries = foundnode.Tag as List<string>;
                if (entries == null)
                {
                    if (!createnew)
                        return;

                    // Huh???
                    if (foundnode.Tag != null)
                        return;

                    entries = new List<string>();
                    foundnode.Tag = entries;
                }

                int min = 0;
                int max = entries.Count - 1;
                int location;
                bool found;
                string haystack = null;
                while (StringSearchHelper(thispart, haystack, ref min, ref max, out location, out found))
                    haystack = entries[location];

                if (!found)
                {
                    if (!createnew)
                        return;

                    entries.Insert(location, thispart);
                }
                node = foundnode;
                index = location;
            }
            else
            {
                TreeNode curnode;
                if (treeView1.Nodes.Count == 0 || treeView1.Nodes[treeView1.Nodes.Count - 1].Text != UnknownId)
                {
                    if (!createnew)
                        return;
                    curnode = NewTreeNode(treeView1.Nodes, UnknownId);
                }
                else
                    curnode = treeView1.Nodes[treeView1.Nodes.Count - 1];

                if (curnode.Nodes.Count == 1 && curnode.Nodes[0].Text == "!" && curnode.Nodes[0].Name != "!")
                {
                    ExpandUnfinishedNode(curnode);
                }

                keyname = string.Format("{0:X16}", id);

                while (curnode.Nodes.Count > 0)
                {
                    string cutstr = keyname.Substring(0, curnode.Nodes[0].Text.Length);
                    curnode = GetChildNode(createnew, curnode.Nodes, cutstr);
                    if (curnode == null)
                        return;
                }

                List<UInt64> entries = curnode.Tag as List<UInt64>;
                if (entries == null)
                {
                    if (!createnew)
                        return;

                    // !!!
                    if (curnode.Tag != null)
                        return;

                    entries = new List<ulong>();
                    entries.Add(id);
                    curnode.Tag = entries;
                    index = 0;
                }
                else
                {
                    int loop;
                    for (loop = 0; loop < entries.Count; loop++)
                    {
                        if (entries[loop] >= id)
                            break;
                    }

                    if (loop >= entries.Count || entries[loop] != id)
                    {
                        if (!createnew)
                            return;

                        entries.Insert(loop, id);
                    }

                    index = loop;
                }

                node = curnode;
            }
        }

        private TreeNode GetChildNode(bool createnew, TreeNodeCollection collection, string thispart)
        {
            TreeNode foundnode = collection[thispart];
            if (foundnode == null)
            {
                if (!createnew)
                    return null;

                foundnode = InsertNewNode(collection, thispart);
            }
            else if (foundnode.Nodes.Count == 1 && foundnode.Nodes[0].Text == "!" && foundnode.Nodes[0].Name != "!")
            {
                ExpandUnfinishedNode(foundnode);
            }
            return foundnode;
        }

        public bool StringSearchHelper(string needle, string haystackspot, ref int minval, ref int maxval, out int foundloc, out bool found)
        {
            int thisloc = (minval + maxval) / 2;

            if (haystackspot == null)
            {
                found = false;
                if (maxval < minval)
                {
                    foundloc = minval;
                    return false;
                }
                foundloc = thisloc;
                return true;
            }

            int compare = StringComparer.CurrentCultureIgnoreCase.Compare(needle, haystackspot);
            if (compare == 0)
            {
                foundloc = thisloc;
                found = true;
                return false;
            }
            found = false;
            if (compare < 0)
                maxval = thisloc - 1;
            else
                minval = thisloc + 1;
            if (minval > maxval)
            {
                foundloc = minval;
                return false;
            }
            foundloc = (minval + maxval) / 2;
            return true;
        }

        private TreeNode InsertNewNode(TreeNodeCollection collection, string nodetext)
        {
            TreeNode newnode = NewTreeNode(nodetext);

            int insertat = 0;
            int min = 0;
            int max = collection.Count - 1;
            string haystack = null;
            bool found;
            if (collection.Count > 0 && object.ReferenceEquals(collection, treeView1.Nodes)
                && collection[max - 1].Text == UnknownId)
                max--;

            while (StringSearchHelper(nodetext, haystack, ref min, ref max, out insertat, out found))
                haystack = collection[insertat].Text;

            if (insertat < collection.Count)
                collection.Insert(insertat, newnode);
            else
                collection.Add(newnode);
            return newnode;
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!modifiedStringsToolStripMenuItem.Checked)
            {
                e.Cancel = true;
                return;
            }
            if (e.ColumnIndex == 0)
            {
                e.Cancel = true;
                List<string> vals = treeView1.SelectedNode.Tag as List<string>;
                if (vals != null)
                {
                    if (e.RowIndex == vals.Count)
                        e.Cancel = false;
                    else if (e.RowIndex < vals.Count)
                    {
                        UInt64 keyid = GetRowKey(e.RowIndex);
                        string value;
                        if (mStubbleEntries.TryGetValue(keyid, out value) != STBLVault.StringSource.None && !string.IsNullOrEmpty(value))
                            e.Cancel = true;
                    }
                }
                else if (treeView1.SelectedNode.Tag == null)
                {
                    TreeNode node = treeView1.SelectedNode;
                    while (node.Parent != null)
                        node = node.Parent;
                    if (node.Text != UnknownId)
                    {
                        e.Cancel = false;
                        treeView1.SelectedNode.Tag = new List<string>();
                    }
                }
            }
            else if (e.ColumnIndex == 1)
            {
                int maxidx = 0;
                List<string> strings = treeView1.SelectedNode.Tag as List<string>;
                List<UInt64> keys = treeView1.SelectedNode.Tag as List<UInt64>;
                if (strings != null)
                    maxidx = strings.Count;
                if (keys != null)
                    maxidx = keys.Count;
                if (e.RowIndex >= maxidx)
                    e.Cancel = true;
            }
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (mIsAdding)
                return;

            int start = e.RowIndex;
            int end = start + e.RowCount;

            List<string> strings = treeView1.SelectedNode.Tag as List<string>;
            List<UInt64> keys = treeView1.SelectedNode.Tag as List<UInt64>;
            if (strings != null)
            {
                if (start < strings.Count)
                    strings.RemoveRange(start, Math.Min(strings.Count, end) - start);
            }
            if (keys != null)
            {
                if (start < keys.Count)
                    keys.RemoveRange(start, Math.Min(keys.Count, end) - start);
            }
        }

        bool mIsAdding;
        private void SetRowCount(int count)
        {
            mIsAdding = true;
            try
            {
                dataGridView1.RowCount = count;
            }
            finally
            {
                mIsAdding = false;
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (mIsAdding)
                return;
            int start = e.RowIndex;
            int end = start + e.RowCount;

            List<string> strings = treeView1.SelectedNode.Tag as List<string>;
            List<UInt64> keys = treeView1.SelectedNode.Tag as List<UInt64>;
            if (strings != null)
            {
                while (start > strings.Count)
                {
                    start--;
                    end--;
                }
                while (start < end)
                {
                    strings.Insert(start, string.Empty);
                    start++;
                }
            }
            if (keys != null)
            {
                while (start > keys.Count)
                {
                    start--;
                    end--;
                }
                while (start < end)
                {
                    keys.Insert(start, 0);
                    start++;
                }
            }
        }

        bool mUpdatingCellValue;
        private void dataGridView1_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (mUpdatingCellValue)
                return;
            try
            {
                mUpdatingCellValue = true;
                if (e.ColumnIndex == 0)
                    UpdateRowKey(e);
                else
                    UpdateRowValue(e);
            }
            finally
            {
                mUpdatingCellValue = false;
            }
            if (mForceSelectCell.Y < 0)
                dataGridView1.InvalidateRow(e.RowIndex);
                
        }

        private void UpdateRowValue(DataGridViewCellValueEventArgs e)
        {
            UInt64 keyid = GetRowKey(e.RowIndex);
            mStubbleEntries.SetEntry(keyid, e.Value as string);

            if (mSearchDialog != null)
                mSearchDialog.ResetSearch();

            string keyname;
            List<string> keylist = treeView1.SelectedNode.Tag as List<string>;
            if (keylist != null && !STBLVault.LookupKey(keyid, out keyname))
            {
                AddKeyFromCurrentNode(e.RowIndex);
            }
            return;
        }

        private void AddKeyFromCurrentNode(int index)
        {
            List<string> keylist = treeView1.SelectedNode.Tag as List<string>;
            if (keylist == null || index < 0 || index >= keylist.Count)
                return;

            string keyname = keylist[index];

            if (keyname.StartsWith(":") || keyname.StartsWith("/"))
                keyname = treeView1.SelectedNode.FullPath + keyname;
            else
                keyname = string.Format("{0}:{1}", treeView1.SelectedNode.FullPath, keyname);
            STBLVault.AddNewKey(keyname);
            STBLVault.SyncKeyNameMap();
        }

        private void UpdateRowKey(DataGridViewCellValueEventArgs e)
        {
            List<string> strings = treeView1.SelectedNode.Tag as List<string>;
            List<UInt64> keys = treeView1.SelectedNode.Tag as List<UInt64>;

            string valstr = e.Value as string;
            if (valstr == null)
                valstr = string.Empty;

            if (mSearchDialog != null)
                mSearchDialog.ResetSearch();

            if (strings != null && e.RowIndex < strings.Count)
            {
                if (valstr == string.Empty)
                {
                    UInt64 oldkeyid = GetRowKey(e.RowIndex);
                    if (!string.IsNullOrEmpty(GetValueOf(e.RowIndex)))
                    {
                        MessageBox.Show(this, "Can not remove keys with a value set.", "Can not remove", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    mStubbleEntries.RemoveEntry(oldkeyid, STBLVault.StringSource.Modified, true);
                    if (mStubbleEntries.TryGetValue(oldkeyid, out valstr) != STBLVault.StringSource.None)
                    {
                        dataGridView1.Invalidate();
                        return;
                    }

                    strings.RemoveAt(e.RowIndex);
                    SetRowCount(dataGridView1.RowCount - 1);
                    dataGridView1.Invalidate();
                    return;
                }

                strings[e.RowIndex] = valstr;
                UInt64 keyid = GetRowKey(e.RowIndex);
                TreeNode node;
                int index;
                FindNodeForId(keyid, false, out node, out index);
                bool sort = true;
                if (node != null)
                {
                    if (object.ReferenceEquals(node, treeView1.SelectedNode))
                    {
                        sort = false;
                        if (index != e.RowIndex)
                        {
                            if (string.IsNullOrEmpty(strings[index]))
                            {
                                strings[index] = strings[e.RowIndex];
                                strings.RemoveAt(e.RowIndex);
                                SetRowCount(dataGridView1.RowCount - 1);
                                mForceSelectCell = new Point(0, index);
                                dataGridView1.Invalidate();
                            }
                            else if (string.IsNullOrEmpty(strings[e.RowIndex]))
                            {
                                strings.RemoveAt(e.RowIndex);
                                SetRowCount(dataGridView1.RowCount - 1);
                                mForceSelectCell = new Point(0, index);
                                dataGridView1.Invalidate();
                            }
                            else
                            {
                                MessageBox.Show(this, string.Format("Key {0} already exists in the table at row {1}", strings[e.RowIndex], index), "Name Exists", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                    }
                    else
                    {
                        List<UInt64> othernodekeys = node.Tag as List<UInt64>;
                        if (othernodekeys != null)
                        {
                            AddKeyFromCurrentNode(e.RowIndex);
                            othernodekeys.RemoveAt(index);
                            if (othernodekeys.Count == 0)
                            {
                                TreeNode parent = node.Parent;
                                node.Remove();
                                while (parent != null && parent.Tag == null && parent.Nodes.Count == 0)
                                {
                                    node = parent;
                                    parent = node.Parent;
                                    node.Remove();
                                }
                            }
                        }
                    }
                }
                if (sort)
                {
                    int count = 0;
                    string needle = string.Empty;

                    if (keys != null)
                    {
                        count = keys.Count;
                        needle = keys[e.RowIndex].ToString("X16");
                    }
                    else if (strings != null)
                    {
                        count = strings.Count;
                        needle = strings[e.RowIndex];
                    }

                    int min = 0;
                    int max = count - 2;
                    int loc;
                    bool found;
                    string haystack = null;
                    while (StringSearchHelper(needle, haystack, ref min, ref max, out loc, out found))
                    {
                        if (loc >= e.RowIndex)
                            loc++;

                        if (keys != null)
                            haystack = keys[loc].ToString("X16");
                        else if (strings != null)
                            haystack = strings[loc];
                    }

                    if (loc != e.RowIndex)
                    {
                        if (strings != null)
                        {
                            string val = strings[e.RowIndex];
                            strings.RemoveAt(e.RowIndex);
                            if (loc == strings.Count)
                                strings.Add(val);
                            else
                                strings.Insert(loc, val);
                        }
                        else if (keys != null)
                        {
                            UInt64 val = keys[e.RowIndex];
                            keys.RemoveAt(e.RowIndex);
                            if (loc == keys.Count)
                                keys.Add(val);
                            else
                                keys.Insert(loc, val);
                        }

                        int col = dataGridView1.CurrentCellAddress.X;
                        if (col < 0)
                            col = 0;
                        mForceSelectCell = new Point(col, loc);
                    }

                    dataGridView1.Invalidate();
                }
            }
        }

        private ImportExport.IExport mCurExporter;
        private string mCurFilename;

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    ImportExport.IExport exporter;
                    string filename = saveFileDialog1.FileName;
                    string extension = Path.GetExtension(filename).ToLower();
                    if ((saveFileDialog1.FilterIndex == 1 || extension == ".package") && !(extension == ".csv" || extension == ".txt"))
                    {
                        exporter = new ImportExport.ExportPackage();
                    }
                    else
                    {
                        exporter = new ImportExport.ExportCSV();
                    }

                    exporter.STBLData = mStubbleEntries;
                    if (exporter.Export(this, filename))
                    {
                        mCurFilename = filename;
                        mCurExporter = exporter;
                        Text = "Stubble String Table Editor - " + filename;
                        dataGridView1.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error saving\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            CheckDGCopyPaste();
            if (treeView1.SelectedNode != null && dataGridView1.CurrentCellAddress.Y >= 0)
                GetValueOf(dataGridView1.CurrentCellAddress.Y);
        }

        private void CheckDGCopyPaste()
        {
            bool HasName = false;
            bool HasValue = false;

            List<UInt64> keys = null;
            List<string> names = null;

            if (treeView1.SelectedNode != null)
            {
                keys = treeView1.SelectedNode.Tag as List<UInt64>;
                names = treeView1.SelectedNode.Tag as List<string>;
            }

            if (dataGridView1.CurrentCellAddress.Y >= 0)
            {
                if (keys != null && dataGridView1.CurrentCellAddress.Y < keys.Count)
                    HasValue = true;
                else if (names != null && dataGridView1.CurrentCellAddress.Y < names.Count)
                {
                    HasValue = true;
                    HasName = true;
                }
            }

            copyKeyIDToolStripMenuItem.Enabled = HasValue;
            copyKeyToolStripMenuItem.Enabled = HasName;
            copyToolStripMenuItem.Enabled = HasValue;
            copyValueToolStripMenuItem.Enabled = HasValue;
            if (modifiedStringsToolStripMenuItem.Checked)
            {
                cutToolStripMenuItem.Enabled = HasValue && dataGridView1.CurrentCellAddress.X == 1;
                pasteToolStripMenuItem.Enabled = HasValue && dataGridView1.CurrentCellAddress.X == 1;
            }
            else
            {
                cutToolStripMenuItem.Enabled = false;
                pasteToolStripMenuItem.Enabled = false;
            }
        }

        private void dataGridView1_Enter(object sender, EventArgs e)
        {
            CheckDGCopyPaste();
        }

        private void copyKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetRowKeyName(dataGridView1.CurrentCellAddress.Y);
            if (!string.IsNullOrEmpty(name))
                CopySimpleString(name);
        }

        private static void CopySimpleString(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            DataObject obj = new DataObject();
            obj.SetText(name, TextDataFormat.UnicodeText);
            bool isascii = true;
            for (int loop = 0; loop < name.Length; loop++)
            {
                if (name[loop] >= 128)
                    isascii = false;
            }
            if (isascii)
                obj.SetText(name, TextDataFormat.Text);
            Clipboard.SetDataObject(obj);
        }

        private void copyKeyIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UInt64 itemid = GetRowKey(dataGridView1.CurrentCellAddress.Y);
            CopySimpleString(string.Format("0x{0:X16}", itemid));
        }

        private void copyValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySimpleString(GetValueOf(dataGridView1.CurrentCellAddress.Y));
        }

        private Point mForceSelectCell = new Point(-1, -1);
        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            mForceSelectCell = new Point(-1, -1);
        }

        private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            e.CancelEdit = !string.IsNullOrEmpty(e.Node.Text);
            copyToolStripMenuItem.Enabled = false;
            pasteToolStripMenuItem.Enabled = false;
            cutToolStripMenuItem.Enabled = false;
        }

        private void rootCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode newnode = new TreeNode();
            treeView1.Nodes.Insert(0, newnode);
            newnode.BeginEdit();
        }

        private void subcategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                rootCategoryToolStripMenuItem.PerformClick();
            TreeNode newnode;

            newnode = treeView1.SelectedNode;
            while (newnode.Parent != null)
                newnode = newnode.Parent;

            if (newnode.NextNode == null && newnode.Text == UnknownId)
                return;

            newnode = new TreeNode();
            treeView1.SelectedNode.Nodes.Insert(0, newnode);
            newnode.EnsureVisible();
            newnode.BeginEdit();
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            copyToolStripMenuItem.Enabled = true;
            pasteToolStripMenuItem.Enabled = true;
            cutToolStripMenuItem.Enabled = true;
            TreeNodeCollection parentnodes;
            if (e.Node.Parent != null)
                parentnodes = e.Node.Parent.Nodes;
            else
                parentnodes = treeView1.Nodes;
            e.Node.Remove();
            if (!string.IsNullOrEmpty(e.Label))
            {
                if (parentnodes.ContainsKey(e.Label))
                    treeView1.SelectedNode = parentnodes[e.Label];
                else
                    treeView1.SelectedNode = InsertNewNode(parentnodes, e.Label);
            }
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            UInt64 keyid = GetRowKey(e.Row.Index);
            mStubbleEntries.RemoveEntry(keyid, STBLVault.StringSource.Display, true);
            dataGridView1.InvalidateRow(e.Row.Index);
            string value;
            e.Cancel = (mStubbleEntries.TryGetValue(keyid, out value) != STBLVault.StringSource.None);
        }

        private int SelectedNodeKeyCount()
        {
            if (treeView1.SelectedNode == null)
                return 0;
            if (treeView1.SelectedNode.Tag == null)
                return 0;

            List<string> strings = treeView1.SelectedNode.Tag as List<string>;
            if (strings != null)
                return strings.Count;
            List<UInt64> keys = treeView1.SelectedNode.Tag as List<UInt64>;
            if (keys != null)
                return keys.Count;
            return 0;
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= SelectedNodeKeyCount())
                return;

            StubbleEntry entry;
            STBLVault.StringSource source = mStubbleEntries.TryGetValue(GetRowKey(e.RowIndex), out entry);
            if (source == STBLVault.StringSource.None && e.ColumnIndex >= 0)
                return;

            if (e.ColumnIndex < 0)
            {
                if ((e.PaintParts & DataGridViewPaintParts.ContentBackground) != DataGridViewPaintParts.ContentBackground)
                    return;

                if (source != STBLVault.StringSource.Modified)
                    return;

                e.Paint(e.ClipBounds, e.PaintParts);

                string mark = null;
                if (entry == null)
                    mark = "+?";
                else if (entry.Source.SourceTable == DBPFReference.MinValue)
                    mark = "+";
                else if (source == STBLVault.StringSource.Modified)
                    mark = "!";

                Font fontstyle = new Font(e.CellStyle.Font, FontStyle.Bold);

                StringFormat fmt = new StringFormat();
                fmt.Alignment = StringAlignment.Center;
                fmt.LineAlignment = StringAlignment.Center;
                fmt.FormatFlags = StringFormatFlags.NoWrap;

                Rectangle markbounds = new Rectangle(e.CellBounds.X + e.CellBounds.Width / 2, e.CellBounds.Y, e.CellBounds.Width / 2, e.CellBounds.Height);
                Brush markbrush;
                if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                    markbrush = new SolidBrush(Color.Pink);
                else
                    markbrush = new SolidBrush(Color.Red);

                e.Graphics.DrawString(mark, fontstyle, markbrush, markbounds, fmt);

                e.Handled = true;
                return;
            }

            if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                return;

            if ((e.PaintParts & DataGridViewPaintParts.ContentBackground) == DataGridViewPaintParts.ContentBackground)
            {
                Color backcolor = e.CellStyle.BackColor;
                Color forecolor = e.CellStyle.ForeColor;
                if (source == STBLVault.StringSource.Game)
                    backcolor = Color.FromArgb(backcolor.R ^ 0x20, backcolor.G ^ 0x20, backcolor.B ^ 0x20);
                if (source == STBLVault.StringSource.Loaded)
                    forecolor = Color.FromArgb(forecolor.R ^ 0x20, forecolor.G ^ 0x20, forecolor.B ^ 0x20);
                e.Graphics.FillRectangle(new SolidBrush(backcolor), e.CellBounds);
                e.Paint(e.ClipBounds, e.PaintParts & ~(DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.Background));
                e.Handled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = CheckUnsaved();
        }

        private bool CheckUnsaved()
        {
            if (mStubbleEntries.IsDirty)
            {
                DialogResult result = MessageBox.Show(this, "If you continue you will lose your un-saved data.  Do you wish to save first?", "Unsaved Data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Cancel:
                        return true;
                    case DialogResult.Yes:
                        saveToolStripMenuItem.PerformClick();
                        break;
                }
            }
            return false;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = CopyText(false);
            if (!string.IsNullOrEmpty(text))
                CopySimpleString(text);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = CopyText(true);
            if (!string.IsNullOrEmpty(text))
                CopySimpleString(text);
            if (mSearchDialog != null)
                mSearchDialog.ResetSearch();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            string text = data.GetData(DataFormats.UnicodeText, true) as string;
            if (text == null)
                return;

            PasteText(text);
            if (mSearchDialog != null)
                mSearchDialog.ResetSearch();
        }

        private void PasteText(string text)
        {
            Control c = ActiveControl;
            ContainerControl cc = c as ContainerControl;

            while (cc != null)
            {
                c = cc.ActiveControl;
                cc = c as ContainerControl;
            }
            TextBoxBase tb = c as TextBoxBase;
            if (tb != null)
            {
                if (!tb.ReadOnly && tb.Enabled)
                    tb.SelectedText = text;
                return;
            }
            DataGridView dgv = c as DataGridView;
            if (dgv != null)
            {
                if (dgv.EditingControl != null)
                    return;
                if (dgv.BeginEdit(true))
                {
                    if (dgv.EditingControl != null)
                        PasteText(text);
                }
                return;
            }
        }

        private string CopyText(bool cut)
        {
            Control c = ActiveControl;
            ContainerControl cc = c as ContainerControl;

            while (cc != null)
            {
                c = cc.ActiveControl;
                cc = c as ContainerControl;
            }
            TextBoxBase tb = c as TextBoxBase;
            if (tb != null)
            {
                if (!tb.ReadOnly && tb.Enabled)
                {
                    string ret = tb.SelectedText;
                    if (cut)
                        tb.SelectedText = string.Empty;
                    return ret;
                }
                return null;
            }
            DataGridView dgv = c as DataGridView;
            if (dgv != null)
            {
                if (dgv.EditingControl != null)
                    return null;
                if (cut)
                {
                    if (dgv.BeginEdit(true))
                    {
                        if (dgv.EditingControl != null)
                        {
                            return CopyText(cut);
                        }
                    }
                }
                else if (object.ReferenceEquals(dgv, dataGridView1))
                {
                    if (dgv.SelectedRows.Count > 0)
                    {
                        if (dgv.CurrentCellAddress.Y >= 0 && dgv.CurrentCellAddress.Y < SelectedNodeKeyCount())
                        {
                            UInt64 id = GetRowKey(dgv.CurrentCellAddress.Y);
                            string keyname;
                            if (STBLVault.LookupKey(id, out keyname))
                                keyname = string.Format("\"{0}\"", keyname.Replace("\\", "\\\\").Replace("\"", "\\\""));
                            else
                                keyname = string.Format("0x{0:16X}", id);
                            string value = GetValueOf(dgv.CurrentCellAddress.Y);
                            string csvtext = string.Format("{0},\"{1}\"", keyname, value.Replace(@"\", @"\\").Replace("\"", "\\\""));
                            DataObject obj = new DataObject();
                            obj.SetText(csvtext, TextDataFormat.UnicodeText);
                            obj.SetText(csvtext, TextDataFormat.CommaSeparatedValue);
                            Clipboard.SetDataObject(obj);
                        }
                    }
                }
                else
                {
                    string text = dgv.CurrentCell.Value.ToString();
                    CopySimpleString(text);
                }
                return null;
            }
            return null;
        }

        private void gameStringsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSearchDialog != null)
                mSearchDialog.ResetSearch();

            bool editing = modifiedStringsToolStripMenuItem.Checked;
            rootCategoryToolStripMenuItem.Enabled = editing;
            newStringToolStripMenuItem.Enabled = editing;
            subcategoryToolStripMenuItem.Enabled = editing;
            dataGridView1.AllowUserToAddRows = editing;
            cutToolStripMenuItem.Enabled = editing;
            pasteToolStripMenuItem.Enabled = editing;

            STBLVault.StringSource source = STBLVault.StringSource.None;
            if (modifiedStringsToolStripMenuItem.Checked)
                source |= STBLVault.StringSource.Modified;
            if (unmodifiedStringsToolStripMenuItem.Checked)
                source |= STBLVault.StringSource.Loaded;
            if (gameStringsToolStripMenuItem.Checked)
                source |= STBLVault.StringSource.Game;

            if (source != mStubbleEntries.CurrentSource)
            {
                mStubbleEntries.CurrentSource = source;
                RepopulateTree();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckUnsaved())
                return;

            PerformLoad(true);
        }

        private void PerformLoad(bool asnewfile)
        {
            if (openFileDialog1.ShowDialog(this) != DialogResult.OK)
                return;

            int loaded = 0;
            bool cleared = !asnewfile;
            try
            {
                Stream stream = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                using (stream)
                {
                    byte[] header = new byte[4];
                    stream.Read(header, 0, 4);
                    stream.Position = 0;
                    if (Encoding.ASCII.GetString(header) == "DBPF")
                    {
                        DBPFFile file = new DBPFFile(stream);
                        using (file)
                        {
                            foreach (DBPFIndexEntry entry in file.Index)
                            {
                                if (entry.Reference.Type == 0x220557DA)
                                {
                                    StubbleSource source = new StubbleSource();
                                    source.SourceFile = openFileDialog1.FileName;
                                    source.SourceTable = entry.Reference;

                                    if (!cleared)
                                    {
                                        cleared = true;
                                        mStubbleEntries.ClearUserData();
                                        mCurExporter = null;
                                    }

                                    DBPFDataStream filedata = file.Open(entry.Reference);
                                    using (filedata)
                                    {
                                        // Force file-at-once read
                                        filedata.GetData();
                                        loaded += mStubbleEntries.LoadEntries(STBLVault.StringSource.Loaded, source,
                                            (Languages)(entry.Reference.Instance >> 56),
                                            STBLVault.OverwriteMode.PriorityOrEqual,
                                            STBLVault.GetDictionaryLoader(filedata));
                                    }
                                }
                            }
                            if (loaded == 0)
                            {
                                MessageBox.Show(this, "No STBL entries found", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                return;
                            }
                            if (!unmodifiedStringsToolStripMenuItem.Checked)
                                unmodifiedStringsToolStripMenuItem.Checked = true;
                            else
                                RepopulateTree();
                        }
                    }
                    else
                    {
                        ImportExport.ImportCSV importopts = new STBLBrowser.ImportExport.ImportCSV();
                        if (importopts.ShowDialog(this) == DialogResult.OK)
                        {
                            StubbleSource source = new StubbleSource();
                            source.SourceFile = openFileDialog1.FileName;
                            loaded += mStubbleEntries.LoadEntries(STBLVault.StringSource.Loaded, source,
                                importopts.ImportLanguage,
                                STBLVault.OverwriteMode.All,
                                importopts.ImportStrings(this, stream));
                            if (loaded == 0)
                            {
                                MessageBox.Show(this, "No STBL entries found", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                return;
                            }
                            STBLVault.SyncKeyNameMap();
                            if (!unmodifiedStringsToolStripMenuItem.Checked)
                                unmodifiedStringsToolStripMenuItem.Checked = true;
                            else
                                RepopulateTree();
                        }
                    }
                }
                if (asnewfile)
                    Text = "Stubble String Table Editor - " + openFileDialog1.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error reading file: {0}", ex.Message), "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void findByIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode origselectednode = treeView1.SelectedNode;
            int origselectedindex = dataGridView1.CurrentCellAddress.Y;
            FindById fbi = new FindById();
            fbi.KeyNameNeeded += delegate(UInt64 key)
            {
                TreeNode node;
                int index;
                FindNodeForId(key, false, out node, out index);
                if (node != null)
                {
                    SelectNodeRow(node, index);
                }

                string ret;
                if (STBLVault.LookupKey(key, out ret))
                    return ret;
                return null;
            };
            if (fbi.ShowDialog() == DialogResult.OK)
            {
                TreeNode node;
                int index;
                FindNodeForId(fbi.KeyID, false, out node, out index);
                if (node == null)
                {
                    MessageBox.Show(this, "Key not found", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    SelectNodeRow(node, index);
                    origselectednode = null;
                }
            }

            if (origselectednode != null)
                SelectNodeRow(origselectednode, origselectedindex);
        }

        private void SelectNodeRow(TreeNode node, int index)
        {
            node.EnsureVisible();
            treeView1.SelectedNode = node;
            if (index < dataGridView1.RowCount && index >= 0)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[index].Cells[1];
                dataGridView1.Rows[index].Selected = true;
            }
        }

        private void fNVHashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FNV().Show();
        }

        TextSearch mSearchDialog;

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSearchDialog == null)
            {
                mSearchDialog = new TextSearch();
                mSearchDialog.KeyNameEnumeratorNeeded += new GetKeyDataEnumerator(mSearchDialog_KeyNameEnumeratorNeeded);
                mSearchDialog.KeyValueEnumeratorNeeded += new GetKeyDataEnumerator(mSearchDialog_KeyValueEnumeratorNeeded);
                mSearchDialog.MatchFound += new EventHandler<MatchFoundEventArgs>(mSearchDialog_MatchFound);
            }
            if (!mSearchDialog.Visible)
                mSearchDialog.Show(this);
            else
                mSearchDialog.Focus();
        }

        void mSearchDialog_MatchFound(object sender, MatchFoundEventArgs e)
        {
            TreeNode node;
            int index;
            FindNodeForId(e.Key, false, out node, out index);
            if (node != null)
            {
                SelectNodeRow(node, index);
                e.Handled = true;
            }
        }

        IEnumerable<KeyValuePair<ulong, string>> mSearchDialog_KeyValueEnumeratorNeeded()
        {
            return mStubbleEntries.GetEntriesForLanguage(mStubbleEntries.CurrentLanguage, STBLVault.StringSource.Display);
        }

        IEnumerable<KeyValuePair<ulong, string>> mSearchDialog_KeyNameEnumeratorNeeded()
        {
            return STBLVault.KnownKeys;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (mSearchDialog != null)
            {
                using (mSearchDialog)
                {
                    if (mSearchDialog.Visible)
                        mSearchDialog.Close();
                }
            }
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSearchDialog != null)
                mSearchDialog.DoFindNext();
            else
                findToolStripMenuItem.PerformClick();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckUnsaved())
                return;

            Text = "Stubble String Table Editor";
            mStubbleEntries.ClearUserData();
            RepopulateTree();
        }

        private void newStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewString ns = new NewString();
            if (ns.ShowDialog(this) == DialogResult.OK)
            {
                UInt64 keyid = ns.KeyId;
                string keyname = ns.KeyName;
                if (keyname != null && STBLVault.LookupKey(keyid) == null)
                {
                    STBLVault.AddNewKey(keyname);
                    STBLVault.SyncKeyNameMap();
                }

                mStubbleEntries.SetEntry(keyid, ns.Value);

                TreeNode node;
                int index;
                FindNodeForId(ns.KeyId, true, out node, out index);
                node.EnsureVisible();
                SelectNodeRow(node, index);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mCurExporter == null)
            {
                saveAsToolStripMenuItem.PerformClick();
            }
            else
            {
                mCurExporter.Export(this);
                dataGridView1.Invalidate();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            PerformLoad(false);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
    }
}
