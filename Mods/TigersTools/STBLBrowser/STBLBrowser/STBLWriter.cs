using System;
using System.Collections.Generic;
using System.Text;

using DBPF;
using System.IO;

namespace STBLBrowser
{
    class STBLWriter
    {
        Dictionary<UInt64, string> mEntries = new Dictionary<ulong, string>();
        Languages mLanguage = Languages.English;
        UInt64 mInstance = 0;

        public STBLWriter Copy()
        {
            return new STBLWriter(this);
        }

        public STBLWriter()
        {
        }

        private STBLWriter(STBLWriter orig)
        {
            mEntries = new Dictionary<ulong, string>(orig.mEntries);
            mLanguage = orig.mLanguage;
            mInstance = orig.mInstance;
        }

        public void Add(UInt64 key, string value)
        {
            mEntries[key] = value;
        }

        public void AddIfNotExists(UInt64 key, string value)
        {
            if (mEntries.ContainsKey(key))
                return;

            mEntries[key] = value;
        }

        public KeyValuePair<DBPFIndexEntry, byte[]> Export()
        {
            MemoryStream ms = new MemoryStream();
            using (ms)
            {
                ms.Write(Encoding.ASCII.GetBytes("STBL"), 0, 4);
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((byte)2);
                bw.Write((short)0);
                bw.Write((int)mEntries.Count);
                bw.Write((int)0);
                bw.Write((short)0);
                foreach (KeyValuePair<UInt64, string> entry in mEntries)
                {
                    bw.Write(entry.Key);
                    bw.Write((int)Encoding.Unicode.GetByteCount(entry.Value) / 2);
                    bw.Write(Encoding.Unicode.GetBytes(entry.Value));
                }
            }

            byte[] data = ms.ToArray();
            byte[] compressed;
            if (DBPFCompression.Compress(data, out compressed))
                return new KeyValuePair<DBPFIndexEntry,byte[]>(new DBPFIndexEntry(new DBPFReference(0x220557DA, 0, Instance), 0, (uint)compressed.Length, (uint)data.Length, true), compressed);
            else
                return new KeyValuePair<DBPFIndexEntry,byte[]>(new DBPFIndexEntry(new DBPFReference(0x220557DA, 0, Instance), 0, (uint)data.Length, (uint)data.Length, false), data);
        }
        
        public Languages Language
        {
            get
            {
                return mLanguage;
            }
            set
            {
                mLanguage = value;
            }
        }

        public UInt64 Instance
        {
            get
            {
                if (mInstance == 0)
                    mInstance = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 5);
                return mInstance & ~(0xFFUL << 56) | (((UInt64)mLanguage) << 56);
            }
            set
            {
                mInstance = value;
            }
        }
    }

    public enum Languages
    {
        English = 0,
        Chinese_Simplified = 1,
        Chinese_Traditional = 2,
        Czech = 3,
        Danish = 4,
        Dutch = 5,
        Finnish = 6,
        French = 7,
        German = 8,
        Greek = 9,
        Hungarian = 10,
        Italian = 11,
        Japanese = 12,
        Korean = 13,
        Norwegian = 14,
        Polish = 15,
        Portuguese = 16,
        Portuguese_Brazilian = 17,
        Russian = 18,
        Spanish = 19,
        Spanish_Mexico = 20,
        Swedish = 21,
        Thai = 22,
        Language_Count = 23
    }
    static class Locales
    {
        public static string[] Names = new string[]{
            "en-us",
            "zh-cn",
            "zh-tw",
            "cs-cz",
            "da-dk",
            "nl-nl",
            "fi-fi",
            "fr-fr",
            "de-de",
            "el-gr",
            "hu-hu",
            "it-it",
            "ja-jp",
            "ko-kr",
            "no-no",
            "pl-pl",
            "pt-pt",
            "pt-br",
            "ru-ru",
            "es-es",
            "es-mx",
            "sv-se",
            "th-th"
        };
    }
}
