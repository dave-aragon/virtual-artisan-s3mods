using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace STBLBrowser
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Dictionary<UInt64, string> keynames = new Dictionary<ulong, string>();
                try
                {
                    STBLVault.InitializeKeyNameMap(Path.Combine(Application.StartupPath, "STBL.txt"));
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Error opening stbl.txt: {0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                try
                {
                    STBLVault.InitializeKeyNameMap(STBLVault.UserMapFilename);
                }
                catch (Exception e)
                {
                    e.ToString();
                }
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(string.Format("An error has occurred.  Copy details to the clipboard?\n\n{0}", ex), "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataObject obj = new DataObject();
                    obj.SetText(ex.ToString());
                    Clipboard.SetDataObject(obj, true);
                }
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (MessageBox.Show(string.Format("An error has occurred.  Copy details to the clipboard?\n\n{0}", e.Exception), "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DataObject obj = new DataObject();
                obj.SetText(e.Exception.ToString());
                Clipboard.SetDataObject(obj, true);
            }
        }
    }
}
