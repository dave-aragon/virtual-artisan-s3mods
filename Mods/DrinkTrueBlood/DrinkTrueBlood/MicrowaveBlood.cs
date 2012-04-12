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
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Objects.Seating;
using Sims3.SimIFace.Enums;

namespace Misukisu.Interactions
{
    public class WarmUpBloodInMicrowave : Interaction<Sim, Microwave>
    {

        public override string GetInteractionName()
        {
            return "Have Tru Blood";
        }

        public class Definition : InteractionDefinition<Sim, Microwave, WarmUpBloodInMicrowave>
        {
            public override string GetInteractionName(Sim a, Microwave target, InteractionObjectPair interaction)
            {
                return "Tru Blood";
            }

            public override string[] GetPath(bool isFemale)
            {
                string[] menuPath = new string[]
				{
					Food.GetString(Food.StringIndices.HaveSnack) + Localization.Ellipsis
				};
                return menuPath;
            }

            public override bool Test(Sim a, Microwave target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !target.InUse && target.Parent is Counter;
            }
        }
        public static InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
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

        public override string GetInteractionName()
        {
            return "Have Tru Blood";
        }

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

        public override void CleanupAfterExitReason()
        {
            if (this.Actor.IsHoldingAnything())
            {
                if (this.Actor.CarryStateMachine == null)
                {
                    CarrySystem.EnterWhileHolding(this.Actor, this.Actor.GetObjectInRightHand() as ICarryable);
                }
                Food.PutHeldObjectDownOnCounterTableOrFloor(this.Actor, SurfaceType.Normal);
            }
            base.CleanupAfterExitReason();
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
                TrueBlood trueBlood = GlobalFunctions.CreateObjectOutOfWorld(this.ChosenRecipe.ObjectToCreateInFridge, this.ChosenRecipe.CodeVersion) as TrueBlood;
                GameObject gameObject = trueBlood as GameObject;
                gameObject.AddToUseList(this.Actor);
                try
                {
                    this.Target.PutOnFridgeShelf(gameObject);
                    trueBlood.InitializeForRecipe(this.ChosenRecipe);
                    Recipe.MealDestination destination = Recipe.MealDestination.SurfaceOrEat;
                    CookingProcess process = new CookingProcess(ChosenRecipe, new List<Ingredient>(), Target, Actor.LotCurrent, destination,
                         Recipe.MealQuantity.Single, Recipe.MealRepetition.MakeOne, "Tru Blood", new String[] { }, trueBlood, Actor, false);
                    trueBlood.CookingProcess = process;
                    CookingProcess.MoveToNextStep(trueBlood, Actor);
                    base.SetActor(trueBlood.ActorNameForFridge, gameObject);

                    base.AnimateSim("Remove - " + trueBlood.ActorNameForFridge);
                }
                catch
                {
                    gameObject.Destroy();
                    throw;
                }
                CarrySystem.EnterWhileHolding(this.Actor, trueBlood, false);
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
                    return target.CookingProcess.InteractionTest(a, target);
                }
            }
            public static InteractionDefinition Singleton = new Definition();
            public override bool Run()
            {
                if (this.CheckForCancelAndCleanup())
                {
                    return false;
                }

                Microwave microwave = GlobalFunctions.GetClosestObject<Microwave>(Actor, false, true, null, null);
                if (microwave == null)
                {
                    return false;
                }
                if (Target.Parent == Actor)
                {
                    if (microwave.RouteToMicrowave(this))
                    {
                        string actorNameForMicrowave = (Target as IMicrowavable).ActorNameForMicrowave;
                        microwave.AddToUseList(Actor);
                        try
                        {
                            microwave.SimStateMachineClient.SetActor(actorNameForMicrowave, Target);
                            microwave.SimStateMachineClient.SetActor("x", Actor);
                            microwave.SimStateMachineClient.AddOneShotScriptEventHandler(1001u, new SacsEventHandler(microwave.StartLoopSoundCallback));
                            microwave.SimStateMachineClient.EnterState("x", "Enter - Holding " + actorNameForMicrowave);
                            microwave.SimStateMachineClient.RequestState("x", "Start Microwave");
                            microwave.MicrowaveSelfStateMachineClient.EnterState("Microwave", "Enter");
                            microwave.MicrowaveSelfStateMachineClient.RequestState("Microwave", "Loop - Cook");
                            microwave.On = true;
                            microwave.AddCookingAlarm((Target as IPartOfCookingProcess).CookingProcess.CookTimeLeftMinutes / microwave.CookTimeSpeedMultiplier);
                            microwave.SimStateMachineClient.RequestState("x", "Exit - Hands Empty");
                        }
                        finally
                        {
                            microwave.RemoveFromUseList(Actor);
                        }
                        if (Actor.HasExitReason(ExitReason.Canceled))
                        {
                            return false;
                        }
                        Actor.InteractionQueue.PushAsContinuation(TakeBloodFromMicrowave.Singleton, microwave, true);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                return true;

            }
        }
    }

