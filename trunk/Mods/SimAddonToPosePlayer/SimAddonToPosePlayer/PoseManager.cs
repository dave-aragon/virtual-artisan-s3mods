using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.CmoPoseBox;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Actors;
using Misukisu.PosePlayerAddon;
using Sims3.Gameplay.Interactions;

namespace Misukisu.PosePlayerAddon
{
    class PoseManager
    {
        public static Dictionary<Sim, string> CurrentPoses = new Dictionary<Sim, string>();
        public static CmoPoseBox PoseBox = null;
        public static Sim LastPoser = null;

        public PoseManager()
            : base()
        {
            PoseBox = FindPoseBox();

            List<SimDescription> sims = Household.AllSimsLivingInWorld();
            foreach (SimDescription sim in sims)
            {
                Sim aSim = sim.CreatedSim;
                if (aSim != null)
                {
                    aSim.AddInteraction(StartPoseFromList.Singleton);
                    aSim.AddInteraction(StopPosing.Singleton);
                    aSim.AddInteraction(StartQuickPosing.Singleton);
                    aSim.AddInteraction(StartQuickPosingFromList.Singleton);
                    aSim.AddInteraction(StartTakingSamePoseAs.Singleton);
                    aSim.AddInteraction(PutPoseToList.Singleton);
                    aSim.AddInteraction(RemovePoseFromList.Singleton);

                    aSim.AddInteraction(MoveUp.Singleton);
                    aSim.AddInteraction(MoveUpUserDefined.Singleton);
                    aSim.AddInteraction(MoveDown.Singleton);
                    aSim.AddInteraction(MoveDownUserDefined.Singleton);
                    aSim.AddInteraction(TurnLeft.Singleton);
                    aSim.AddInteraction(TurnRight.Singleton);
                    aSim.AddInteraction(TurnAround.Singleton);
                    aSim.AddInteraction(MoveBack.Singleton);
                    aSim.AddInteraction(MoveBackUserDefined.Singleton);
                    aSim.AddInteraction(MoveForward.Singleton);
                    aSim.AddInteraction(MoveForwardUserDefined.Singleton);
                    aSim.AddInteraction(MoveRight.Singleton);
                    aSim.AddInteraction(MoveLeft.Singleton);
                }
            }

        }

        public static bool IsPoseBoxAvailable()
        {
            if (PoseManager.PoseBox == null)
            {
                PoseManager.FindPoseBox();
            }
            return (PoseManager.PoseBox != null);
        }

        public static string[] GetPoseMenuPath()
        {
            return new String[] { "Photo Shooting..." };
        }


        public static void SimStoppedPosing(Sim sim)
        {
            CurrentPoses.Remove(sim);
            if (sim == LastPoser)
            {
                LastPoser = null;
            }
        }

        public static void SetCurrentPose(Sim sim, string poseKey)
        {
            if (poseKey != null)
            {
                if (CurrentPoses.ContainsKey(sim))
                {
                    SimStoppedPosing(sim);
                }
                CurrentPoses.Add(sim, poseKey);
                LastPoser = sim;
            }
            else
            {
                SimStoppedPosing(sim);
            }
        }

        internal static string GetCurrentPose(Sim target)
        {
            string posename = null;
            if (CurrentPoses.ContainsKey(target))
            {
                posename = CurrentPoses[target];
            }
            return posename;
        }

        public static CmoPoseBox FindPoseBox()
        {
            CmoPoseBox box = null;
            List<CmoPoseBox> poseboxes = new List<CmoPoseBox>(Sims3.Gameplay.Queries.GetObjects<CmoPoseBox>());
            if (poseboxes.Count > 0)
            {
                box = poseboxes[0];
            }

            PoseBox = box;
            return box;
        }

        public static void CancelAllPosingActions(Sim Target)
        {
            Target.AddExitReason(ExitReason.UserCanceled);
            Target.InteractionQueue.CancelAllInteractionsByType(PlayPoseFromList.Singleton);
            Target.InteractionQueue.CancelAllInteractionsByType(TakeSamePoseAs.Singleton);
            Target.InteractionQueue.CancelAllInteractionsByType(QuickPose.Singleton);
            Target.InteractionQueue.CancelAllInteractionsByType(QuickPoseFromMyList.Singleton);
        }

        public static bool IsPosing(Sim target)
        {
            InteractionInstance currentAction = target.InteractionQueue.GetCurrentInteraction();
            if (currentAction is PlayPoseFromList
                || currentAction is QuickPose
                || currentAction is QuickPoseFromMyList
                || currentAction is TakeSamePoseAs)
            {
                return true;
            }
            else
            {
                SimStoppedPosing(target);
            }

            return false;
        }


    }
}
