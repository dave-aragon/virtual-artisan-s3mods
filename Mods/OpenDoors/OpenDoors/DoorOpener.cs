using System;
using System.Collections.Generic;
using System.Text;
using Sims3.SimIFace;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Abstracts;

namespace Misukisu.OpenDoors
{
    public class DoorOpener
    {
        [TunableComment("Scripting Mod Instantiator, value does not matter, only its existence"), Tunable]
        protected static bool kInstantiator = false;
        private bool boughtListenerAdded = false;
        private static DoorOpener instance = new DoorOpener();

        static DoorOpener()
        {
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(instance.AddInteractionsToDoors);
        }

        protected void AddInteractionsToDoors(object sender, EventArgs e)
        {
            //Sims3.Gameplay.Gameflow.SetGameSpeed(Gameflow.GameSpeed.Pause, Sims3.Gameplay.Gameflow.SetGameSpeedContext.GameStates);

            //Debugger debugger = new Debugger("Testing");
            //debugger.Debug("Testing", "adding interactions to existing doors");

            List<Door> doors = new List<Door>(Sims3.Gameplay.Queries.GetObjects<Door>());
            //debugger.Debug("Testing", "DoorCount=" + doors.Count);
            foreach (Door door in doors)
            {
                AddOpenInteractions(door);
            }
            //debugger.Debug("Testing", "All interactions added");
            //debugger.EndDebugLog();
            if (!boughtListenerAdded)
            {
                EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(instance.OnObjectBought));
                boughtListenerAdded = true;
            }
        }

        protected ListenerAction OnObjectBought(Event e)
        {
            if (e != null)
            {
                Door door = e.TargetObject as Door;

                if (door != null)
                {
                    AddOpenInteractions(door);
                }
            }
            return ListenerAction.Keep;
        }

        private static void AddOpenInteractions(Door door)
        {
            door.RemoveInteractionByType(OpenDoorToOtherSideInteraction.Singleton);
            door.AddInteraction(OpenDoorToOtherSideInteraction.Singleton);
            door.RemoveInteractionByType(OpenDoorInteraction.Singleton);
            door.AddInteraction(OpenDoorInteraction.Singleton);
        }
    }
}
