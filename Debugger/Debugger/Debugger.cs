using Sims3.SimIFace;
using Misukisu.Debug;
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
                "Log: " + filename + "\n"+
                "Remove the misukisu_debugger.package if you do not want to debug";
            SimpleMessageDialog.Show("Virtual Artisan Debugger", msg, ModalDialog.PauseMode.PauseSimulator);
        }

        private void AttachToDebuggables()
        {
            List<IDebuggable> debuggables = new List<IDebuggable>(Sims3.Gameplay.Queries.GetObjects<IDebuggable>());
            foreach (IDebuggable debuggable in debuggables)
            {
                debuggable.setDebugger(this);
                Debug(this, "Attached debugger to " + debuggable.GetType().ToString());
            }
        }

        private void InitDebugLog()
        {
            throw new NotImplementedException();
        }

        public void Debug(object sender, string msg)
        {
            throw new NotImplementedException();
        }

    }
}
