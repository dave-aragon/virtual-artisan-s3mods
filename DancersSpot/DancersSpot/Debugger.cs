using System;
using System.Collections.Generic;
using System.Text;
using Sims3.UI;
using Sims3.SimIFace;
using Sims3.Gameplay.Utilities;
using System.Reflection;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.UI.Hud;

namespace Misukisu.Dancer
{

    class Debugger
    {
        private CustomXmlWriter mLogXmlWriter;
        public Debugger(object target)
            : base()
        {
            StartDebugLog(target);
        }

        // TODO: dump script errors always to log

        private void NotifyUserOfDebugging(string filename)
        {

            string msg = I18n.Localize(CommonTexts.DEBUG_STARTING, new string[] { filename },
                "Debugging makes the game slower" + Message.NewLine +
                "Log File: " + filename);
            string title = I18n.Localize(CommonTexts.DEBUG_STARTING_TITLE,
                "Virtual Artisan Debugger Started");
            SimpleMessageDialog.Show(title, msg, ModalDialog.PauseMode.NoPause);

        }

        public void StartDebugLog(object target)
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
                    mLogXmlWriter.WriteElementString("VirtualArtisanDebugger", target.GetType().ToString());
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
                    NotifyUserOfDebugging(result);
                }
            }
            catch (Exception ex)
            {
                EndDebugLog();
                string title = I18n.Localize(CommonTexts.DEBUG_CANNOT_START_LOGGER, "Virtual Artisan Debugger CANNOT be started");
                string text = I18n.Localize(CommonTexts.DEBUG_CANNOT_CREATE_LOG_FILE, "Cannot create debug log");

                Message.Sender.ShowError(title, text, false, ex);
            }

        }

        public void EndDebugLog()
        {
            try
            {
                if (mLogXmlWriter != null)
                {
                    // Closes the file handles too
                    mLogXmlWriter.WriteEndDocument();
                    mLogXmlWriter = null;
                }
            }
            catch (Exception ex)
            {
                Message.Sender.ShowError(this, "Cannot stop debugging", false, ex);

            }
        }

        public void Debug(object sender, string msg)
        {
            try
            {
                if (mLogXmlWriter != null)
                {
                    CustomXmlAttribute[] attributes = new CustomXmlAttribute[]
		            {
			            new CustomXmlAttribute("Time", SimClock.CurrentTime().ToString()),
                         new CustomXmlAttribute("Sender", sender.GetType().Name)  ,
                         new CustomXmlAttribute("Id", GetId(sender)) 
		            };
                    mLogXmlWriter.WriteElementString("Log", ScriptError.Escape(msg), attributes);
                    mLogXmlWriter.FlushBufferToFile();
                }
            }
            catch (Exception ex)
            {
                Message.Sender.ShowError(this, "Cannot debug", false, ex);

            }
        }

        public void DebugError(object sender, string msg, Exception ex)
        {
            try
            {
                if (mLogXmlWriter != null)
                {
                    CustomXmlAttribute[] attributes = new CustomXmlAttribute[]
		            {
			            new CustomXmlAttribute("Time", SimClock.CurrentTime().ToString()),
                         new CustomXmlAttribute("Sender", sender.GetType().Name) ,
                         new CustomXmlAttribute("Id", GetId(sender)) 
		            };
                    mLogXmlWriter.WriteElementString("Log", ScriptError.Escape(msg), attributes);
                    if (ex != null)
                    {
                        mLogXmlWriter.WriteElementString("Error", ex.Message + Message.NewLine + ex.StackTrace, attributes);
                    }
                    mLogXmlWriter.FlushBufferToFile();
                }
            }
            catch (Exception e)
            {
                Message.Sender.Show("Cannot write debug log");
            }
        }

        private string GetId(object item)
        {
            string id = "";

            if (item != null)
            {
                SimDescription simD = item as SimDescription;
                if (simD != null)
                {
                    id = simD.FullName;
                }

                if (id == null)
                {
                    Sim sim = item as Sim;
                    if (sim != null)
                    {
                        id = sim.FullName;
                    }
                }

                if (id == null)
                {
                    Role roleItem = item as Role;
                    if (roleItem != null)
                    {
                        id = GetId(roleItem.mSim);
                    }
                }

                if (id == null)
                {
                    IRoleGiver roleItem = item as IRoleGiver;
                    if (roleItem != null && roleItem.CurrentRole != null)
                    {
                        id = GetId(roleItem.CurrentRole.mSim);
                    }
                }

                if (id == null)
                {
                    IInteractionInstance roleItem = item as IInteractionInstance;
                    if (roleItem != null)
                    {
                        id = GetId(roleItem.IInstanceActor);
                    }
                }

                if (id == null)
                {
                    id = item.GetHashCode().ToString();
                }
            }

            return id;
        }
    }
}
