﻿using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Objects.CmoPoseBox;
using Sims3.SimIFace;

namespace Misukisu.PosePlayerAddon
{
    class StartPosingFromMyList : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            PoseManager.CancelAllPosingActions(Target);
            Target.InteractionQueue.AddNext(PoseFromMyList.Singleton.CreateInstance(
               PoseManager.PoseBox, Target, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, StartPosingFromMyList>
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
                return "Select Pose from My List";
            }
        }
    }

    class PoseFromMyList : Interaction<Sim, CmoPoseBox>
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public static uint simMinutesToPose = 2;

        public override bool Run()
        {
            this.Target.ShowDialog();
            this.Target.MyListShow();
            if (CmoPoseBox.mName == null || CmoPoseBox.mName == "")
            {
                return true;
            }
            this.Actor.OverlayComponent.UpdateInteractionFreeParts(AwarenessLevel.OverlayNone);
            this.Actor.LookAtManager.DisableLookAts();
            PoseManager.SetCurrentPose(Actor, CmoPoseBox.mName);
            this.Actor.PlaySoloAnimation(CmoPoseBox.mName, true);
            this.Actor.ResetAllAnimation();
            this.Actor.PlaySoloAnimation(CmoPoseBox.mName, true);
            this.Actor.ResetAllAnimation();
            this.Actor.WaitForExitReason(3.40282347E+38f, ExitReason.UserCanceled);
            this.Actor.LookAtManager.EnableLookAts();
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, CmoPoseBox, PoseFromMyList>
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
                return "Select Pose From My List";
            }
        }
    }
}