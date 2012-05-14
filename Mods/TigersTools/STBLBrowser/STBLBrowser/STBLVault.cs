using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using DBPF;

using Microsoft.Win32;

namespace STBLBrowser
{
    class STBLVault
    {
        #region Static key/name map members
        public static void InitializeKeyNameMap(string filename)
        {
            StreamReader rdr = new StreamReader(filename);
            using (rdr)
            {
                Stack<string> activenodes = new Stack<string>();
                string line = rdr.ReadLine();
                while (line != null)
                {
                    if (line != string.Empty)
                        sKeyNames[FNV64(line)] = line;
                    line = rdr.ReadLine();
                }
            }
        }

        public static string UserMapFilename
        {
            get
            {
                return Path.Combine(System.Windows.Forms.Application.StartupPath, "UserSTBL.txt");
            }
        }

        public static void SyncKeyNameMap()
        {
            try
            {
                if (sNewKeys == null || sNewKeys.Count == 0)
                    return;

                StreamWriter sw = new StreamWriter(UserMapFilename, true, Encoding.UTF8);
                using (sw)
                {
                    foreach (string key in sNewKeys)
                    {
                        sw.WriteLine(key);
                    }
                    sw.Flush();
                }
                sNewKeys.Clear();
            }
            catch
            { }
        }

        public static string LookupKey(UInt64 key)
        {
            string ret;
            if (sKeyNames.TryGetValue(key, out ret))
                return ret;
            return null;
        }

        public static bool LookupKey(UInt64 key, out string name)
        {
            return sKeyNames.TryGetValue(key, out name);
        }

        public static bool AddNewKey(string name)
        {
            UInt64 key = FNV64(name);
            if (!sKeyNames.ContainsKey(key))
            {
                sKeyNames[key] = name;
                sNewKeys.Add(name);
                return true;
            }
            return false;
        }

        static public UInt64 FNV64(string val)
        {
            UInt64 ret = 0xcbf29ce484222325;
            for (int loop = 0; loop < val.Length; loop++)
            {
                ret *= 0x00000100000001B3;
                ret ^= (UInt64)char.ToLower(val[loop]);
            }
            return ret;
        }

        static public UInt32 FNV32(string val)
        {
            UInt32 ret = 0x811c9dc5u;
            for (int loop = 0; loop < val.Length; loop++)
            {
                ret *= 0x01000193u;
                ret ^= (UInt32)char.ToLower(val[loop]);
            }
            return ret;
        }

        static public UInt32 FNV24(string val)
        {
            UInt32 ret = FNV32(val);
            return (ret & 0xffffff) ^ ((ret >> 24) & 0xff);
        }

        public static Dictionary<UInt64, string> KnownKeys
        {
            get
            {
                return sKeyNames;
            }
        }

        private static Dictionary<UInt64, string> sKeyNames = new Dictionary<ulong, string>();
        private static List<string> sNewKeys = new List<string>();
        #endregion
        #region Static helper functions
        public static IEnumerable<KeyValuePair<UInt64, string>> GetDictionaryLoader(Stream source)
        {
            BinaryReader br = new BinaryReader(source);
            source.Position = 7;
            int count = br.ReadInt32();
            source.Position += 6;
            for (int loop = 0; loop < count; loop++)
            {
                UInt64 stringid = br.ReadUInt64();
                int stringlen = br.ReadInt32();
                string value = Encoding.Unicode.GetString(br.ReadBytes(stringlen * 2));
                yield return new KeyValuePair<UInt64, string>(stringid, value);
            }
        }

