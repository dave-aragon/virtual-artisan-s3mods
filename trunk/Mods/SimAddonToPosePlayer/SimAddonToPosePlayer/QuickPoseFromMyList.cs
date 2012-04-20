using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.CmoPoseBox;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;

namespace Misukisu.PosePlayerAddon
{
    class StartQuickPosingFromList : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            PoseManager.CancelAllPosingActions(Target);
            Target.InteractionQueue.AddAfterCheckingForDuplicates(QuickPoseFromMyList.Singleton.CreateInstance(
               PoseManager.PoseBox, Target, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, StartQuickPosingFromList>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return (CmoPoseBox.myList.Count > 0) && !isAutonomous && PoseManager.IsPoseBoxAvailable();
            }

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Quick Poses from My List";
            }
        }
    }

    class QuickPoseFromMyList : Interaction<Sim, CmoPoseBox>
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public static uint simMinutesToPose = 2;

        public override bool Run()
        {
            bool result = true;
            Actor.OverlayComponent.UpdateInteractionFreeParts(AwarenessLevel.OverlayNone);
            Actor.LookAtManager.DisableLookAts();
            // Clone the list to prevent need for synchronization
            List<string> poseList = new List<string>(CmoPoseBox.myList);
            foreach (string poseName in poseList)
            {
                if (Actor.HasExitReason(ExitReason.UserCanceled))
                {
                    result = false;
                    break;
                }
                if (poseName == null)
                {
                    result = false;
                    break;
                }
                PoseManager.SetCurrentPose(Actor, poseName);
                Actor.PlaySoloAnimation(poseName, true);
                Actor.ResetAllAnimation();
                Actor.PlaySoloAnimation(poseName, true);
                Actor.ResetAllAnimation();
                bool userCanceled = Actor.WaitForExitReason(simMinutesToPose, ExitReason.UserCanceled);
                if (userCanceled)
                {
                    result = false;
                    break;
                }
            }
            Actor.LookAtManager.EnableLookAts();
            return result;
        }

        private sealed class Definition : InteractionDefinition<Sim, CmoPoseBox, QuickPoseFromMyList>
        {
            public override bool Test(Sim actor, CmoPoseBox target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return (CmoPoseBox.myList.Count > 0) && PoseManager.IsPoseBoxAvailable();
            }

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }

            public override string GetInteractionName(Sim actor, CmoPoseBox target, InteractionObjectPair iop)
            {
                return "Quick Poses from My List";
            }
        }
    }
}
