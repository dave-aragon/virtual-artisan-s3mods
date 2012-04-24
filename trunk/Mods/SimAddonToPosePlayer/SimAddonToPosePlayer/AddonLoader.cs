
using Sims3.SimIFace;
using System;
using Sims3.Gameplay.EventSystem;
using System.Reflection;
namespace Misukisu.PosePlayerAddon
{
    public class AddonLoader
    {
        private static string POSEBOX = "Sims3.Gameplay.Objects.CmoPoseBox";

        [TunableComment("Scripting Mod Instantiator, value does not matter, only its existence"), Tunable]
        protected static bool kInstantiator = false;

        static AddonLoader()
        {
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(InstallAddon);
        }

        protected static void InstallAddon(object sender, EventArgs e)
        {
           new PoseManager();
        }

        private static bool IsPosePlayerAvailable()
        {
            bool poseboxFound = false;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                string shortName = assembly.FullName.Replace(", Culture=neutral", "");
                if (POSEBOX.Equals(shortName))
                {
                    poseboxFound = true;
                    break;
                }
            }
            return poseboxFound;
        }
    }
}
