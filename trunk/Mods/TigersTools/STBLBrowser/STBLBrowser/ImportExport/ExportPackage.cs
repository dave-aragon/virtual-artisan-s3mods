using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DBPF;
using System.IO;

namespace STBLBrowser.ImportExport
{
    internal partial class ExportPackage : Form, IExport
    {
        public ExportPackage()
        {
            InitializeComponent();
        }

        private void AnalyzePackage()
        {
            mInstance = UInt64.MaxValue;
            bool haveiid = false;
            bool havemultipleiid = false;
            mHighestPriority = UInt32.MaxValue;
            Dictionary<string, bool> sourcelist = new Dictionary<string, bool>();
            for (int loop = 0; loop < (int)Languages.Language_Count; loop++)
            {
                foreach (KeyValuePair<UInt64, StubbleEntry> entry in mData.GetEntriesWithSourceForLanguage((Languages)loop, STBLVault.StringSource.Modified | STBLVault.StringSource.Loaded))
                {
                    if (entry.Value.Source.SourceTable == DBPF.DBPFReference.MinValue)
                        continue;
                    mHighestPriority = Math.Min(mHighestPriority, (UInt32)entry.Value.Source.SourceTable.Instance);
                    if (!havemultipleiid)
                    {
                        UInt64 thisinstance = entry.Value.Source.SourceTable.Instance & ~(0xffUL << 56);
                        if (haveiid)
                        {
                            if (mInstance != thisinstance)
                            {
                                havemultipleiid = true;
                                mInstance = UInt64.MaxValue;
                            }
                        }
                        else
                        {
                            mInstance = thisinstance;
                        }
                    }
                    if (!string.IsNullOrEmpty(entry.Value.Source.SourceFile))
                        sourcelist[entry.Value.Source.SourceFile] = true;
                }
            }
            if (mInstance != UInt64.MaxValue)
            {
                foreach (string origpackage in STBLVault.GetAllPackageFilenames())
                {
                    if (sourcelist.ContainsKey(origpackage))
                    {
                        mInstance = UInt64.MaxValue;
                        break;
                    }
                }
            }
        }

