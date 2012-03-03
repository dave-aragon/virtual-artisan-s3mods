using Sims3.SimIFace;

using System;
using Sims3.UI;
using System.Collections.Generic;

namespace Misukisu.Common
{
    public class Debugger : IDebugger
    {

        [TunableComment("Scripting Mod Instantiator, value does not matter, only its existence"), Tunable]
        protected static bool kInstantiator = false;
        public static Debugger Instance = null;

        static Debugger()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinished);
        }

        private static void OnWorldLoadFinished(object sender, EventArgs e)
        {
            InstantiateDebugger();
        }

        private static void InstantiateDebugger()
        {
            if (Instance == null)
            {
                new Debugger();
            }
        }

        private Debugger()
        {
            InitDebugLog();
            NotifyUser("filenamehere");
            AttachToDebuggables();

        }

        private void NotifyUser(string filename)
        {
            string msg = "Debugger is started - this WILL slow down your game\n" +
                "Log: " + filename + "\n" +
                "Remove the misukisu_debugger.package if you do not want to debug";
            SimpleMessageDialog.Show("Virtual Artisan Debugger Is Started", msg, ModalDialog.PauseMode.PauseSimulator);
        }

        private void AttachToDebuggables()
        {
            List<IDebuggable> debuggables = new List<IDebuggable>(Sims3.Gameplay.Queries.GetObjects<IDebuggable>());
            Debug(this, debuggables.Count.ToString() + " debuggables found");
            foreach (IDebuggable debuggable in debuggables)
            {
                AttachDebuggerTo(debuggable);
            }
        }

        public void AttachDebuggerTo(IDebuggable debuggable)
        {
            debuggable.setDebugger(this);
            Debug(this, "Attached debugger to " + debuggable.GetType().ToString());
        }

        private void InitDebugLog()
        {
            //TODO:
        }

        public void Debug(object sender, string msg)
        {
            string finalMsg = sender.GetType().ToString() + " - " + msg;
            StyledNotification.Format format = new StyledNotification.Format(finalMsg, StyledNotification.NotificationStyle.kSystemMessage);
            StyledNotification.Show(format);


            bool result;
            try
            {
                if (addHeader)
                {
                    Common.sLogEnumerator++;
                    string[] array = GameUtils.GetGenericString(GenericStringID.VersionLabels).Split(new char[]
			{
				'\n'
			});
                    string[] array2 = GameUtils.GetGenericString(GenericStringID.VersionData).Split(new char[]
			{
				'\n'
			});
                    string text2 = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Common.NewLine;
                    string text3 = text2;
                    text2 = string.Concat(new string[]
			{
				text3, 
				"<", 
				VersionStamp.sNamespace, 
				">", 
				Common.NewLine
			});
                    object obj = text2;
                    text2 = string.Concat(new object[]
			{
				obj, 
				"<ModVersion value=\"", 
				VersionStamp.sVersion, 
				"\"/>", 
				Common.NewLine
			});
                    int num = (array.Length > array2.Length) ? array2.Length : array.Length;
                    for (int i = 0; i < num; i++)
                    {
                        string text4 = array[i].Replace(":", "").Replace(" ", "");
                        string a;
                        if ((a = text4) != null && (a == "GameVersion" || a == "BuildVersion"))
                        {
                            string text5 = text2;
                            text2 = string.Concat(new string[]
					{
						text5, 
						"<", 
						text4, 
						" value=\"", 
						array2[i], 
						"\"/>", 
						Common.NewLine
					});
                        }
                    }
                    IGameUtils gameUtils = (IGameUtils)AppDomain.CurrentDomain.GetData("GameUtils");
                    if (gameUtils != null)
                    {
                        ProductVersion productFlags = (ProductVersion)gameUtils.GetProductFlags();
                        object obj2 = text2;
                        text2 = string.Concat(new object[]
				{
					obj2, 
					"<Installed=\"", 
					productFlags, 
					"\"/>", 
					Common.NewLine
				});
                    }
                    object obj3 = text2;
                    text2 = string.Concat(new object[]
			{
				obj3, 
				"<Enumerator value=\"", 
				Common.sLogEnumerator, 
				"\"/>", 
				Common.NewLine
			});
                    text2 = text2 + "<Content>" + Common.NewLine;
                    text = text2 + text.Replace("&", "&amp;");
                    text = text + Common.NewLine + "</Content>";
                    string text6 = text;
                    text = string.Concat(new string[]
			{
				text6, 
				Common.NewLine, 
				"</", 
				VersionStamp.sNamespace, 
				">"
			});
                }
                uint num2 = 0u;
                Simulator.CreateScriptErrorFile(ref num2);
                if (num2 != 0u)
                {
                    CustomXmlWriter customXmlWriter = new CustomXmlWriter(num2);
                    customXmlWriter.WriteToBuffer(text);
                    customXmlWriter.WriteEndDocument();
                }
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

    }
}
