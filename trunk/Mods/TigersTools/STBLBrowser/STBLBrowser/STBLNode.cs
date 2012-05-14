using System;
using System.Collections.Generic;
using System.Text;

namespace STBLBrowser
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class STBLNode
    {
        public string Name;
        public List<STBLNode> Children;
        public List<string> Entries;
    }
}
