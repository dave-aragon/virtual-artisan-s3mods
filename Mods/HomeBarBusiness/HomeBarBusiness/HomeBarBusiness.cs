using System;
using System.Collections.Generic;
using System.Text;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.EventSystem;

namespace Misukisu.HomeBarBusiness
{
    public class ActivateBusiness
    {
        [TunableComment("Scripting Mod Instantiator, value does not matter, only its existence"), Tunable]
        protected static bool kInstantiator = false;
        private static bool boughtListenerAdded = false;
        public static Debugger debugger = new Debugger("Testing");

        static ActivateBusiness()
        {
            debugger.Debug("Testing", "started");
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(AddInteractionsToBars);
            debugger.Debug("Testing", "event handler added");
        }


        protected static void AddInteractionsToBars(object sender, EventArgs e)
        {
            //Sims3.Gameplay.Gameflow.SetGameSpeed(Gameflow.GameSpeed.Pause, Sims3.Gameplay.Gameflow.SetGameSpeedContext.GameStates);


            //debugger.Debug("Testing", "adding interactions to existing doors");

            List<BarProfessional> bars = new List<BarProfessional>(Sims3.Gameplay.Queries.GetObjects<BarProfessional>());
            debugger.Debug("Testing", "BarCount=" + bars.Count);
            foreach (BarProfessional bar in bars)
            {
                AddTendInteractions(bar);
            }
            debugger.Debug("Testing", "All interactions added");
            //debugger.EndDebugLog();
            if (!boughtListenerAdded)
            {
                EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(OnObjectBought));
                boughtListenerAdded = true;
            }
        }

        protected static ListenerAction OnObjectBought(Event e)
        {
            if (e != null)
            {
                BarProfessional door = e.TargetObject as BarProfessional;

                if (door != null)
                {
                    AddTendInteractions(door);
                }
            }
            return ListenerAction.Keep;
        }

        private static void AddTendInteractions(BarProfessional bar)
        {
            bar.RemoveInteractionByType(TendHomeBar.Singleton);
            bar.AddInteraction(TendHomeBar.Singleton);
            //bar.RemoveInteractionByType(WaitForOrders.Singleton);
            //bar.AddInteraction(WaitForOrders.Singleton);
        }
    }

}
