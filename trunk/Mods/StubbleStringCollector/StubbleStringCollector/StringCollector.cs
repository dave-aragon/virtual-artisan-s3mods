
using Sims3.SimIFace;
using System;
using Sims3.Gameplay.EventSystem;
using System.Reflection;
using System.Collections.Generic;
using Sims3.Gameplay.Utilities;
namespace Misukisu.Stubble
{
    public class StringCollector
    {
        [TunableComment("Scripting Mod Instantiator, value does not matter, only its existence"), Tunable]
        protected static bool kInstantiator = false;
        private LocalizerWithCollector localizer;
        private static StringCollector collector;
        //private Debugger debugger;

        static StringCollector()
        {
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(InstallLocalizerWithCollector);
            World.sOnWorldQuitEventHandler += new EventHandler(WriteCollectedStringsToFile);
        }

        private static void InstallLocalizerWithCollector(object sender, EventArgs e)
        {
            collector = new StringCollector();
           
        }

        private static void WriteCollectedStringsToFile(object sender, EventArgs e)
        {
            if (collector != null)
            {
                collector.WriteStringFile();
            }
        }

        private void WriteStringFile()
        {
            try
            {
                List<string> strings = localizer.GetCollectedStrings();
                //debugger.Debug(this, "writing strings to file - count: " + strings.Count);
                uint fileHandle = 0u;
                string result = Simulator.CreateScriptErrorFile(ref fileHandle);
                if (fileHandle != 0u)
                {
                    // The XmlWriter has methods to write anything
                    CustomXmlWriter xmlWriter = new CustomXmlWriter(fileHandle);
                    foreach (string key in strings)
                    {
                        xmlWriter.WriteToBuffer(key);
                        xmlWriter.WriteToBuffer(System.Environment.NewLine);
                    }
                    xmlWriter.FlushBufferToFile();

                    Simulator.CloseScriptErrorFile(fileHandle);

                }
               
            }
            catch (Exception ex)
            {
                Debugger debugger = new Debugger(this);
                debugger.DebugError(this, "String collector failed to write file", ex);
                debugger.EndDebugLog();
            }
        }



        public StringCollector()
        {

           
            try{
           
            localizer = new LocalizerWithCollector();
            StringTable.gStringTable = localizer;
           
            }
            catch (Exception ex)
            {
                Debugger debugger = new Debugger(this);
                debugger.DebugError(this, "String collector creation failed",ex);
                debugger.EndDebugLog();
                
            }
        }
    }
}
