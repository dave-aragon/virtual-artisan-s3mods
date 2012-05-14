using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;

namespace STBLBrowser.ImportExport
{
    internal interface IExport
    {
        STBLVault STBLData { get; set; }
        bool Export(IWin32Window owner, string filename);
        void Export(IWin32Window owner);
    }
}
