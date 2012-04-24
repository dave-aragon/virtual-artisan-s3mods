using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.CmoPoseBox;

namespace Misukisu.PosePlayerAddon
{
    public class PutPoseToList : ImmediateInteraction<Sim, Sim>
    {
        public class Definition : InteractionDefinition<Sim, Sim, PutPoseToList>
        {
            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return "Add Current Pose to My List";
            }

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous && PoseManager.IsPoseBoxAvailable();
            }
        }
        public static InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {

            string posename = PoseManager.GetCurrentPose(Target);
            if (posename != null)
            {
                CmoPoseBox.myList.Add(posename);
            }
            else
            {
                return false;
            }
            return true;
        }
    }

    public class RemovePoseFromList : ImmediateInteraction<Sim, Sim>
    {
        public class Definition : InteractionDefinition<Sim, Sim, RemovePoseFromList>
        {
            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return "Remove Current Pose from My List";
            }

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                Boolean result = false;
                if (PoseManager.IsPosing(target))
                {
                    string posename = PoseManager.GetCurrentPose(target);
                    if (posename != null)
                    {
                        if (CmoPoseBox.myList.Contains(posename))
                        {
                            result = true;
                        }
                    }
                }
                return result;
            }
        }
        public static InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {

            string posename = PoseManager.GetCurrentPose(Target);
            if (posename != null)
            {
                CmoPoseBox.myList.Remove(posename);
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
