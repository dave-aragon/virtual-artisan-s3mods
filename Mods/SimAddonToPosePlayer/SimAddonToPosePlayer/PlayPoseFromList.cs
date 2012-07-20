﻿using System;
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
    class StartPoseFromList : ImmediateInteraction<Sim, Sim>
    {

        public static readonly InteractionDefinition Singleton = new Definition();
        public static readonly InteractionDefinition LinkedInteraction = PlayPoseFromList.Singleton;

        public override bool Run()
        {
            PoseManager.CancelAllPosingActions(Target);
            Target.InteractionQueue.AddNext(LinkedInteraction.CreateInstance(
                PoseManager.PoseBox, Target, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
            return true;
        }


        private sealed class Definition : InteractionDefinition<Sim, Sim, StartPoseFromList>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPoseBoxAvailable();
            }

            

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Select Pose...";
            }
        }
    }

    public class PlayPoseFromList : Interaction<Sim, CmoPoseBox>
    {
        public static InteractionDefinition Singleton = new PlayPoseFromList.Definition();

        public class Definition : InteractionDefinition<Sim, CmoPoseBox, PlayPoseFromList>
        {
            public override string GetInteractionName(Sim actor, CmoPoseBox target, InteractionObjectPair iop)
            {
                return "Take Pose";
            }
            public override bool Test(Sim actor, CmoPoseBox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseData.HasPoseData && !isAutonomous;
            }
        }
        
        public override bool Run()
        {
            PoseData poseData = PoseData.SelectPoseFromList();
            if (poseData == null)
            {
                return false;
            }
            return PoseManager.Pose(Actor, Target, poseData.Key);
        }
    }
}