        private STBLVault mData;
        private UInt32 mHighestPriority;
        private UInt64 mInstance;
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
                MessageBox.Show(owner, "No loaded or modified data to save", "No data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            AnalyzePackage();
            if (mInstance != UInt64.MaxValue)
            {
                textBox1.Text = mInstance.ToString("X14");
                checkBox5.Checked = true;
            }
            else
                checkBox5.Checked = false;

            UInt64 existing;

            string origfilename = null;
            bool cleanup = false;
            try
            {
                try
                {
                    Stream outfile;
                    OpenOrCreate(filename, out origfilename, out cleanup, out outfile);

                    using (outfile)
                    {
                        DBPFFile dbfile;
                        if (origfilename == null)
                        {
                            dbfile = new DBPFFile();
                        }
                        else
                        {
                            dbfile = new DBPFFile(origfilename);
                        }

                        using (dbfile)
                        {
                            if (dbfile.IsProtected)
                            {
                                MessageBox.Show(owner, "Saving to protected DB files not allowed", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return false;
                            }
                            // Find any existing STBL instances
                            existing = UInt64.MaxValue;
                            foreach (DBPFIndexEntry entry in dbfile.Index)
                            {
                                if (entry.Reference.Type == 0x220557DA)
                                {
                                    if (existing == UInt64.MaxValue || (UInt32)entry.Reference.Instance < (UInt32)existing)
                                        existing = entry.Reference.Instance & ~(0xFFUL << 56);
                                }
                            }

                            // Disable options that make no sense if there is no entry to replace
                            if (existing != UInt64.MaxValue)
                            {
                                radioButton2.Checked = true;
                                radioButton2.Enabled = true;
                                radioButton3.Enabled = true;
                            }
                            else
                            {
                                radioButton1.Checked = true;
                                radioButton2.Enabled = false;
                                radioButton3.Enabled = false;
                            }

                            if (ShowDialog(owner) != DialogResult.OK)
                                return false;

                            // Figure out what instance to write
                            mInstance = 0;
                            if (radioButton1.Checked)
                            {
                                // Check if instance specified
                                if (!(checkBox5.Checked && UInt64.TryParse(textBox1.Text, out mInstance)))
                                {
                                    // Generate a new instance, making the priority close to the high end of usable values
                                    byte[] ranval = Guid.NewGuid().ToByteArray();
                                    mInstance = (UInt64)(BitConverter.ToUInt32(ranval, 1) & 0xFFFFFF) << 32;
                                    UInt64 priority = mHighestPriority;
                                    if (priority == UInt32.MaxValue)
                                        priority++;
                                    priority /= 8;
                                    if (priority == 0)
                                        priority = 1;
                                    UInt64 chosenpriority = BitConverter.ToUInt64(ranval, 6) % priority + priority * 7;
                                    mInstance |= chosenpriority;
                                }
                            }
                            else
                                // Replace an existing entry
                                mInstance = existing;

                            Dictionary<DBPFReference, KeyValuePair<DBPFIndexEntry, byte[]>> toreplace = new Dictionary<DBPFReference, KeyValuePair<DBPFIndexEntry, byte[]>>();
                            List<KeyValuePair<DBPFIndexEntry, byte[]>> toadd = new List<KeyValuePair<DBPFIndexEntry, byte[]>>();

                            GatherSaveData(dbfile, toreplace, toadd);

                            // Write the new file
                            WriteAndCommit(filename, outfile, dbfile, toreplace, toadd);
                        }
                    }
                    if (origfilename != null)
                        File.Delete(origfilename);
                    cleanup = false;
                    mFilename = filename;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(owner, string.Format("Error saving package:\n{0}", ex.Message), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                PerformCleanup(filename, origfilename, cleanup);
            }
            return false;
        }

        public void Export(IWin32Window owner)
        {
            string origfilename = null;
            bool cleanup = false;
            try
            {
                try
                {
                    Stream outfile;
                    OpenOrCreate(mFilename, out origfilename, out cleanup, out outfile);

                    using (outfile)
                    {
                        DBPFFile dbfile;
                        if (origfilename == null)
                        {
                            dbfile = new DBPFFile();
                        }
                        else
                        {
                            dbfile = new DBPFFile(origfilename);
                        }

                        using (dbfile)
                        {
                            if (dbfile.IsProtected)
                            {
                                MessageBox.Show(owner, "Saving to protected DB files not allowed", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }

                            Dictionary<DBPFReference, KeyValuePair<DBPFIndexEntry, byte[]>> toreplace = new Dictionary<DBPFReference, KeyValuePair<DBPFIndexEntry, byte[]>>();
                            List<KeyValuePair<DBPFIndexEntry, byte[]>> toadd = new List<KeyValuePair<DBPFIndexEntry, byte[]>>();

                            GatherSaveData(dbfile, toreplace, toadd);

                            // Write the new file
                            WriteAndCommit(mFilename, outfile, dbfile, toreplace, toadd);
                        }
                    }
                    if (origfilename != null)
                        File.Delete(origfilename);
                    cleanup = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(owner, string.Format("Error saving package:\n{0}", ex.Message), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                PerformCleanup(mFilename, origfilename, cleanup);
            }
        }

        private static void PerformCleanup(string filename, string origfilename, bool cleanup)
        {
            if (cleanup)
            {
                if (origfilename != null)
                {
                    if (File.Exists(origfilename))
                    {
                        try
                        {
                            if (File.Exists(filename))
                                File.Delete(filename);
                            File.Move(origfilename, filename);
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void OpenOrCreate(string filename, out string origfilename, out bool cleanup, out Stream outfile)
        {

            // Check if the file exists
            try
            {
                outfile = new FileStream(filename, FileMode.CreateNew);
                origfilename = null;
                cleanup = true;
            }
            catch (System.IO.IOException)
            {
                // If it exists, try to move it aside, no point going on if it can't be modified at all
                string filenameonly = Path.GetFileNameWithoutExtension(filename);
                string extension = Path.GetExtension(filename);
                string tmpfilename = Path.Combine(Path.GetDirectoryName(filename), filenameonly + "-saving" + extension);
                if (File.Exists(tmpfilename))
                    File.Delete(tmpfilename);
                File.Move(filename, tmpfilename);
                origfilename = tmpfilename;
                outfile = new FileStream(filename, FileMode.CreateNew);
                cleanup = true;
            }
        }

        private void WriteAndCommit(string filename, Stream outfile, DBPFFile dbfile, Dictionary<DBPFReference, KeyValuePair<DBPFIndexEntry, byte[]>> toreplace, List<KeyValuePair<DBPFIndexEntry, byte[]>> toadd)
        {
            dbfile.Write(outfile, delegate(ref DBPFIndexEntry entry, ref byte[] data)
            {
                KeyValuePair<DBPFIndexEntry, byte[]> replacement;
                if (toreplace.TryGetValue(entry.Reference, out replacement))
                {
                    entry = replacement.Key;
                    data = replacement.Value;
                }
            }, toadd);

            foreach (DBPFReference commitref in toreplace.Keys)
            {
                StubbleSource source = new StubbleSource();
                source.SourceFile = filename;
                source.SourceTable = commitref;
                mData.CommitModifiedData((Languages)(commitref.Instance >> 56), source);
            }
            foreach (KeyValuePair<DBPFIndexEntry, byte[]> commitentry in toadd)
            {
                StubbleSource source = new StubbleSource();
                source.SourceFile = filename;
                source.SourceTable = commitentry.Key.Reference;
                mData.CommitModifiedData((Languages)(commitentry.Key.Reference.Instance >> 56), source);
            }
        }

        private void GatherSaveData(DBPFFile dbfile, Dictionary<DBPFReference, KeyValuePair<DBPFIndexEntry, byte[]>> toreplace, List<KeyValuePair<DBPFIndexEntry, byte[]>> toadd)
        {
            STBLWriter copysource = null;
            // Copy the reference language entries
            if (checkBox2.Checked)
            {
                copysource = new STBLWriter();
                foreach (KeyValuePair<UInt64, string> entry in mData.GetEntriesForLanguage((Languages)comboBox1.Items[comboBox1.SelectedIndex], STBLVault.StringSource.Modified | STBLVault.StringSource.Loaded))
                {
                    copysource.Add(entry.Key, entry.Value);
                }
            }

            for (int loop = 0; loop < (int)Languages.Language_Count; loop++)
            {
                // Skip languages there is nothing to write for
                if (!mData.HasEntriesForLanguage((Languages)loop, STBLVault.StringSource.Loaded | STBLVault.StringSource.Modified)
                    && copysource == null)
                    continue;

                STBLWriter newentry;
                if (copysource != null)
                    newentry = copysource.Copy();
                else
                    newentry = new STBLWriter();
                newentry.Instance = mInstance;
                newentry.Language = (Languages)loop;

                // Overwrite or underwrite the user defined entries
                if (copysource != null && checkBox1.Checked)
                    foreach (KeyValuePair<UInt64, string> entry in mData.GetEntriesForLanguage((Languages)loop, STBLVault.StringSource.Modified | STBLVault.StringSource.Loaded))
                        newentry.AddIfNotExists(entry.Key, entry.Value);
                else
                    foreach (KeyValuePair<UInt64, string> entry in mData.GetEntriesForLanguage((Languages)loop, STBLVault.StringSource.Modified | STBLVault.StringSource.Loaded))
                        newentry.Add(entry.Key, entry.Value);

                // Load in the existing entry if requested
                if (radioButton2.Checked)
                {
                    DBPFReference oldref = new DBPFReference(0x220557DA, 0, mInstance | ((UInt64)loop << 56));
                    if (dbfile.Index.ContainsKey(oldref))
                    {
                        DBPFDataStream oldfile = dbfile.Open(oldref);
                        using (oldfile)
                        {
                            oldfile.GetData();
                            if (checkBox3.Checked)
                            {
                                // Merge overwriting old with new
                                foreach (KeyValuePair<UInt64, string> oldentry in STBLVault.GetDictionaryLoader(oldfile))
                                {
                                    newentry.AddIfNotExists(oldentry.Key, oldentry.Value);
                                }
                            }
                            else
                            {
                                // Merge overwriting new with old
                                foreach (KeyValuePair<UInt64, string> oldentry in STBLVault.GetDictionaryLoader(oldfile))
                                {
                                    newentry.Add(oldentry.Key, oldentry.Value);
                                }
                            }
                        }
                    }
                }

                // Create the STBL format data blob
                KeyValuePair<DBPFIndexEntry, byte[]> dbentry = newentry.Export();
                if (dbfile.Index.ContainsKey(dbentry.Key.Reference))
                    toreplace[dbentry.Key.Reference] = dbentry;
                else
                    toadd.Add(dbentry);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox5.Enabled = radioButton1.Checked;
            checkBox1.Enabled = radioButton1.Checked && checkBox5.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox3.Enabled = radioButton2.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = checkBox5.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = checkBox1.Enabled = checkBox2.Checked;
        }
    }
}
