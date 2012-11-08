using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;
using Sims3.UI;
using Sims3.SimIFace;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Objects.CmoPoseBox;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Abstracts;

namespace Misukisu.PosePlayerAddon
{
    class StopMovingMe : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();


        public override bool Run()
        {
            PoseManager.RemoveMoveInteractions(Target, false);
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, GameObject, StopMovingMe>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }

            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Stop Moving And Keep This Location";
            }
        }
    }

    class StopMovingObjects : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();


        public override bool Run()
        {
            PoseManager.RestoreAllMovedObjects();
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, StopMovingObjects>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.MovedObjects.Count > 0 && !isAutonomous;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Restore Locations of All Moved Objects";
            }
        }
    }

    class MoveObject : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();


        public override bool Run()
        {
            GameObject objectToMove = ShowObjectSelectionDialog(Target);
            if (objectToMove != null)
            {
                PoseManager.AddMoveInteractions(objectToMove);
            }

            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, MoveObject>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Move Object...";
            }
        }

        private GameObject ShowObjectSelectionDialog(Sim sim)
        {
            List<ObjectPicker.HeaderInfo> headers = new List<ObjectPicker.HeaderInfo>();
            headers.Add(new ObjectPicker.HeaderInfo("Object", null, 400));


            List<ObjectPicker.RowInfo> tableData = new List<ObjectPicker.RowInfo>();

            int roomId = sim.RoomId;
            listAllObjectsInRoom(sim, tableData, roomId);

            List<ObjectPicker.TabInfo> list3 = new List<ObjectPicker.TabInfo>();
            list3.Add(new ObjectPicker.TabInfo("shop_all_r2", "Select An Object", tableData));
            string buttonOk = Localization.LocalizeString("Ui/Caption/Global:Ok", new object[0]);
            string buttonCancel = Localization.LocalizeString("Ui/Caption/Global:Cancel", new object[0]);
            List<ObjectPicker.RowInfo> userSelection = BigObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseSimulator,
                "Select An Object To Move", buttonOk, buttonCancel, list3, headers, 1);
            if (userSelection == null || userSelection.Count < 1)
            {
                return null;
            }
            return userSelection[0].Item as GameObject;
        }

        private void listAllObjectsInRoom(Sim sim, List<ObjectPicker.RowInfo> tableData, int roomId)
        {
            GameObject[] gameObjectsInLot = sim.LotCurrent.GetObjects<GameObject>();
            foreach (GameObject gameObject in gameObjectsInLot)
            {
                if (gameObject.RoomId == roomId && !HasMoveInteraction(gameObject))
                {
                    ObjectPicker.RowInfo rowInfo = new ObjectPicker.RowInfo(gameObject, new List<ObjectPicker.ColumnInfo>());
                    ThumbnailKey thumbnail = gameObject.GetThumbnailKey();
                    rowInfo.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, gameObject.GetLocalizedName()));
                    tableData.Add(rowInfo);
                }
            }
        }

        private bool HasMoveInteraction(GameObject gameObject)
        {
            bool hasInteraction = false;
            List<InteractionObjectPair> interactions = gameObject.Interactions;
            Type type = MoveUp.Singleton.GetType();
            foreach (InteractionObjectPair iop in interactions)
            {
                if (iop.InteractionDefinition.GetType() == type)
                {
                    hasInteraction = true;
                    break;
                }
            }
            return hasInteraction;
        }
    }
}
