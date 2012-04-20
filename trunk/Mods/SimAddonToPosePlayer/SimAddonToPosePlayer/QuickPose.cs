using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.CmoPoseBox;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using Misukisu.PosePlayerAddon;
using Sims3.Gameplay.Utilities;

namespace Misukisu.PosePlayerAddon
{
    class StartQuickPosing : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            PoseManager.CancelAllPosingActions(Target);
            Target.InteractionQueue.AddAfterCheckingForDuplicates(QuickPose.Singleton.CreateInstance(
               PoseManager.PoseBox, Target, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, StartQuickPosing>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseData.HasPoseData && !isAutonomous && PoseManager.IsPoseBoxAvailable();
            }

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Random Quick Poses";
            }
        }
    }

    class QuickPose : Interaction<Sim, CmoPoseBox>
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public int shots = int.MaxValue;
        public static uint simMinutesToPose = 2;
        protected List<PoseData> poseList;

        public QuickPose()
            : base()
        {
            PoseList = new List<PoseData>(PoseData.sData.Values);
        }

        public override bool Run()
        {
            bool result = true;
            Actor.OverlayComponent.UpdateInteractionFreeParts(AwarenessLevel.OverlayNone);
            Actor.LookAtManager.DisableLookAts();
            for (int i = 0; i < shots; i++)
            {
                if (Actor.HasExitReason(ExitReason.UserCanceled))
                {
                    result = false;
                    break;
                }

                PoseData poseData = GetRandomPose();
                if (poseData == null)
                {
                    result = false;
                    break;
                }
                PoseManager.SetCurrentPose(Actor, poseData.Key);
                Actor.PlaySoloAnimation(poseData.Key, true);
                Actor.ResetAllAnimation();
                Actor.PlaySoloAnimation(poseData.Key, true);
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

        public PoseData GetRandomPose()
        {
            PoseData pose = null;
            int count = PoseData.sData.Count;
            if (count > 0)
            {
                pose = RandomUtil.GetRandomObjectFromList<PoseData>(PoseList);

            }
            return pose;
        }


        private sealed class Definition : InteractionDefinition<Sim, CmoPoseBox, QuickPose>
        {
            public override bool Test(Sim actor, CmoPoseBox target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseData.HasPoseData && !isAutonomous;
            }

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }

            public override string GetInteractionName(Sim actor, CmoPoseBox target, InteractionObjectPair iop)
            {
                return "Random Quick Poses";
            }
        }

        public List<PoseData> PoseList
        {
            get
            {
                return poseList;
            }
            set
            {
                poseList = value;
            }
        }
    }
}

