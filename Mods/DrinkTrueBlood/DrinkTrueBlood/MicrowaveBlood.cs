using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.CookingObjects.Misukisu;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Socializing;

namespace Misukisu.Interactions
{
    public class MicrowaveBlood : Interaction<Sim, Microwave>
    {
        public class Definition : InteractionDefinition<Sim, Microwave, MicrowaveBlood>
        {
            public override string GetInteractionName(Sim a, Microwave target, InteractionObjectPair interaction)
            {
                return "Warm up Tru:Blood";
            }

            public override bool Test(Sim a, Microwave target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
                //return Microwave.InteractionTestForPutInMicrowave(a, target);
            }
        }
        public static InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
            // return Microwave.InteractionBodyForPutInMicrowave(this);

            Fridge fridge = GlobalFunctions.GetClosestObject<Fridge>(Actor, false, true, null, null);
            if (fridge == null)
            {
                return false;
            }
            Actor.InteractionQueue.PushAsContinuation(TakeBloodFromFridge.Singleton, fridge, true);
           
            return true;

        }
    }

    public class TakeBloodFromFridge : Interaction<Sim, Fridge>
    {
        public ImpassableRegion mImpassableRegion = new ImpassableRegion();
        public static InteractionDefinition Singleton = new Definition();
        public class Definition : InteractionDefinition<Sim, Fridge, TakeBloodFromFridge>
        {
            public override bool Test(Sim actor, Fridge target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }

        public Recipe ChosenRecipe
        {
            get
            {
                return Recipe.NameToRecipeHash["TrueBlood"];
            }
        }

        public override void Cleanup()
        {
            this.mImpassableRegion.Cleanup();
            this.mImpassableRegion = null;
            base.Cleanup();
        }

        public override bool Run()
        {
            if (this.CheckForCancelAndCleanup())
            {
                return false;
            }
            if (!Target.RouteToOpen(this, true))
            {
                return false;
            }
            if (Target.InUse)
            {
                this.Actor.AddExitReason(ExitReason.RouteFailed);
                return false;
            }
            this.mImpassableRegion.AddMember(this.Actor);
            this.mImpassableRegion.AddMember(this.Target);
            this.mImpassableRegion.UpdateFootprint();
            base.StandardEntry();

            bool flag = true;
            List<Ingredient> ingredientsUsed = new List<Ingredient>();
            if (this.ChosenRecipe.UseUpIngredientsFrom(this.Actor, ref ingredientsUsed, Recipe.MealQuantity.Single) || this.Actor.IsNPC)
            {
                Fridge.EnterStateMachine(this);
                IRemovableFromFridgeAsInitialRecipeStep removableFromFridgeAsInitialRecipeStep = GlobalFunctions.CreateObjectOutOfWorld(this.ChosenRecipe.ObjectToCreateInFridge, this.ChosenRecipe.CodeVersion) as IRemovableFromFridgeAsInitialRecipeStep;
                GameObject gameObject = removableFromFridgeAsInitialRecipeStep as GameObject;
                gameObject.AddToUseList(this.Actor);
                try
                {
                    this.Target.PutOnFridgeShelf(gameObject);
                    removableFromFridgeAsInitialRecipeStep.InitializeForRecipe(this.ChosenRecipe);
                    base.SetActor(removableFromFridgeAsInitialRecipeStep.ActorNameForFridge, gameObject);

                    base.AnimateSim("Remove - " + removableFromFridgeAsInitialRecipeStep.ActorNameForFridge);
                }
                catch
                {
                    gameObject.Destroy();
                    throw;
                }
                CarrySystem.EnterWhileHolding(this.Actor, removableFromFridgeAsInitialRecipeStep, false);
                if (this.CheckForCancelAndCleanup())
                {
                    return false;
                }
                if (this.Actor.HasTrait(TraitNames.NaturalCook))
                {
                    TraitTipsManager.ShowTraitTip(13271263770231522448uL, this.Actor, TraitTipsManager.TraitTipCounterIndex.NaturalCook, TraitTipsManager.kNaturalCookCountOfMealsCooked);
                }

                InteractionDefinition warmUp = PutBloodToMicrowave.Singleton;
                Actor.InteractionQueue.PushAsContinuation(warmUp, gameObject, true);
                base.AnimateSim("Exit - Standing");
            }
            else
            {
                flag = false;
            }
            base.StandardExit();
            if (flag)
            {
                ActiveTopic.AddToSim(this.Actor, "Has Made Food");
            }
            return flag;
        }


        public class PutBloodToMicrowave : Interaction<Sim, TrueBlood>
        {
            public class Definition : InteractionDefinition<Sim, TrueBlood, PutBloodToMicrowave>
            {
                public override bool Test(Sim a, TrueBlood target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static InteractionDefinition Singleton = new Definition();
            public override bool Run()
            {
                // return Microwave.InteractionBodyForPutInMicrowave(this);

                Fridge fridge = GlobalFunctions.GetClosestObject<Fridge>(Actor, false, true, null, null);
                if (fridge == null)
                {
                    return false;
                }
                if (Target.Parent == Actor)
                {
                    if (!Microwave.PutInMicrowave(this))
                    {
                        return false;
                    }
                }

                return true;

            }
        }
    }

}
