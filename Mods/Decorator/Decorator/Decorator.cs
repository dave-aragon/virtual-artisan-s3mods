using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Abstracts;
using Misukisu.Decorator;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Interfaces;

namespace Sims3.Gameplay.Objects.Misukisu
{


    public sealed class PickObjectToMove : ImmediateInteraction<IActor, Decorator>
    {
        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<IActor, Decorator, PickObjectToMove>
        {
            public override string GetInteractionName(IActor a, Decorator target, InteractionObjectPair interaction)
            {
                bool isAlreadyOn = target.areThingsMoving();

                if (isAlreadyOn)
                {
                    return "Stop moving: " + target.GetMovingItemName();
                }
                else
                {
                    return "Select Object To Move...";
                }
            }



            public override bool Test(IActor a, Decorator target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
        public static readonly InteractionDefinition Singleton = new PickObjectToMove.Definition();
        public override bool Run()
        {
            bool isAlreadyOn = Target.areThingsMoving();
            if (isAlreadyOn)
            {
                this.Target.RemoveMoveActionsFromAll();
            }
            else
            {
                GameObject objectToMove = Target.ShowItemSelectionCombo();
                if (objectToMove != null)
                {
                    Target.AddMoveInteractions(objectToMove);
                }

            }
            return true;
        }

       
    }


    public sealed class StartDecorating : ImmediateInteraction<Sim, Decorator>
    {
        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, StartDecorating>
        {
            public override string GetInteractionName(Sim a, Decorator target, InteractionObjectPair interaction)
            {
                bool isAlreadyOn = target.areThingsMoving();

                if (isAlreadyOn)
                {
                    return "Remove Move Interactions From All Objects In Room";

                }
                else
                {
                    return "Add Move Interactions To All Objects In Room";
                }
            }



            public override bool Test(Sim a, Decorator target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous ;
            }
        }
        public static readonly InteractionDefinition Singleton = new StartDecorating.Definition();
        public override bool Run()
        {


            bool isAlreadyOn = Target.areThingsMoving();
            if (isAlreadyOn)
            {
                this.Target.RemoveMoveActionsFromAll();
            }
            else
            {
                this.Target.AddMoveActionsToAll();
            }
            return true;
        }

        
    }

    public class Decorator : Sims3.Gameplay.Abstracts.GameObject
    {
        //public static Debugger debugger = new Debugger("Testing");
        private List<GameObject> movingThings = new List<GameObject>();


        public bool areThingsMoving()
        {
            bool isAlreadyOn = false;
            if (movingThings.Count > 0)
            {
                isAlreadyOn = true;
            }

            return isAlreadyOn;
        }

        public override void OnStartup()
        {
            base.OnStartup();
            AddInteraction(StartDecorating.Singleton);
            AddInteraction(PickObjectToMove.Singleton);

            AddInteraction(MoveUpProxy.Singleton);
            AddInteraction(MoveUpUserDefinedProxy.Singleton);
            AddInteraction(MoveDownProxy.Singleton);
            AddInteraction(MoveDownUserDefinedProxy.Singleton);
            AddInteraction(TurnLeftProxy.Singleton);
            AddInteraction(TurnRightProxy.Singleton);
            AddInteraction(TurnAroundProxy.Singleton);
            AddInteraction(TiltFaceUpProxy.Singleton);
            AddInteraction(TiltBackProxy.Singleton);
            AddInteraction(TiltForwardProxy.Singleton);
            AddInteraction(TiltFaceDownProxy.Singleton);
            AddInteraction(TiltUserDefinedProxy.Singleton);
            AddInteraction(MoveBackProxy.Singleton);
            AddInteraction(MoveBackUserDefinedProxy.Singleton);
            AddInteraction(MoveForwardProxy.Singleton);
            AddInteraction(MoveForwardUserDefinedProxy.Singleton);
            AddInteraction(MoveRightProxy.Singleton);
            AddInteraction(MoveRightUserDefinedProxy.Singleton);
            AddInteraction(MoveLeftProxy.Singleton);
            AddInteraction(MoveLeftUserDefinedProxy.Singleton);
        }

        public override void AddBuildBuyInteractions(List<InteractionDefinition> buildBuyInteractions)
        {
            buildBuyInteractions.Add(PickObjectToMove.Singleton);
            buildBuyInteractions.Add(MoveUpProxy.Singleton);
            buildBuyInteractions.Add(MoveUpUserDefinedProxy.Singleton);
            buildBuyInteractions.Add(MoveDownProxy.Singleton);
            buildBuyInteractions.Add(MoveDownUserDefinedProxy.Singleton);
            buildBuyInteractions.Add(TurnLeftProxy.Singleton);
            buildBuyInteractions.Add(TurnRightProxy.Singleton);
            buildBuyInteractions.Add(TurnAroundProxy.Singleton);
            buildBuyInteractions.Add(TiltFaceUpProxy.Singleton);
            buildBuyInteractions.Add(TiltBackProxy.Singleton);
            buildBuyInteractions.Add(TiltForwardProxy.Singleton);
            buildBuyInteractions.Add(TiltFaceDownProxy.Singleton);
            buildBuyInteractions.Add(TiltUserDefinedProxy.Singleton);
            buildBuyInteractions.Add(MoveBackProxy.Singleton);
            buildBuyInteractions.Add(MoveBackUserDefinedProxy.Singleton);
            buildBuyInteractions.Add(MoveForwardProxy.Singleton);
            buildBuyInteractions.Add(MoveForwardUserDefinedProxy.Singleton);
            buildBuyInteractions.Add(MoveRightProxy.Singleton);
            buildBuyInteractions.Add(MoveRightUserDefinedProxy.Singleton);
            buildBuyInteractions.Add(MoveLeftProxy.Singleton);
            buildBuyInteractions.Add(MoveLeftUserDefinedProxy.Singleton);
            base.AddBuildBuyInteractions(buildBuyInteractions);
        }

        public void AddMoveActionsToAll()
        {
            List<GameObject> items = listAllObjectsInRoom(this.RoomId);
            foreach (GameObject gameObject in items)
            {
                if (gameObject != this)
                {
                    AddMoveInteractions(gameObject);
                   
                }
            }
        }

        public void RemoveMoveActionsFromAll()
        {
            List<GameObject> items = movingThings;
            foreach (GameObject gameObject in items)
            {
                RemoveMoveInteractions(gameObject);
                movingThings.Remove(gameObject);
            }

        }

        public void AddMoveInteractions(GameObject gameObject)
        {
            movingThings.Add(gameObject);
            //debugger.Debug(this, "adding interaction to " + gameObject.CatalogName);
            List<InteractionDefinition> actions = new List<InteractionDefinition>();
            actions.Add(MoveUp.Singleton);
            actions.Add(MoveUpUserDefined.Singleton);
            actions.Add(MoveDown.Singleton);
            actions.Add(MoveDownUserDefined.Singleton);
            actions.Add(TurnLeft.Singleton);
            actions.Add(TurnRight.Singleton);
            actions.Add(TurnAround.Singleton);
            actions.Add(TiltFaceUp.Singleton);
            actions.Add(TiltBack.Singleton);
            actions.Add(TiltForward.Singleton);
            actions.Add(TiltFaceDown.Singleton);
            actions.Add(TiltUserDefined.Singleton);
            actions.Add(MoveBack.Singleton);
            actions.Add(MoveBackUserDefined.Singleton);
            actions.Add(MoveForward.Singleton);
            actions.Add(MoveForwardUserDefined.Singleton);
            actions.Add(MoveRight.Singleton);
            actions.Add(MoveRightUserDefined.Singleton);
            actions.Add(MoveLeft.Singleton);
            actions.Add(MoveLeftUserDefined.Singleton);


            foreach (InteractionDefinition def in actions)
            {
                gameObject.AddInteraction(def);
            }
            //debugger.Debug(this, "Added regular action ");
            gameObject.AddBuildBuyInteractions(actions);

            //debugger.Debug(this, "Added build action ");
        }

        public static void RemoveMoveInteractions(GameObject gameObject)
        {
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


        private List<GameObject> listAllObjectsInRoom(int roomId)
        {
            //debugger.Debug(this, "listing object in room " + roomId);
            List<GameObject> result = new List<GameObject>();
            List<GameObject> gameObjectsInLot = this.LotCurrent.GetObjectsInRoom<GameObject>(roomId);

            //debugger.Debug(this, "Found items: " + gameObjectsInLot.Count);
            foreach (GameObject gameObject in gameObjectsInLot)
            {

                if (!(gameObject is Sim))
                {
                    result.Add(gameObject);
                }
            }
            return result;
        }

        public GameObject ShowItemSelectionCombo()
        {
            List<GameObject> gameObjectsInLot = listAllObjectsInRoom(this.RoomId);
            Dictionary<string, object> items = new Dictionary<string, object>();
            foreach (GameObject item in gameObjectsInLot)
            {
                if (item != this && !(item is Sim))
                {
                    items.Add(item.CatalogName + " (" + item.ObjectId + ")", item);
                }
            }
            object result = ComboSelectionDialog.Show("Select Object To Move", items, "none");
            GameObject itemToMove = result as GameObject;
            return itemToMove;
        }

      

        internal string GetMovingItemName()
        {
            string item = "nothing";
            if (movingThings.Count > 0)
            {
                item = movingThings[0].CatalogName;
            }

            return item;
        }

        internal GameObject GetItemToMove()
        {
            return movingThings[0];
        }

        internal bool HasItemsToMove()
        {
            if (movingThings.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}