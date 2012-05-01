using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.ActorSystems;
using Sims3.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Objects.CmoPoseBox;

namespace Misukisu.PosePlayerAddon
{
    class PoseLookingAt : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            PoseManager.CancelAllPosingActions(Target);
            Target.InteractionQueue.AddNext(RepeatPoseWithLookAt.Singleton.CreateInstance(
                PoseManager.PoseBox, Target, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
            return true;
        }


        private sealed class Definition : InteractionDefinition<Sim, Sim, PoseLookingAt>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous && PoseManager.IsPoseBoxAvailable();
            }

            public override string[] GetPath(bool isFemale)
            {
                return PoseManager.GetPoseMenuPath();
            }


            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Look At...";
            }
        }
    }

    class RepeatPoseWithLookAt : Interaction<Sim, CmoPoseBox>
    {
        public static InteractionDefinition Singleton = new Definition();

        public class Definition : InteractionDefinition<Sim, CmoPoseBox, RepeatPoseWithLookAt>
        {
            public override string GetInteractionName(Sim actor, CmoPoseBox target, InteractionObjectPair iop)
            {
                return "Pose Looking At Other Sim";
            }
            public override bool Test(Sim actor, CmoPoseBox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }

        public override bool Run()
        {
            Sim lookAtTarget = ShowSimSelectionDialog(Actor);
            if (lookAtTarget == null)
            {
                return false;
            }

            string poseData = PoseManager.GetCurrentPose(Actor);
            if (poseData == null)
            {
                return false;
            }

            this.Actor.LookAtManager.EnableLookAts();
            this.Actor.LookAtManager.SetLookAt(LookAtManager.Type.Interaction, lookAtTarget, 20000, LookAtJointFilter.EyeBones);
            this.Actor.OverlayComponent.UpdateInteractionFreeParts(AwarenessLevel.OverlayUpperbody);
            this.Actor.LookAtManager.DisableLookAts();
            PoseManager.SetCurrentPose(Actor, poseData);
            Target.PlaySoloAnimation(this.Actor.SimDescription.IsHuman, this.Actor, poseData, true, ProductVersion.BaseGame);
            this.Actor.ResetAllAnimation();
            Target.PlaySoloAnimation(this.Actor.SimDescription.IsHuman, this.Actor, poseData, true, ProductVersion.BaseGame);
            this.Actor.ResetAllAnimation();
            this.Actor.WaitForExitReason(3.40282347E+38f, ExitReason.UserCanceled);
           
            return true;
        }

        public static Sim ShowSimSelectionDialog(Sim sim)
        {
            List<ObjectPicker.HeaderInfo> headers = new List<ObjectPicker.HeaderInfo>();
            headers.Add(new ObjectPicker.HeaderInfo("Sim", null, 400));


            List<ObjectPicker.RowInfo> tableData = new List<ObjectPicker.RowInfo>();

            int roomId = sim.RoomId;
            Sim[] gameObjectsInLot = sim.LotCurrent.GetObjects<Sim>();
            foreach (Sim gameObject in gameObjectsInLot)
            {
                if (gameObject.RoomId == roomId && gameObject != sim)
                {
                    ObjectPicker.RowInfo rowInfo = new ObjectPicker.RowInfo(gameObject, new List<ObjectPicker.ColumnInfo>());
                    ThumbnailKey thumbnail = gameObject.GetThumbnailKey();
                    rowInfo.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, gameObject.GetLocalizedName()));
                    tableData.Add(rowInfo);
                }
            }

            List<ObjectPicker.TabInfo> list3 = new List<ObjectPicker.TabInfo>();
            list3.Add(new ObjectPicker.TabInfo("shop_all_r2", "Select A Sim To Look At", tableData));
            string buttonOk = Localization.LocalizeString("Ui/Caption/Global:Ok", new object[0]);
            string buttonCancel = Localization.LocalizeString("Ui/Caption/Global:Cancel", new object[0]);
            List<ObjectPicker.RowInfo> userSelection = BigObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseSimulator,
                "Select A Sim To Look At", buttonOk, buttonCancel, list3, headers, 1);
            if (userSelection == null || userSelection.Count < 1)
            {
                return null;
            }
            return userSelection[0].Item as Sim;
        }
    }
}
