using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.CelebritySystem;
using System.Collections;
using Sims3.UI;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Objects.CookingObjects.Misukisu;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;


namespace Misukisu.Interactions
{
    class OrderTrueBlood : BarProfessional.BarInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public static Bartending.DrinkDescription DrinkDescription = new Bartending.DrinkDescription(Bartending.DrinkMood.Regular);
        public override global::Sims3.Gameplay.Interactions.InteractionDefinition GetBartenderInteraction()
        {
            return ServeTrueBlood.Singleton;
        }

        public override string GetInteractionName()
        {
            return "Order Tru Blood";
        }

        public override bool RunBehavior()
        {
            GameObject gameObject = this.mDestination.Object.GetContainedObject(this.mDestination.PlacementSlot) as GameObject;
            TrueBlood tb = gameObject as TrueBlood;

            if (tb != null)
            {
                int num;
                if (this.RunPaymentBehavior(out num))
                {
                    Sim instanceActor = this.LinkedInteractionInstance.InstanceActor;
                    Bartending bartending = (Bartending)instanceActor.SkillManager.AddElement(SkillNames.Bartending);
                    if (bartending != null)
                    {
                        Sim simInChargeOfBar = this.GetSimInChargeOfBar();
                        bartending.OnServedSim(this.Actor, simInChargeOfBar, num, Quality.Nice, this.Target.LotCurrent, DrinkDescription);
                        instanceActor.ModifyFunds(num);
                    }


                    if (CarrySystem.PickUpWithoutRouting(this.Actor, tb, true))
                    {
                        tb.PushEatHeldFoodInteraction(this.Actor);
                    }
                    else
                    {
                        gameObject.FadeOut(false, true);
                    }

                    return true;
                }
                gameObject.FadeOut(true, true);
            }
            return false;
        }

        public virtual Sim GetSimInChargeOfBar()
        {
            if (this.Target.CurrentRole != null)
            {
                return this.Target.CurrentRole.SimInRole;
            }
            return null;
        }

