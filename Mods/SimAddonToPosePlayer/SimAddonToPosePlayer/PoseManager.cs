using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.CmoPoseBox;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Actors;
using Misukisu.PosePlayerAddon;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;
using Sims3.Gameplay.EventSystem;

namespace Misukisu.PosePlayerAddon
{
    class PoseManager
    {
        public static Dictionary<GameObject, LocationVectors> MovedObjects = new Dictionary<GameObject, LocationVectors>();
        public static Dictionary<Sim, string> CurrentPoses = new Dictionary<Sim, string>();
        public static CmoPoseBox PoseBox = null;
        public static Sim LastPoser = null;


        public PoseManager()
            : base()
        {
            PoseBox = FindPoseBox();
            EventTracker.AddListener(EventTypeId.kSimInstantiated, new ProcessEventDelegate(NewSimIsHere));

            List<SimDescription> sims = Household.AllSimsLivingInWorld();
            foreach (SimDescription sim in sims)
            {
                Sim aSim = sim.CreatedSim;
                AddSimInteractions(aSim);
            }

        }

        public ListenerAction NewSimIsHere(Event e)
        {
            Sim sim = e.TargetObject as Sim;
            AddSimInteractions(sim);
            return ListenerAction.Keep;
        }

        private static void AddSimInteractions(Sim aSim)
        {
            if (aSim != null)
            {
                aSim.AddInteraction(StartPoseFromList.Singleton);
                aSim.AddInteraction(StartPoseByName.Singleton);
                aSim.AddInteraction(StartPosingFromMyList.Singleton);
                aSim.AddInteraction(StopPosing.Singleton);
                aSim.AddInteraction(ReleaseAllPosingSims.Singleton);
                aSim.AddInteraction(StartQuickPosing.Singleton);
                aSim.AddInteraction(StartQuickPosingFromList.Singleton);
                aSim.AddInteraction(StartTakingSamePoseAs.Singleton);
                aSim.AddInteraction(PutPoseToList.Singleton);
                aSim.AddInteraction(RemovePoseFromList.Singleton);
                aSim.AddInteraction(MoveObject.Singleton);
                //aSim.AddInteraction(React.Singleton);
                aSim.AddInteraction(PoseLookingAt.Singleton);
                aSim.AddInteraction(StopMovingObjects.Singleton);

                AddMoveInteractions(aSim);
            }
        }

        public static bool Pose(Sim actor, CmoPoseBox poseBox, String poseName)
        {
            if (poseName == null || poseName == "")
            {
                return false;
            }
            SetCurrentPose(actor, poseName);
            actor.OverlayComponent.UpdateInteractionFreeParts(AwarenessLevel.OverlayNone);
            actor.LookAtManager.DisableLookAts();
            poseBox.PlaySoloAnimation(actor.SimDescription.IsHuman, actor, poseName, true, ProductVersion.BaseGame);
            actor.ResetAllAnimation();
            poseBox.PlaySoloAnimation(actor.SimDescription.IsHuman, actor, poseName, true, ProductVersion.BaseGame);
            actor.ResetAllAnimation();
            actor.WaitForExitReason(3.40282347E+38f, ExitReason.UserCanceled);
            actor.LookAtManager.EnableLookAts();
            return true;
        }

        public static void AddMoveInteractions(GameObject gameObject)
        {
            if (!(gameObject is Sim))
            {
                LocationVectors vectors = new LocationVectors();
                vectors.ForwardVector = new Vector3(gameObject.ForwardVector);
                vectors.Position = new Vector3(gameObject.Position);
                MovedObjects.Add(gameObject, vectors);
                gameObject.AddInteraction(StopMovingMe.Singleton);
            }

            gameObject.AddInteraction(MoveUp.Singleton);
            gameObject.AddInteraction(MoveUpUserDefined.Singleton);
            gameObject.AddInteraction(MoveDown.Singleton);
            gameObject.AddInteraction(MoveDownUserDefined.Singleton);
            gameObject.AddInteraction(TurnLeft.Singleton);
            gameObject.AddInteraction(TurnRight.Singleton);
            gameObject.AddInteraction(TurnAround.Singleton);
            gameObject.AddInteraction(TiltFaceUp.Singleton);
            gameObject.AddInteraction(TiltBack.Singleton);
            gameObject.AddInteraction(TiltForward.Singleton);
            gameObject.AddInteraction(TiltFaceDown.Singleton);
            gameObject.AddInteraction(TiltUserDefined.Singleton);
            gameObject.AddInteraction(MoveBack.Singleton);
            gameObject.AddInteraction(MoveBackUserDefined.Singleton);
            gameObject.AddInteraction(MoveForward.Singleton);
            gameObject.AddInteraction(MoveForwardUserDefined.Singleton);
            gameObject.AddInteraction(MoveRight.Singleton);
            gameObject.AddInteraction(MoveRightUserDefined.Singleton);
            gameObject.AddInteraction(MoveLeft.Singleton);
            gameObject.AddInteraction(MoveLeftUserDefined.Singleton);
            gameObject.AddInteraction(TurnAtAngle.Singleton);
        }

