using System;
using System.Collections.Generic;
using System.Text;

using DBPF;

namespace STBLBrowser
{
    class StubbleEntry
    {
        public string Value;
        public StubbleSource Source;
    }
    class StubbleSource
    {
        public string SourceFile;
        public DBPFReference SourceTable;
    }
    class UserSource : StubbleSource
    {
        public bool Modified;
    }
}
