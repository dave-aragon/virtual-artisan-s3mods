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

namespace Misukisu.SimAddonToPosePlayer
{
    class SnapshotPose : Interaction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public static int shots = 15;
        public static uint simMinutesToPose = 2;

        public override bool Run()
        {
            Target.AddExitReason(ExitReason.UserCanceled);
            Target.InteractionQueue.CancelAllInteractionsByType(PlayPoseFromList.Singleton);
            Debugger debugger = new Debugger(this);
            Target.ClearExitReasons();
            bool result = true;
            Target.OverlayComponent.UpdateInteractionFreeParts(AwarenessLevel.OverlayNone);
            Target.LookAtManager.DisableLookAts();
            for (int i = 0; i < shots; i++)
            {
                debugger.Debug(this, "Seaching next pose");
                PoseData poseData = GetRandomPose();
                if (poseData == null)
                {
                    result = false;
                    break;
                }
                debugger.Debug(this, "Entering pose " + i + ": " + poseData.Name);

                Target.PlaySoloAnimation(poseData.Key, false);

                bool userCanceled = Target.WaitForExitReason(simMinutesToPose, ExitReason.UserCanceled);
                if (userCanceled)
                {
                    debugger.Debug(this, "User canceled posing");
                    result = false;
                    break;
                }
            }
            Target.LookAtManager.EnableLookAts();
            debugger.EndDebugLog();
            return result;
        }

        public PoseData GetRandomPose()
        {
            PoseData pose = null;
            int count=PoseData.sData.Count;
            if (count > 0)
            {
               pose= RandomUtil.GetRandomObjectFromList<PoseData>(new List<PoseData>(PoseData.sData.Values));
                
            }
            return pose;
        }


        private sealed class Definition : InteractionDefinition<Sim, Sim, SnapshotPose>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseData.HasPoseData && !isAutonomous;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new String[] { "Posing..." };
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Snapshot Poses ("+shots+")";
            }
        }
    }
}