        public bool RunPaymentBehavior(out int tipAmount)
        {
            tipAmount = 0;
            Sim instanceActor = this.LinkedInteractionInstance.InstanceActor;
            int num = BarProfessional.GetCost(DrinkDescription, this.Target.LotCurrent.GetMetaAutonomyType, this.Actor, this.Target, true) * 1;
            if (num == 0)
            {
                return true;
            }
            float celebrityDiscount = this.Actor.CelebrityManager.GetCelebrityDiscount(true);
            tipAmount = GetTipAmount(this.Actor, instanceActor, this.Target.LotCurrent.GetMetaAutonomyType, DrinkDescription);
            if (Bartending.HasTabOpen(this.Actor, this.Target.LotCurrent))
            {
                Bartending.AddToTab(this.Target.LotCurrent, num);
                if (this.Actor.IsSelectable)
                {
                    this.Actor.ModifyFunds(-tipAmount);
                }
            }
            else
            {
                if (this.Actor.IsSelectable)
                {
                    if (num > this.Actor.FamilyFunds)
                    {
                        this.Actor.ShowTNSIfSelectable(BarProfessional.LocalizeString(this.Actor.IsFemale, "CantPayForDrink", new object[0]), StyledNotification.NotificationStyle.kSimTalking, instanceActor.ObjectId, this.Actor.ObjectId);
                        return false;
                    }
                    int cost = num + tipAmount;
                    if (!CelebrityManager.TryModifyFundsWithCelebrityDiscount(this.Actor, this.Target, cost, celebrityDiscount, true))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int GetTipAmount(Sim actor, Sim creator, Lot.MetaAutonomyType venueType, Bartending.DrinkDescription requestedDrink)
        {
            float num = 0f;
            if (venueType != Lot.MetaAutonomyType.Residential && !actor.IsSelectable)
            {
                Bartending.BarData barData;
                if (Bartending.TryGetBarData(venueType, out barData))
                {
                    float num2 = (float)Bartending.GetCostForDrink(requestedDrink, venueType);
                    float num3 = num2 * barData.PriceCapMultiplier;

                    if (num > num3)
                    {
                        creator.ShowTNSIfSelectable(Bartending.LocalizeString(creator.IsFemale, "BartenderExpensiveIngredients", new object[0]), StyledNotification.NotificationStyle.kSimTalking, actor.ObjectId, creator.ObjectId);
                    }
                    num += num2 * Bartending.GetTipMultiplierForMood(Bartending.DrinkMood.Regular);

                    float num5 = 0f;

                    if (actor.HasTrait(TraitNames.Frugal))
                    {
                        num5 *= Bartending.kTipMultiplierTraitFrugal;
                    }
                    num += num2 * num5;
                    num = Math.Min(num, num3);
                }
                Bartending skill = creator.SkillManager.GetSkill<Bartending>(SkillNames.Bartending);
                if (skill != null && skill.LifetimeOpportunityServedDrinksCompleted)
                {
                    num *= Bartending.kLifetimeOpportunityServedTipMultiplier;
                }
            }
            return (int)num;
        }

        public override bool RunOrderBehavior()
        {
            Bartending.ShowDrinkSpeechBalloon(this.Actor, this.Target.LotCurrent.GetMetaAutonomyType, DrinkDescription);
            ServeTrueBlood makeDrink = (ServeTrueBlood)this.LinkedInteractionInstance;

            return true;
        }

        public class Definition : InteractionDefinition<Sim, BarProfessional, OrderTrueBlood>
        {
            public override string GetInteractionName(Sim actor, BarProfessional target, InteractionObjectPair iop)
            {
                return "Tru Blood (§" + Bartending.GetCostForDrink(DrinkDescription, target.LotCurrent.GetMetaAutonomyType) + ")";
            }
            public override string[] GetPath(bool isFemale)
            {
                return new string[]
				{
					BarProfessional.LocalizeString("OrderDrink", new object[0])
				};
            }
            public override bool Test(Sim actor, BarProfessional target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                SimDescription theActor = actor.SimDescription;
                if (theActor != null)
                {
                    if (theActor.IsVampire == false)
                    {
                        return false;
                    }
                }

                if (!target.IsBartenderAvailable())
                {
                    return false;
                }
                if (actor == target.mBartender)
                {
                    return false;
                }
                if (target.LotCurrent.IsResidentialLot)
                {
                    return false;
                }
                if (isAutonomous)
                {
                   
                    if (BarProfessional.IsRunningBarInteraction(actor))
                    {
                        return false;
                    }
                    if (target.mBartender != null && !Bartending.CanWorkAsBartendender(target.mBartender, target.LotCurrent, true))
                    {
                        return false;
                    }
                    if (Bartending.HasTabOpen(actor, target.LotCurrent))
                    {
                        return false;
                    }

                }
                if (DrinkDescription != null)
                {
                    int num = BarProfessional.GetCost(DrinkDescription, target.LotCurrent.GetMetaAutonomyType, actor, target) * 1;
                    if (num > actor.FamilyFunds)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(BarProfessional.LocalizeString("NotEnoughMoney", new object[0]));
                        return false;
                    }
                }
                return true;
            }
        }
    }

    class ServeTrueBlood : BarProfessional.BartenderInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override string GetInteractionName()
        {
            return "Serve Tru Blood";
        }

        public override bool RunMakeBehavior()
        {
            Recipe recipe = Recipe.NameToRecipeHash["TrueBlood"];
            TrueBlood tb = GlobalFunctions.CreateObjectOutOfWorld(recipe.ObjectToCreateInFridge, recipe.CodeVersion) as TrueBlood;
            tb.ParentToSlot(this.Actor, Sim.ContainmentSlots.RightHand);
            CarrySystem.EnterWhileHolding(this.Actor, tb);
            return true;
        }

        public class Definition : InteractionDefinition<Sim, BarProfessional, ServeTrueBlood>
        {
            public override bool Test(Sim actor, BarProfessional target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