    public class TakeBloodFromMicrowave : Interaction<Sim, Microwave>
    {
        public class Definition : InteractionDefinition<Sim, Microwave, TakeBloodFromMicrowave>
        {
            public override bool Test(Sim a, Microwave target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !target.InUse && target.Parent is Counter && target.GetContainedObject(Microwave.kContainmentSlot) is TrueBlood;
            }
        }

        public override string GetInteractionName()
        {
            return "Warm Tru Blood in Microwave";
        }

        public static InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
            if (this.CheckForCancelAndCleanup())
            {
                return false;
            }
            if (!this.Target.RouteToMicrowave(this, false))
            {
                return false;
            }
            if (this.CheckForCancelAndCleanup())
            {
                return false;
            }
            TrueBlood trueBlood = this.Target.GetContainedObject(Microwave.kContainmentSlot) as TrueBlood;
            if (trueBlood == null || trueBlood.CookingProcess == null)
            {
                return false;
            }
            base.StandardEntry();

            this.Target.SimStateMachineClient.EnterState("x", "Enter - Hands Empty");
            this.Target.SimStateMachineClient.SetActor("x", this.Actor);
            this.Target.SimStateMachineClient.SetActor("BowlLarge", trueBlood);
            bool flag = this.Actor.HasTrait(TraitNames.NaturalCook) &&
                this.Actor.SkillManager.GetSkillLevel(SkillNames.Cooking) >= TraitTuning.NaturalCookTraitLevelToEnhanceFood
                && RandomUtil.RandomChance01(TraitTuning.NaturalCookTraitChanceToPlayEnhanceAnimation);
            this.Target.SimStateMachineClient.SetParameter("isNaturalCook", flag);
            if (flag)
            {
                trueBlood.CookingProcess.NaturalCookEnhanced = true;
            }
            if (this.Actor.HasTrait(TraitNames.BornToCook))
            {
                trueBlood.CookingProcess.BornToCookEnhanced = true;
            }

            this.MicrowaveCookLoop(trueBlood);

            if (trueBlood.CookingProcess.IsDoneCooking)
            {
                this.CreateFinalCookingObjectAndExit(trueBlood);
            }
            else
            {
                this.Target.SimStateMachineClient.RequestState("x", "Exit - Hands Empty");
            }
            if (this.CheckForCancelAndCleanup())
            {
                return false;
            }
            Counter counter = this.Target.Parent as Counter;
            if (counter != null && counter.IsCleanable)
            {
                counter.Cleanable.DirtyInc(this.Actor);
            }
            base.StandardExit();
            return true;

        }

        public void MicrowaveCookLoop(IPartOfCookingProcess cookingObject)
        {
            this.Target.SimStateMachineClient.RequestState("x", "Loop");
            float nukeTime = (float)((int)cookingObject.CookingProcess.CookTimeLeftMinutes) / this.Target.CookTimeSpeedMultiplier;
            this.Target.CheckForAlarm(this.Actor, nukeTime);
            this.DoLoop(ExitReason.Default, delegate(StateMachineClient smc, InteractionInstance.LoopData ld)
            {
                if (cookingObject.CookingProcess.IsDoneCooking)
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
            }
            , null);
        }

        public bool CreateFinalCookingObjectAndExit(TrueBlood trueBlood)
        {
            this.Target.SimStateMachineClient.SetActor((trueBlood as IMicrowavable).ActorNameForMicrowave, trueBlood);
            this.Target.SimStateMachineClient.RequestState("x", "Exit - " + "BowlLarge");
            CarrySystem.EnterWhileHolding(this.Actor, trueBlood as ICarryable);
            if (this.CheckForCancelAndCleanup())
            {
                return false;
            }
            trueBlood.PushEatSnack(this.Actor);

            return true;
        }


        public override void CleanupAfterExitReason()
        {
            if (this.Actor.IsHoldingAnything())
            {
                if (this.Actor.CarryStateMachine == null)
                {
                    CarrySystem.EnterWhileHolding(this.Actor, this.Actor.GetObjectInRightHand() as ICarryable);
                }
                Food.PutHeldObjectDownOnCounterTableOrFloor(this.Actor, SurfaceType.Normal);
            }
            base.CleanupAfterExitReason();
        }
    }

}