        private static Dictionary<Languages, List<KeyValuePair<string, DBPFFile>>> sFilesByLanguage;
        private IEnumerable<KeyValuePair<string, DBPFFile>> FilesForLanguage
        {
            get
            {
                if (sFilesByLanguage == null)
                {
                    sFilesByLanguage = new Dictionary<Languages, List<KeyValuePair<string, DBPFFile>>>();

                    Dictionary<Languages, bool> foundlanguages = new Dictionary<Languages, bool>();
                    foreach (string filename in GetAllPackageFilenames())
                    {
                        foundlanguages.Clear();
                        try
                        {
                            DBPFFile file = new DBPFFile(filename);
                            try
                            {
                                foreach (DBPFIndexEntry entry in file.Index)
                                {
                                    if (entry.Reference.Type == 0x220557da)
                                    {
                                        Languages lang = (Languages)((entry.Reference.Instance >> 56) & 0xff);
                                        if (!foundlanguages.ContainsKey(lang))
                                        {
                                            KeyValuePair<string, DBPFFile> newfile = new KeyValuePair<string, DBPFFile>(filename, file);
                                            List<KeyValuePair<string, DBPFFile>> foundfiles;
                                            if (!sFilesByLanguage.TryGetValue(lang, out foundfiles))
                                            {
                                                foundfiles = new List<KeyValuePair<string, DBPFFile>>();
                                                sFilesByLanguage[lang] = foundfiles;
                                            }
                                            foundlanguages[lang] = true;
                                            foundfiles.Add(newfile);
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                if (foundlanguages.Count == 0)
                                    file.Dispose();
                            }
                        }
                        catch { }
                    }
                }

                List<KeyValuePair<string, DBPFFile>> ret;
                if (!sFilesByLanguage.TryGetValue(CurrentLanguage, out ret))
                {
                    ret = new List<KeyValuePair<string, DBPFFile>>();
                    sFilesByLanguage[CurrentLanguage] = ret;
                }
                return ret;
            }
        }

        public static string GetGameExeFileName()
        {
            string basepath = GetInstallPath();
            string exeName = Path.Combine(basepath, "Game", "Bin", "TS3W.exe");
            return exeName;
        }

        private static string sDocPath = null;
        public static IEnumerable<string> GetAllPackageFilenames()
        {
            string basepath = GetInstallPath();
            foreach (string filename in Directory.GetFiles(Path.Combine(basepath, Path.Combine("GameData", Path.Combine("Shared", "Packages"))), "Full*.package"))
            {
                yield return filename;
            }
            foreach (string filename in Directory.GetFiles(Path.Combine(basepath, Path.Combine("GameData", Path.Combine("Shared", "Packages"))), "Delta*.package"))
            {
                yield return filename;
            }

            //foreach (string filename in Directory.GetFiles(@"C:\Documents and Settings\Tiger\My Documents\Electronic Arts\The Sims 3\InstalledWorlds", "*.world"))
            //{
            //    yield return filename;
            //}
            //foreach (string filename in Directory.GetFiles(@"C:\Program Files\Electronic Arts\The Sims 3\GameData\Shared\NonPackaged\Worlds", "*.world"))
            //{
            //    yield return filename;
            //}
            //foreach (string filename in Directory.GetFiles(@"C:\Program Files\Electronic Arts\The Sims 3\Game\bin\Jazz", "*.package"))
            //{
            //    yield return filename;
            //}
            //foreach (string filename in Directory.GetFiles(@"C:\Program Files\Electronic Arts\The Sims 3\Game\bin\UI", "*.package"))
            //{
            //    yield return filename;
            //}
            //foreach (string filename in Directory.GetFiles(@"C:\Program Files\Electronic Arts\The Sims 3\Game\bin\Gameplay", "*.package"))
            //{
            //    yield return filename;
            //}
            /*
            foreach (string filename in Directory.GetFiles(@"C:\Documents and Settings\Tiger\My Documents\Electronic Arts\The Sims 3\DCBackup", "*.package"))
            {
                yield return filename;
            }
             */
            if (sDocPath == null)
            {
                string searchpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Electronic Arts");
                if (Directory.Exists(searchpath))
                {
                    if (Directory.Exists(Path.Combine(Path.Combine(searchpath, "The Sims 3"), "DCCache")))
                        sDocPath = Path.Combine(Path.Combine(searchpath, "The Sims 3"), "DCCache");
                    else
                    {
                        foreach (string name in Directory.GetDirectories(searchpath))
                        {
                            if (Directory.Exists(Path.Combine(name, "DCCache")))
                            {
                                sDocPath = Path.Combine(name, "DCCache");
                                break;
                            }
                        }
                        if (sDocPath == null)
                            yield break;
                    }
                }
                else
                    yield break;
            }
            foreach (string filename in Directory.GetFiles(sDocPath, "*.ebc"))
            {
                yield return filename;
            }
        }

        private static string sTS3Path;
        public static string GetInstallPath()
        {
            if (sTS3Path != null)
                return sTS3Path;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Sims\The Sims 3");
            if (key != null)
            {
                string basepath = key.GetValue("Install Dir") as string;
                return basepath;
            }
            else
            {
                System.Windows.Forms.FolderBrowserDialog fbr = new System.Windows.Forms.FolderBrowserDialog();
                if (fbr.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    System.Windows.Forms.Application.Exit();
                }
                return sTS3Path = fbr.SelectedPath;
            }
        }

        #endregion

        [Flags]
        public enum StringSource
        {
            None = 0,
            Game = 1,
            Loaded = 2,
            Modified = 4,
            Display = 8,
            All = 7
        }

        private Languages mCurrentLanguage;
        public Languages CurrentLanguage
        {
            get
            {
                return mCurrentLanguage;
            }
        }
        public void SetLanguage(Languages newlang)
        {
            mCurrentLanguage = newlang;
        }

        private StringSource mCurrentSource = StringSource.All;
        public StringSource CurrentSource
        {
            get
            {
                return mCurrentSource;
            }
            set
            {
                mCurrentSource = value & StringSource.All;
            }
        }

        public bool IsDirty
        {
            get
            {
                foreach (Dictionary<UInt64, StubbleEntry> entrylist in mModifiedEntries.Values)
                    if (entrylist.Count > 0)
                        return true;
                return false;
            }
        }

        public enum OverwriteMode
        {
            None,
            Priority,
            PriorityOrEqual,
            All
        }

        public int LoadEntries(StringSource target, StubbleSource source, Languages lang, OverwriteMode overwrite, IEnumerable<KeyValuePair<UInt64, string>> entries)
        {
            if (source.SourceTable == DBPFReference.MinValue && overwrite != OverwriteMode.All && overwrite != OverwriteMode.None)
                throw new InvalidOperationException();

            Dictionary<UInt64, StubbleEntry> targetlist;
            if (target == StringSource.Modified)
            {
                if (!mModifiedEntries.TryGetValue(lang, out targetlist))
                {
                    targetlist = new Dictionary<ulong, StubbleEntry>();
                    mModifiedEntries[lang] = targetlist;
                }
            }
            else if (target == StringSource.Loaded)
            {
                if (!mLoadedEntries.TryGetValue(lang, out targetlist))
                {
                    targetlist = new Dictionary<ulong, StubbleEntry>();
                    mLoadedEntries[lang] = targetlist;
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            int added = 0;
            foreach (KeyValuePair<UInt64, string> entry in entries)
            {
                if (overwrite != OverwriteMode.All)
                {
                    StubbleEntry oldentry;
                    if (targetlist.TryGetValue(entry.Key, out oldentry))
                    {
                        if (overwrite == OverwriteMode.None)
                            continue;

                        if (overwrite == OverwriteMode.Priority)
                        {
                            if ((UInt32)source.SourceTable.Instance >= (UInt32)oldentry.Source.SourceTable.Instance)
                                continue;
                        }
                        else
                        {
                            if (source.SourceTable != oldentry.Source.SourceTable
                                && (UInt32)source.SourceTable.Instance >= (UInt32)oldentry.Source.SourceTable.Instance)
                                continue;
                        }
                    }
                }

                StubbleEntry newentry = new StubbleEntry();
                newentry.Value = entry.Value;
                newentry.Source = source;
                targetlist[entry.Key] = newentry;
                added++;
            }
            return added;
        }

        public void SetEntry(UInt64 key, string value)
        {
            StubbleEntry stblentry;
            StringSource source = TryGetValue(key, StringSource.All, out stblentry);
            switch (source)
            {
                case StringSource.None:
                    stblentry = new StubbleEntry();
                    stblentry.Source = new StubbleSource();
                    break;
                case StringSource.Modified:
                    stblentry.Value = value;
                    return;
                default:
                    StubbleEntry origentry = stblentry;
                    stblentry = new StubbleEntry();
                    stblentry.Source = new StubbleSource();
                    stblentry.Source.SourceFile = origentry.Source.SourceFile;
                    stblentry.Source.SourceTable = origentry.Source.SourceTable;
                    break;
            }
            stblentry.Value = value;
            Dictionary<UInt64, StubbleEntry> langlist;
            if (!mModifiedEntries.TryGetValue(CurrentLanguage, out langlist))
            {
                langlist = new Dictionary<ulong, StubbleEntry>();
                mModifiedEntries[CurrentLanguage] = langlist;
            }
            langlist[key] = stblentry;
        }

        public void RemoveEntry(UInt64 key, StringSource source, bool justone)
        {
            if (source == StringSource.Display)
                source = mCurrentSource;

            Dictionary<UInt64, StubbleEntry> entrylist;
            if ((source & StringSource.Modified) != 0)
            {
                if (mModifiedEntries.TryGetValue(CurrentLanguage, out entrylist)
                    && entrylist.ContainsKey(key))
                {
                    entrylist.Remove(key);
                    if (entrylist.Count == 0)
                        mModifiedEntries.Remove(CurrentLanguage);
                    if (justone)
                        return;
                }
            }
            if ((source & StringSource.Loaded) != 0)
            {
                if (mLoadedEntries.TryGetValue(CurrentLanguage, out entrylist)
                    && entrylist.ContainsKey(key))
                {
                    entrylist.Remove(key);
                    if (entrylist.Count == 0)
                        mLoadedEntries.Remove(CurrentLanguage);
                }
            }
        }

        public StringSource TryGetValue(UInt64 key, out StubbleEntry value)
        {
            return TryGetValue(key, StringSource.Display, out value);
        }
        public StringSource TryGetValue(UInt64 key, StringSource source, out StubbleEntry value)
        {
            if (source == StringSource.Display)
                source = mCurrentSource;

            if ((source & StringSource.Modified) != 0 && TryGetValue(mModifiedEntries, key, out value))
                return StringSource.Modified;
            if ((source & StringSource.Loaded) != 0 && TryGetValue(mLoadedEntries, key, out value))
                return StringSource.Loaded;
            if ((source & StringSource.Game) != 0 && GlobalEntries.TryGetValue(key, out value))
                return StringSource.Game;
            value = null;
            return StringSource.None;
        }

        public StringSource TryGetValue(UInt64 key, StringSource source, out string value)
        {
            StubbleEntry entry;
            StringSource ret = TryGetValue(key, source, out entry);
            if (ret != StringSource.None)
            {
                value = entry.Value;
                return ret;
            }
            value = null;
            return ret;
        }

        public StringSource TryGetValue(UInt64 key, out string value)
        {
            return TryGetValue(key, StringSource.Display, out value);
        }

        public void ClearUserData()
        {
            mLoadedEntries.Clear();
            mModifiedEntries.Clear();
        }

        public bool LangHasData(Languages lang, StringSource source)
        {
            if (source == StringSource.Display)
                source = mCurrentSource;

            if ((source & StringSource.Modified) != 0)
            {
                Dictionary<UInt64, StubbleEntry> entrylist;
                if (mModifiedEntries.TryGetValue(lang, out entrylist) && entrylist.Count > 0)
                    return true;
            }
            if ((source & StringSource.Loaded) != 0)
            {
                Dictionary<UInt64, StubbleEntry> entrylist;
                if (mLoadedEntries.TryGetValue(lang, out entrylist) && entrylist.Count > 0)
                    return true;
            }
            return false;
        }

        public IEnumerable<KeyValuePair<UInt64, string>> GetEntriesForLanguage(Languages lang, StringSource source)
        {
            if (source == StringSource.Display)
                source = mCurrentSource;

            Dictionary<UInt64, StubbleEntry> modentrylist = null;
            if ((source & StringSource.Modified) != 0)
            {
                if (mModifiedEntries.TryGetValue(lang, out modentrylist))
                {
                    foreach (KeyValuePair<UInt64, StubbleEntry> entry in modentrylist)
                    {
                        yield return new KeyValuePair<UInt64, string>(entry.Key, entry.Value.Value);
                    }
                }
            }
            Dictionary<UInt64, StubbleEntry> loadentrylist = null;
            if ((source & StringSource.Loaded) != 0)
            {
                if (mLoadedEntries.TryGetValue(lang, out loadentrylist))
                {
                    foreach (KeyValuePair<UInt64, StubbleEntry> entry in loadentrylist)
                    {
                        if (modentrylist != null && modentrylist.ContainsKey(entry.Key))
                            continue;
                        yield return new KeyValuePair<UInt64, string>(entry.Key, entry.Value.Value);
                    }
                }
            }
            if ((source & StringSource.Game) != 0)
            {
                foreach (KeyValuePair<UInt64, StubbleEntry> entry in GlobalEntries)
                {
                    if (modentrylist != null && modentrylist.ContainsKey(entry.Key))
                        continue;
                    if (loadentrylist != null && loadentrylist.ContainsKey(entry.Key))
                        continue;
                    yield return new KeyValuePair<UInt64, string>(entry.Key, entry.Value.Value);
                }
            }
        }

        public IEnumerable<KeyValuePair<UInt64, StubbleEntry>> GetEntriesWithSourceForLanguage(Languages lang, StringSource source)
        {
            if (source == StringSource.Display)
                source = mCurrentSource;

            Dictionary<UInt64, StubbleEntry> modentrylist = null;
            if ((source & StringSource.Modified) != 0)
            {
                if (mModifiedEntries.TryGetValue(lang, out modentrylist))
                {
                    foreach (KeyValuePair<UInt64, StubbleEntry> entry in modentrylist)
                    {
                        yield return entry;
                    }
                }
            }
            Dictionary<UInt64, StubbleEntry> loadentrylist = null;
            if ((source & StringSource.Loaded) != 0)
            {
                if (mLoadedEntries.TryGetValue(lang, out loadentrylist))
                {
                    foreach (KeyValuePair<UInt64, StubbleEntry> entry in loadentrylist)
                    {
                        if (modentrylist != null && modentrylist.ContainsKey(entry.Key))
                            continue;
                        yield return entry;
                    }
                }
            }
            if ((source & StringSource.Game) != 0)
            {
                foreach (KeyValuePair<UInt64, StubbleEntry> entry in GlobalEntries)
                {
                    if (modentrylist != null && modentrylist.ContainsKey(entry.Key))
                        continue;
                    if (loadentrylist != null && loadentrylist.ContainsKey(entry.Key))
                        continue;
                    yield return entry;
                }
            }
        }

        public void CommitModifiedData(Languages lang, StubbleSource source)
        {
            Dictionary<UInt64, StubbleEntry> entrylist;
            if (!mModifiedEntries.TryGetValue(lang, out entrylist))
                return;
            Dictionary<UInt64, StubbleEntry> loadedlist;
            if (!mLoadedEntries.TryGetValue(lang, out loadedlist))
            {
                loadedlist = new Dictionary<ulong, StubbleEntry>();
                mLoadedEntries[lang] = loadedlist;
            }

            foreach (KeyValuePair<UInt64, StubbleEntry> entry in entrylist)
            {
                entry.Value.Source = source;
                loadedlist[entry.Key] = entry.Value;
            }
            mModifiedEntries.Remove(lang);
        }

        public bool HasEntriesForLanguage(Languages lang, StringSource source)
        {
            if ((source & StringSource.Game) != 0)
                throw new InvalidOperationException();

            if (source == StringSource.Display)
                source = mCurrentSource;

            Dictionary<UInt64, StubbleEntry> entrylist = null;
            if ((source & StringSource.Modified) != 0)
            {
                if (mModifiedEntries.TryGetValue(lang, out entrylist) && entrylist.Count > 0)
                    return true;
            }
            if ((source & StringSource.Loaded) != 0)
            {
                if (mLoadedEntries.TryGetValue(lang, out entrylist) && entrylist.Count > 0)
                    return true;
            }
            return false;
        }

        public UInt32 GetHighestPriorityString(StringSource source, Languages lang)
        {
            UInt32 ret = UInt32.MaxValue;
            if ((source & StringSource.Modified) != 0)
                ret = Math.Min(ret, GetHighestPriorityString(mModifiedEntries, lang));
            if ((source & StringSource.Loaded) != 0)
                ret = Math.Min(ret, GetHighestPriorityString(mLoadedEntries, lang));
            if ((source & StringSource.Game) != 0)
                ret = Math.Min(ret, GetHighestPriorityString(mModifiedEntries, lang));
            return ret;
        }

        private UInt32 GetHighestPriorityString(Dictionary<Languages, Dictionary<UInt64, StubbleEntry>> langentrylist, Languages lang)
        {
            UInt32 ret = UInt32.MaxValue;
            Dictionary<UInt64, StubbleEntry> entrylist;
            if (!langentrylist.TryGetValue(lang, out entrylist))
                return ret;

            foreach (StubbleEntry entry in entrylist.Values)
            {
                if (entry.Source.SourceTable == DBPFReference.MinValue)
                    continue;
                ret = Math.Min(ret, (UInt32)entry.Source.SourceTable.Instance);
            }
            return ret;
        }

        private bool TryGetValue(Dictionary<Languages, Dictionary<UInt64, StubbleEntry>> source, UInt64 key, out StubbleEntry value)
        {
            return TryGetValue(mCurrentLanguage, source, key, out value);
        }

        private bool TryGetValue(Dictionary<Languages, Dictionary<UInt64, StubbleEntry>> source, UInt64 key, out string value)
        {
            return TryGetValue(mCurrentLanguage, source, key, out value);
        }

        private bool TryGetValue(Languages targetlang, Dictionary<Languages, Dictionary<UInt64, StubbleEntry>> source, UInt64 key, out StubbleEntry value)
        {
            Dictionary<UInt64, StubbleEntry> entry;
            if (!source.TryGetValue(targetlang, out entry))
            {
                value = null;
                return false;
            }
            return entry.TryGetValue(key, out value);
        }

        private bool TryGetValue(Languages targetlang, Dictionary<Languages, Dictionary<UInt64, StubbleEntry>> source, UInt64 key, out string value)
        {
            StubbleEntry entry;
            if (!TryGetValue(targetlang, source, key, out entry))
            {
                value = null;
                return false;
            }
            value = entry.Value;
            return true;
        }

        private Languages mGlobalLanguage = (Languages)~0;
        private Dictionary<UInt64, StubbleEntry> GlobalEntries
        {
            get
            {
                if (mGlobalLanguage != CurrentLanguage)
                {
                    mGlobalEntries.Clear();
                    mGlobalLanguage = CurrentLanguage;
                    foreach (string filename in GetAllPackageFilenames())
                    {
                        try
                        {
                            DBPFFile file = new DBPFFile(filename);
                            foreach (DBPFIndexEntry entry in file.Index)
                            {
                                try
                                {
                                    if (entry.Reference.Type == 0x220557DA && (Languages)(entry.Reference.Instance >> 56) == CurrentLanguage)
                                    {
                                        StubbleSource source = new StubbleSource();
                                        source.SourceFile = filename;
                                        source.SourceTable = entry.Reference;

                                        DBPFDataStream filedata = file.Open(entry.Reference);
                                        using (filedata)
                                        {
                                            // Force file-at-once read
                                            filedata.GetData();
                                            foreach (KeyValuePair<UInt64, string> stblentry in GetDictionaryLoader(filedata))
                                            {
                                                StubbleEntry newentry;
                                                if (mGlobalEntries.TryGetValue(stblentry.Key, out newentry))
                                                {
                                                    if (newentry.Source.SourceTable != entry.Reference && (newentry.Source.SourceTable.Instance & 0xFFFFFFFFUL) <= (entry.Reference.Instance & 0xFFFFFFFFUL))
                                                        continue;
                                                }

                                                newentry = new StubbleEntry();
                                                newentry.Source = source;
                                                newentry.Value = stblentry.Value;
                                                mGlobalEntries[stblentry.Key] = newentry;
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }

                return mGlobalEntries;
            }
        }

        private Dictionary<UInt64, StubbleEntry> mGlobalEntries = new Dictionary<ulong, StubbleEntry>();
        private Dictionary<Languages, Dictionary<UInt64, StubbleEntry>> mLoadedEntries = new Dictionary<Languages, Dictionary<ulong, StubbleEntry>>();
        private Dictionary<Languages, Dictionary<UInt64, StubbleEntry>> mModifiedEntries = new Dictionary<Languages, Dictionary<ulong, StubbleEntry>>();
    }
    /*
    class NullEnumerator<T> : IEnumerable<T>, IEnumerator<T>
    {
        private static NullEnumerator<T> sSingleton = new NullEnumerator<T>();
        public static NullEnumerator<T> Singleton
        {
            get
            {
                return sSingleton;
            }
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerator<T> Members

        public T Current
        {
            get { throw new InvalidOperationException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { throw new InvalidOperationException(); }
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        #endregion
    }
     */
}
