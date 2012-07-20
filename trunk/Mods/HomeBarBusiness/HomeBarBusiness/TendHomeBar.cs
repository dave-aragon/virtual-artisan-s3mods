using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace.RouteDestinations;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay;

using Sims3.Gameplay.TuningValues;

namespace Misukisu.HomeBarBusiness
{

    public class TendHomeBar : Interaction<Sim, BarProfessional>
    {
        public static Dictionary<string, EventListener> listeners = new Dictionary<string, EventListener>();

        public class Definition : InteractionDefinition<Sim, BarProfessional, TendHomeBar>, IAllowedOnClosedVenues
        {

            public override string GetInteractionName(Sim actor, BarProfessional target, InteractionObjectPair iop)
            {
                //string menuName=BarProfessional.LocalizeString("TendHomeBarCommercial", new object[0]);
                //ActivateBusiness.debugger.Debug(this, "Menu name is "+ menuName);

                return "Tend The Bar";
            }
            public override bool Test(Sim a, BarProfessional target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous)
                {
                    return false;
                }
                return true;
                if (a.SimDescription.AssignedRole is Bartending.Bartender && a.SimDescription.AssignedRole.RoleGivingObject != target)
                {
                    return false;
                }
                if (a.IsSelectable)
                {
                    if (isAutonomous)
                    {
                        return false;
                    }
                    if (target.LotCurrent.IsCommunityLot && !target.LotCurrent.IsOpenVenue())
                    {
                        return false;
                    }
                }
                if (!Bartending.CanWorkAsBartendender(a, target.LotCurrent))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(BarProfessional.LocalizeString("BarTendHomeBarTakeOverGreyedOut", new object[0]));
                    return false;
                }
                return (a.IsSelectable && (target.mBartender == null || !target.mBartender.IsSelectable)) || target.mBartender == null;
            }
        }

        public static InteractionDefinition Singleton = new TendHomeBar.Definition();

        private void AddNeededSkills(Sim sim)
        {
            SkillManager skillManager = sim.SkillManager;
            if (skillManager != null)
            {
                Skill skill = skillManager.GetSkill<Skill>(SkillNames.Bartending);
                if (skill == null)
                {
                    skillManager.AddAutomaticSkill(SkillNames.Bartending);
                    skill = skillManager.GetSkill<Skill>(SkillNames.Bartending);
                }

                if (skill != null)
                {
                    skill.SkillLevel = skill.MaxSkillLevel;
                    //Message.Sender.Show("Maxed the dancing skills!");
                }
            }

        }

        public static ListenerAction OnDrinkOrdered(Event e)
        {
            try
            {
                Sims3.Gameplay.Skills.Bartending.MixedDrinkEvent order = e as Sims3.Gameplay.Skills.Bartending.MixedDrinkEvent;
                if (order != null)
                {
                    IActor bartender = e.Actor;
                    Household houseHold = Household.ActiveHousehold;
                    if (bartender.LotCurrent == houseHold.LotHome)
                    {

                        if (isBartender(bartender))
                        {
                            int price = GetCostForDrink(order.Drink);
                            Lot lotCurrent = bartender.LotCurrent;

                            List<Sim> sims = lotCurrent.GetObjectsInRoom<Sim>(bartender.RoomId);
                            bool payerFound = false;

                            foreach (Sim current in sims)
                            {

                                InteractionInstance action = current.InteractionQueue.GetCurrentInteraction();
                                //ActivateBusiness.debugger.Debug("Testing", "action is "+ action.ToString());
                                if (action is BarProfessional.OrderDrink || action is BarProfessional.OrderDrinkOnResidentalLot)
                                {
                                    payerFound = true;
                                    if (!houseHold.Contains(current.SimDescription))
                                    {
                                        PayDrink(current, bartender, price);
                                        //ActivateBusiness.debugger.Debug("OnDringOrder", current.Name + " paying " + price+" to bartender " + bartender.Name);
                                    }
                                    break;
                                }



                            }

                            if (!payerFound)
                            {
                                bartender.ModifyFunds(15);
                                //ActivateBusiness.debugger.Debug("OnDringOrder", "Paying " + price + " to bartender " + bartender.Name);
                            }
                        }

                    }
                }
                string text = e.ToDetailedString();
            }
            catch (Exception ex)
            {

            }
            return ListenerAction.Keep;
        }

        public static int GetCostForDrink(Bartending.Drink drink)
        {
            float num = 5f;
            //ActivateBusiness.debugger.Debug("OnDringOrder", "drink is " + drink.ToString());
            if (drink.IsDataDrink)
            {
                //ActivateBusiness.debugger.Debug("OnDringOrder", "Drinks is datadrink");
                Bartending.DrinkData drinkData = Bartending.GetDrinkData(drink.DataKey);
                if (drinkData != null)
                {
                    //ActivateBusiness.debugger.Debug("OnDringOrder", "datafound");
                    num = (float)drinkData.Price;
                }
            }
            else
            {
                //ActivateBusiness.debugger.Debug("OnDringOrder", "Drinks is mooddrink");
                Bartending.MoodData moodData = Bartending.GetMoodData(drink.Mood);
                if (moodData != null)
                {
                    //ActivateBusiness.debugger.Debug("OnDringOrder", "datafound");
                    num = (float)moodData.Price;
                }
            }


            return (int)num;
        }


        public static void PayDrink(Sim actor, IActor bartender, int price)
        {
            //tipAmount = 0;


            //SkillManager skillManager = bartender.SkillManager;
            //if (skillManager != null)
            //{
            //    Skill skill = skillManager.GetSkill<Skill>(SkillNames.Bartending);
            //    if (skill != null)
            //    {
            //       int level = skill.SkillLevel;
            //        for(int i =0; i < level
            //    }
            //}
            //if (Bartending.HasTabOpen(actor, bartender.LotCurrent))
            //{
            //    num = 0;
            //}
            //else
            //{
            //if (actor.HasTrait(Sims3.Gameplay.ActorSystems.TraitNames.WateringHoleRegular))
            //{
            //    num = (int)((float)num * TraitTuning.WateringHoleRegularPriceMultiplier);
            //}
            //}
            //float celebrityDiscount = actor.CelebrityManager.GetCelebrityDiscount(true);
            //tipAmount = Bartending.GetTipAmount(actor, instanceActor, drink, this.Target.LotCurrent.GetMetaAutonomyType, this.mDescription);

            //if (Bartending.HasTabOpen(actor, actor.LotCurrent))
            //{
            //    Bartending.AddToTab(actor.LotCurrent, num);
            //    //if (actor.IsSelectable)
            //    //{
            //    //    actor.ModifyFunds(-tipAmount);
            //    //}
            //}
            //else
            //{

            if (price > actor.FamilyFunds)
            {
                actor.ShowTNSIfSelectable(BarProfessional.LocalizeString(actor.IsFemale, "CantPayForDrink", new object[0]),
                    StyledNotification.NotificationStyle.kSimTalking, bartender.ObjectId, actor.ObjectId);

            }
            else
            {
                actor.ModifyFunds(-price);
                bartender.ModifyFunds(price);
            }

            //int cost = num + tipAmount;
            //if (!CelebrityManager.TryModifyFundsWithCelebrityDiscount(actor, bar, cost, celebrityDiscount, true))
            //{
            //    return false;
            //}

            //}

        }


        private static bool isBartender(IActor actor)
        {
            List<BarProfessional> bars = actor.LotCurrent.GetObjectsInRoom<BarProfessional>(actor.RoomId);

            foreach (BarProfessional bar in bars)
            {
                if (bar.mBartender == actor)
                {
                    return true;
                }
            }
            return false;
        }




        public override bool Run()
        {

            EventListener listener = null;
            if (listeners.TryGetValue(Actor.FullName, out listener))
            {
                if (EventTracker.ContainsListener(listener))
                {
                    EventTracker.RemoveListener(listener);
                }
                listeners.Remove(Actor.FullName);
            }

            listener = EventTracker.AddListener(EventTypeId.kMixedDrink, new ProcessEventDelegate(TendHomeBar.OnDrinkOrdered), this.Actor);
            listeners.Add(Actor.FullName, listener);


            //AddNeededSkills(this.Actor);
            MakeBusinessBar.EnableBarInteractions(Target);
            Sim mBartender = this.Target.mBartender;
            bool flag = false;
            if (this.Target.IsBartenderReplacement())
            {
                flag = true;
            }
            //if (!Bartending.IsSimAllowedToBartend(this.Actor, this.Target.LotCurrent.GetMetaAutonomyType))
            //{
            //    ObjectGuid thumbnailObject = ObjectGuid.InvalidObjectGuid;
            //    StyledNotification.NotificationStyle style = StyledNotification.NotificationStyle.kGameMessagePositive;
            //    if (mBartender != null)
            //    {
            //        thumbnailObject = mBartender.ObjectId;
            //        style = StyledNotification.NotificationStyle.kSimTalking;
            //    }
            //    this.Actor.ShowTNSIfSelectable(BarProfessional.LocalizeString(this.Actor.IsFemale, "BarTendHomeBarTakeOverFail", new object[0]), style, thumbnailObject, this.Actor.ObjectId);
            //    return false;
            //}
            if (flag)
            {
                RadialRangeDestination radialRangeDestination = new RadialRangeDestination();
                radialRangeDestination.mCenterPoint = this.Target.GetPositionOfSlot(Slot.RoutingSlot_0);
                radialRangeDestination.mfConeAngle = BarProfessional.Tend.kRouteToReplaceAngle;
                radialRangeDestination.mConeVector = -this.Target.GetForwardOfSlot(Slot.RoutingSlot_0);
                radialRangeDestination.mfMinRadius = BarProfessional.Tend.kRouteToReplaceRadiusMinimum;
                radialRangeDestination.mfMaxRadius = BarProfessional.Tend.kRouteToReplaceRadiusMaximum;
                radialRangeDestination.mfPreferredRadius = BarProfessional.Tend.kRouteToReplaceRadiusPreferred;
                radialRangeDestination.mTargetObject = this.Target;
                radialRangeDestination.mFacingPreference = RouteOrientationPreference.TowardsObject;
                Route route = this.Actor.CreateRoute();
                route.AddDestination(radialRangeDestination);
                route.SetValidRooms(this.Target.LotCurrent.LotId, new int[]
			{
				this.Target.RoomId
			});
                route.Plan();
                if (!this.Actor.DoRoute(route))
                {
                    return false;
                }
            }
            else
            {
                if (!this.Target.RouteToBarAsBartender(this.Actor))
                {
                    return false;
                }
            }
            if (!this.Test())
            {
                this.Actor.RouteAway(BarProfessional.Tend.kRouteAwayDistanceMinimum, BarProfessional.Tend.kRouteAwayDistanceMaximum, false, base.GetPriority(), false, false, true, RouteDistancePreference.NoPreference);
                return false;
            }
            if (flag)
            {
                this.Actor.LoopIdle();
                while (mBartender.CurrentInteraction != null && !(mBartender.CurrentInteraction is BarProfessional.IdleTend))
                {
                    Simulator.Sleep(0u);
                    if (this.Actor.HasExitReason())
                    {
                        return false;
                    }
                }
                this.Actor.ShowTNSIfSelectable(BarProfessional.LocalizeString(this.Actor.IsFemale, "BarTendHomeBarTakeOver", new object[]
			{
				this.Actor
			}), StyledNotification.NotificationStyle.kSimTalking, mBartender.ObjectId, this.Actor.ObjectId);
                this.Target.mBartender = this.Actor;
                mBartender.InteractionQueue.CancelAllInteractions();
                mBartender.RouteAway(BarProfessional.Tend.kRouteAwayDistanceMinimum, BarProfessional.Tend.kRouteAwayDistanceMaximum, false, base.GetPriority(), false, false, true, RouteDistancePreference.NoPreference);
            }
            bool hasSwitchedIntoBarTendHomeBarerOutfit = false;
            this.Actor.Posture = new BarProfessional.TendingPosture(this.Actor, this.Target, hasSwitchedIntoBarTendHomeBarerOutfit);
            this.Actor.BridgeOrigin = this.Actor.Posture.Idle();
            InteractionInstance continuation = BarProfessional.IdleTend.Singleton.CreateInstance(this.Target, this.Actor, base.GetPriority(), true, true);
            return base.TryPushAsContinuation(continuation);
        }
    }



}
