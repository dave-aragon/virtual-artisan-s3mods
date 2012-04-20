using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.UI;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.CmoPoseBox;

namespace Misukisu.PosePlayerAddon
{
    class PoseInteraction : ImmediateInteraction<Sim, Sim>
    {

        public static readonly InteractionDefinition Singleton = new Definition();
        public static readonly InteractionDefinition LinkedInteraction = PlayPoseFromList.Singleton;

        public override bool Run()
        {
            Target.AddExitReason(ExitReason.UserCanceled);
            Target.InteractionQueue.CancelAllInteractionsByType(LinkedInteraction);
            Target.InteractionQueue.AddAfterCheckingForDuplicates(LinkedInteraction.CreateInstance(
                PoseManager.PoseBox, Target, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
            return true;
        }


        private sealed class Definition : InteractionDefinition<Sim, Sim, PoseInteraction>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {

                //  return GlobalFunctions.GetClosestObject<BarProfessional>(sim, false, true, new List<BarProfessional>(), null);
                if (PoseManager.PoseBox == null)
                {
                    PoseManager.FindPoseBox();
                }

                return (PoseManager.PoseBox != null);
            }

            public override string[] GetPath(bool isFemale)
            {
                return new String[] { "Posing..." };
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Select Pose...";
            }
        }
    }
}
