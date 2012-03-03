using System;
using System.Collections.Generic;
using System.Text;
using Sims3.UI;
using Sims3.SimIFace;
using Sims3.Gameplay.Utilities;
using System.Reflection;

namespace Misukisu.Common
{

    class Debugger
    {
        private CustomXmlWriter mLogXmlWriter;
        public Debugger(object target)
            : base()
        {
            StartDebugLog(target.GetType().Name);
        }

        private void NotifyUserOfDebugging(string filename)
        {
            
                string msg = "Debugging makes the game slower" + Message.NewLine +
                    "Log File: " + filename;
                SimpleMessageDialog.Show("Virtual Artisan Debugger Started", msg, ModalDialog.PauseMode.NoPause);
            
        }

        public void StartDebugLog(string target)
        {
            string result = "";
            try
            {
                uint num = 0u;
                result = Simulator.CreateScriptErrorFile(ref num);
                if (num != 0u)
                {
                    mLogXmlWriter = new CustomXmlWriter(num);
                    mLogXmlWriter.WriteStartDocument();
                    mLogXmlWriter.WriteElementString("VirtualArtisanDebugger", target);
                    string[] data = GameUtils.GetGenericString(GenericStringID.VersionData).Split(new char[] { '\n' });

                    string[] labels = GameUtils.GetGenericString(GenericStringID.VersionLabels).Split(new char[] { '\n' });
                    int count = (labels.Length > data.Length) ? data.Length : labels.Length;
                    for (int i = 0; i < count; i++)
                    {
                        if (labels[i] == "Game Version:")
                        {
                            mLogXmlWriter.WriteElementString("GameVersion", data[i]);
                        }
                        else if (labels[i] == "Build Version:")
                        {
                            mLogXmlWriter.WriteElementString("BuildVersion", data[i]);
                        }
                    }


                    IGameUtils gameUtils = (IGameUtils)AppDomain.CurrentDomain.GetData("GameUtils");
                    if (gameUtils != null)
                    {
                        ProductVersion productFlags = (ProductVersion)gameUtils.GetProductFlags();
                        mLogXmlWriter.WriteElementString("Installed", productFlags.ToString());
                    }
                    mLogXmlWriter.WriteStartElement("Assemblies");

                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly assembly in assemblies)
                    {
                        string shortName = assembly.FullName.Replace(", Culture=neutral", "");
                        mLogXmlWriter.WriteElementString("Assembly", shortName);
                    }

                    mLogXmlWriter.WriteEndElement();

                    mLogXmlWriter.FlushBufferToFile();
                   
                }
            }
            catch (Exception ex)
            {
                EndDebugLog();
                Message.Sender.ShowError("Virtual Artisan Debugger CANNOT be started", "Cannot create debug log", false, ex);
                return;
            }
            NotifyUserOfDebugging(result);
        }

        public void EndDebugLog()
        {
            if (mLogXmlWriter != null)
            {
                // Closes the file handles too
                mLogXmlWriter.WriteEndDocument();
                mLogXmlWriter = null;
            }
        }

        public void Debug(object sender, string msg)
        {
            if (mLogXmlWriter != null)
            {
                CustomXmlAttribute[] attributes = new CustomXmlAttribute[]
		            {
			            new CustomXmlAttribute("Time", SimClock.CurrentTime().ToString()),
                         new CustomXmlAttribute("Sender", sender.GetType().Name) 
		            };
                mLogXmlWriter.WriteElementString("Log", ScriptError.Escape(msg), attributes);
                mLogXmlWriter.FlushBufferToFile();
            }
        }



        //public void foo()
        //{
        //    uint num = 0u;
        //    string result = Simulator.CreateScriptErrorFile(ref num);
        //    if (num != 0u)
        //    {
        //        CustomXmlWriter customXmlWriter = new CustomXmlWriter(num);
        //        customXmlWriter.WriteStartDocument();
        //        CustomXmlAttribute[] attributes = new CustomXmlAttribute[]
        //{
        //    new CustomXmlAttribute("Version", 6.ToString()), 
        //    new CustomXmlAttribute("Type", isFullError ? "FullScriptError" : "MiniScriptError"), 
        //    new CustomXmlAttribute("MaxRecursionDepth", this.mMaxRecursionDepth.ToString())
        //};
        //        customXmlWriter.WriteStartElement("ScriptError", attributes);
        //        string text = this.GetErrorText();
        //        text = ScriptError.Escape(text);
        //        customXmlWriter.WriteElementString("ExceptionData", text);
        //        this.GetGameData(customXmlWriter);
        //        if (isFullError)
        //        {
        //            this.DumpAllObjects(customXmlWriter);
        //            this.GetMiscData(customXmlWriter);
        //        }
        //        customXmlWriter.WriteEndElement();
        //        customXmlWriter.WriteEndDocument();
        //    }
        //    return result;
        //}



    }
}
