using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.CmoPoseBox;
using Sims3.Gameplay.Autonomy;

namespace Misukisu.SimAddonToPosePlayer
{
    class StopPosing : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            Target.AddExitReason(ExitReason.UserCanceled);
            Target.InteractionQueue.CancelAllInteractionsByType(PlayPoseFromList.Singleton);
            return true;
        }


        private sealed class Definition : InteractionDefinition<Sim, Sim, StopPosing>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {

                if (target.InteractionQueue.GetCurrentInteraction() is PlayPoseFromList)
                {
                    return true;
                }

                return false;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new String[] { "Posing..." };
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Stop Posing";
            }
        }
    }
}

