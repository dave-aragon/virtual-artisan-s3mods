using System;
using Sims3.SimIFace;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.UI;

namespace Misukisu.NoVampireGlow
{
    public class RemoveVampireGlow : IAlarmOwner
    {
        [TunableComment("Scripting Mod Instantiator, value does not matter, only its existence"), Tunable]
        protected static bool kInstantiator = false;
        public static readonly RemoveVampireGlow Singleton= new RemoveVampireGlow();

        static RemoveVampireGlow()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinished);
        }

        private static void OnWorldLoadFinished(object sender, EventArgs e)
        {
            Singleton.RemoveGlowFromAll();
            AlarmManager.Global.AddAlarmDay(
                       World.GetSunriseTime() + 1, DaysOfTheWeek.All,
                       new AlarmTimerCallback(Singleton.RemoveGlowFromAll),
                       "Vampire Glow Removal", AlarmType.NeverPersisted,Singleton);
        }

        private void RemoveGlowFromAll()
        {

            List<SimDescription> residents = Household.AllSimsLivingInWorld();
            List<SimDescription> townies = Household.AllTownieSimDescriptions();
            RemoveGlowFromVampires(residents);
            SimpleMessageDialog.Show("Removed Vampire Glow", "From all vamps in world", ModalDialog.PauseMode.NoPause);
       
        }

        private void RemoveGlowFromVampires(List<SimDescription> sims)
        {
            foreach (SimDescription sim in sims)
            {
                if (sim.IsVampire && sim.CreatedSim != null)
                {

                    RemoveGlow(sim);
                   
                }
            }
        }

        private void RemoveGlow(SimDescription sim)
        {
            if (sim.IsVampire && sim.CreatedSim != null)
            {
                World.ObjectSetVisualOverride(sim.CreatedSim.ObjectId, eVisualOverrideTypes.None, null);
            }
        }
    }
}
