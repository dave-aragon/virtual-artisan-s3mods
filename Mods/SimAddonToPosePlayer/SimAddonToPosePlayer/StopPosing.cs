using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.CmoPoseBox;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Misukisu.PosePlayerAddon;

namespace Misukisu.PosePlayerAddon
{
    class StopPosing : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            PoseManager.CancelAllPosingActions(Target);
            return true;
        }


        private sealed class Definition : InteractionDefinition<Sim, Sim, StopPosing>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }

           
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Stop Posing";
            }
        }
    }
}

