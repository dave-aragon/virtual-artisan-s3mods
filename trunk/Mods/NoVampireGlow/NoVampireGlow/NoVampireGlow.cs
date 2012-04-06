using Sims3.SimIFace;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.UI;
using System;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay;
using Sims3.Gameplay.ActorSystems;

namespace Misukisu.NoVampireGlow
{
    public class RemoveVampireGlow : IAlarmOwner
    {
        [TunableComment("Scripting Mod Instantiator, value does not matter, only its existence"), Tunable]
        protected static bool kInstantiator = false;
        public static readonly RemoveVampireGlow Singleton = new RemoveVampireGlow();
        public static List<AlarmHandle> timers = new List<AlarmHandle>();

        static RemoveVampireGlow()
        {
            InWorldState.InWorldSubStateChanging += new InWorldState.InWorldSubStateChangingCallback(InWorldSubStateChanged);
        }

        public static void InWorldSubStateChanged(InWorldState.SubState previousState, InWorldState.SubState newState)
        {
            try
            {
                if (newState == InWorldState.SubState.LiveMode)
                {
                    AlarmHandle handle = AlarmManager.Global.AddAlarm(3, TimeUnit.Seconds, removeGlow,
                        "VampireGlowRemoval", AlarmType.NeverPersisted, Singleton);
                }
            }
            catch (Exception ex)
            {
                // Just let the show continue
            }
        }

        public static void removeGlow()
        {
            try
            {
                releaseAlarms();
                Singleton.RemoveGlowFromAll();
            }
            catch (Exception ex)
            {
                // Just let the show continue
            }
        }

        public static void releaseAlarms()
        {
            foreach (AlarmHandle handle in timers)
            {
                AlarmManager.Global.RemoveAlarm(handle);
            }
        }

        private void RemoveGlowFromAll()
        {
            List<SimDescription> residents = Household.AllSimsLivingInWorld();
            List<SimDescription> townies = Household.AllTownieSimDescriptions();
            int count = RemoveGlowFromVampires(residents);
            //SimpleMessageDialog.Show("Removed Vampire Glow", "From all " + count + " vamps in world", ModalDialog.PauseMode.NoPause);
        }

        private int RemoveGlowFromVampires(List<SimDescription> sims)
        {
            int count = 0;
            foreach (SimDescription sim in sims)
            {
                if (sim.IsVampire && sim.CreatedSim != null)
                {
                    RemoveGlow(sim);
                    count++;
                }
            }
            return count;
        }

        private void RemoveGlow(SimDescription sim)
        {
            if (sim.IsVampire && sim.CreatedSim != null)
            {
                World.ObjectSetVisualOverride(sim.CreatedSim.ObjectId, eVisualOverrideTypes.None, null);
                sim.OccultManager.UpdateOccultUI();
            }
        }
    }
}
