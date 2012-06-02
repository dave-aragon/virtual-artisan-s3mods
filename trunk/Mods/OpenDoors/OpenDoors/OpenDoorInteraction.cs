using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;

namespace Misukisu.OpenDoors
{
    class OpenDoorInteraction : ImmediateInteraction<Sim, Door>
    {
        public static bool isDoorOpen(Door target)
        {
            bool isOpen = false;
            uint numDoors = (uint)target.GetNumDoors();
            for (uint num = 0u; num < numDoors; num += 1u)
            {
                if (target.GetDoorState(num) == Door.tState.Opened)
                {
                    isOpen = true;
                    break;
                }

            }
            return isOpen;
        }

        public class Definition : ActorlessInteractionDefinition<Sim, Door, OpenDoorInteraction>
        {
            public bool mOpen = true;
            public CommonDoor.tSide mSide = CommonDoor.tSide.Front;
            public Definition()
            {
            }

            public override string GetInteractionName(Sim a, Door target, InteractionObjectPair interaction)
            {
                string name = "Leave Open";
                mOpen = true;
                bool isOpen = isDoorOpen(target);

                if (isOpen)
                {
                    name = "Close";
                    mOpen = false;
                }

                return name;
            }



            public override bool Test(Sim a, Door target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous)
                {
                    return false;
                }
                return true;
            }
        }
        public static InteractionDefinition Singleton = new OpenDoorInteraction.Definition();
        public override bool Run()
        {
            OpenDoorInteraction.Definition definition = base.InteractionDefinition as OpenDoorInteraction.Definition;
            if (definition.mOpen)
            {
                uint numDoors = (uint)this.Target.GetNumDoors();
                for (uint num = 0u; num < numDoors; num += 1u)
                {
                    this.Target.Open(num, definition.mSide);
                    this.Target.SetDoorState(num, Door.tState.Opened);
                }
            }
            else
            {
                uint numDoors = (uint)this.Target.GetNumDoors();
                for (uint num = 0u; num < numDoors; num += 1u)
                {
                    this.Target.Close(num, definition.mSide);
                    this.Target.SetDoorState(num, Door.tState.Closed);
                }
            }

            return true;
        }
    }

    class OpenDoorToOtherSideInteraction : ImmediateInteraction<Sim, Door>
    {
        public class Definition : ActorlessInteractionDefinition<Sim, Door, OpenDoorToOtherSideInteraction>
        {
            public Definition()
            {
            }

            public override string GetInteractionName(Sim a, Door target, InteractionObjectPair interaction)
            {
                string name = "Leave Open (To Other Side)";

                return name;
            }

            public override bool Test(Sim a, Door target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous || OpenDoorInteraction.isDoorOpen(target))
                {
                    return false;
                }
                return true;
            }
        }
        public static InteractionDefinition Singleton = new OpenDoorToOtherSideInteraction.Definition();
        public override bool Run()
        {
            OpenDoorToOtherSideInteraction.Definition definition = base.InteractionDefinition as OpenDoorToOtherSideInteraction.Definition;

            uint numDoors = (uint)this.Target.GetNumDoors();
            for (uint num = 0u; num < numDoors; num += 1u)
            {
                this.Target.Open(num, CommonDoor.tSide.Back);
                this.Target.SetDoorState(num, Door.tState.Opened);
            }

            return true;
        }
    }
}
