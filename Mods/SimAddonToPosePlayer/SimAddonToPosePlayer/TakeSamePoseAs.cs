using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.CmoPoseBox;

namespace Misukisu.PosePlayerAddon
{
    class StartTakingSamePoseAs : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
            PoseManager.CancelAllPosingActions(Target);
            Target.InteractionQueue.AddNext(TakeSamePoseAs.Singleton.CreateInstance(
                PoseManager.PoseBox, Target, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, StartTakingSamePoseAs>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!PoseManager.IsPoseBoxAvailable())
                {
                    return false;
                }

                Sim poser = PoseManager.LastPoser;
                if (poser == null)
                {
                    return false;
                }

                if (PoseManager.LastPoser == target)
                {
                    return false;
                }

                string poseData = PoseManager.GetCurrentPose(poser);
                if (poseData == null)
                {
                    return false;
                }
                return true;
            }

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return TakeSamePoseAs.getPoseNameWithLastPoser();
            }
        }
    }

    public class TakeSamePoseAs : Interaction<Sim, CmoPoseBox>
    {
        public static InteractionDefinition Singleton = new Definition();

        public class Definition : InteractionDefinition<Sim, CmoPoseBox, TakeSamePoseAs>
        {
            public override string GetInteractionName(Sim actor, CmoPoseBox target, InteractionObjectPair iop)
            {
                return getPoseNameWithLastPoser();
            }
            public override bool Test(Sim actor, CmoPoseBox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }

        public override bool Run()
        {
            Sim poser = PoseManager.LastPoser;
            if (poser == null)
            {
                return false;
            }
            string poseName = PoseManager.GetCurrentPose(poser);
            if (poseName == null)
            {
                return false;
            }

            return PoseManager.Pose(Actor, Target, poseName);
        }

        public static string getPoseNameWithLastPoser()
        {
            string lastPoserName = "";
            Sim lasPoser = PoseManager.LastPoser;
            if (lasPoser != null)
            {
                lastPoserName = lasPoser.FirstName;
            }
            return "Take Same Pose as " + lastPoserName;
        }
    }
}