        internal static void RestoreAllMovedObjects()
        {
            List<GameObject> keys = new List<GameObject>(MovedObjects.Keys);
            foreach (GameObject key in keys)
            {
                RemoveMoveInteractions(key, true);
            }
        }

        public static void RemoveMoveInteractions(GameObject gameObject, bool resetLocation)
        {
            if (MovedObjects.ContainsKey(gameObject))
            {
                if (resetLocation)
                {
                    LocationVectors vectors;
                    if (MovedObjects.TryGetValue(gameObject, out vectors))
                    {
                        gameObject.SetForward(vectors.ForwardVector);
                        gameObject.SetPosition(vectors.Position);
                    }
                }
                MovedObjects.Remove(gameObject);
            }
            gameObject.RemoveInteractionByType(StopMovingMe.Singleton);

            gameObject.RemoveInteractionByType(TurnLeft.Singleton);
            gameObject.RemoveInteractionByType(TurnAtAngle.Singleton);
            gameObject.RemoveInteractionByType(TurnRight.Singleton);
            gameObject.RemoveInteractionByType(TurnAround.Singleton);
            gameObject.RemoveInteractionByType(TiltFaceUp.Singleton);
            gameObject.RemoveInteractionByType(TiltBack.Singleton);
            gameObject.RemoveInteractionByType(TiltForward.Singleton);
            gameObject.RemoveInteractionByType(TiltFaceDown.Singleton);
            gameObject.RemoveInteractionByType(TiltUserDefined.Singleton);

            gameObject.RemoveInteractionByType(MoveUp.Singleton);
            gameObject.RemoveInteractionByType(MoveUpUserDefined.Singleton);
            gameObject.RemoveInteractionByType(MoveDown.Singleton);
            gameObject.RemoveInteractionByType(MoveDownUserDefined.Singleton);
            gameObject.RemoveInteractionByType(MoveBack.Singleton);
            gameObject.RemoveInteractionByType(MoveBackUserDefined.Singleton);
            gameObject.RemoveInteractionByType(MoveForward.Singleton);
            gameObject.RemoveInteractionByType(MoveForwardUserDefined.Singleton);

            gameObject.RemoveInteractionByType(MoveRight.Singleton);
            gameObject.RemoveInteractionByType(MoveRightUserDefined.Singleton);
            gameObject.RemoveInteractionByType(MoveLeft.Singleton);
            gameObject.RemoveInteractionByType(MoveLeftUserDefined.Singleton);
        }

        public static bool IsPoseBoxAvailable()
        {
            if (PoseBox != null && PoseBox.HasBeenDestroyed)
            {
                PoseBox = null;
            }

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
            Target.LookAtManager.ClearAllLookAts(true);
            Target.AddExitReason(ExitReason.UserCanceled);
            Target.InteractionQueue.CancelAllInteractionsByType(PoseByName.Singleton);
            Target.InteractionQueue.CancelAllInteractionsByType(PlayPoseFromList.Singleton);
            Target.InteractionQueue.CancelAllInteractionsByType(PoseFromMyList.Singleton);
            Target.InteractionQueue.CancelAllInteractionsByType(TakeSamePoseAs.Singleton);
            Target.InteractionQueue.CancelAllInteractionsByType(QuickPose.Singleton);
            Target.InteractionQueue.CancelAllInteractionsByType(QuickPoseFromMyList.Singleton);
            Target.InteractionQueue.CancelAllInteractionsByType(RepeatPoseWithLookAt.Singleton);

        }

        internal static void ReleaseAllPosers()
        {
            List<Sim> keys = new List<Sim>(CurrentPoses.Keys);
            foreach (Sim key in keys)
            {
                CancelAllPosingActions(key);
                SimStoppedPosing(key);
            }
        }

        public static bool IsPosing(GameObject gameObject)
        {
            Sim target = gameObject as Sim;
            if (target != null)
            {
                InteractionInstance currentAction = target.InteractionQueue.GetCurrentInteraction();
                if (currentAction is PlayPoseFromList
                    || currentAction is QuickPose
                    || currentAction is QuickPoseFromMyList
                    || currentAction is TakeSamePoseAs
                    || currentAction is PoseFromMyList
                    || currentAction is RepeatPoseWithLookAt
                    || currentAction is PoseByName
                   )
                {
                    return true;
                }
                else
                {
                    SimStoppedPosing(target);
                }

                return false;
            }
            else
            {
                // If nonsim thing has move interaction, it is usable until removed
                return true;
            }
        }




    }
}
