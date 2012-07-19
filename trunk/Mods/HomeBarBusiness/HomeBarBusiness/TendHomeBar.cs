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
using HomeBarBusiness;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay;

namespace Misukisu.HomeBarBusiness
{

    public class TendHomeBar : Interaction<Sim, BarProfessional>
    {
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
                ActivateBusiness.debugger.Debug("order ", "order taken");
                Sims3.Gameplay.Skills.Bartending.MixedDrinkEvent order = e as Sims3.Gameplay.Skills.Bartending.MixedDrinkEvent;
                if (order != null)
                {
                    IActor actor = e.Actor;
                    Household houseHold = Household.ActiveHousehold;
                    if (actor.LotCurrent == houseHold.LotHome)
                    {
                        ActivateBusiness.debugger.Debug("order ", "drink was made here");
                        BarProfessional bar = GlobalFunctions.GetClosestObject<BarProfessional>(actor, true, true, new List<BarProfessional>(), null);
                        if (bar.mBartender == actor)
                        {
                            ActivateBusiness.debugger.Debug("order ", "drink was made here");
                            foreach (Sim current in houseHold.LotHome.GetSims())
                            {
                                ActivateBusiness.debugger.Debug("order ", current.Name);
                                InteractionInstance action = current.InteractionQueue.GetCurrentInteraction();
                                if (action != null)
                                {
                                    // TODO: try this, it should work now
                                    ActivateBusiness.debugger.Debug("order ", "action: " + action.ToString());
                                }
                            }
                        }
                    }
                }
                string text = e.ToDetailedString();
                ActivateBusiness.debugger.Debug("order ", text);
            }
            catch (Exception ex)
            {
                ActivateBusiness.debugger.DebugError("order ", "failed!",ex);
            }
            return ListenerAction.Keep;
        }


        public override bool Run()
        {
            EventTracker.AddListener(EventTypeId.kMixedDrink, new ProcessEventDelegate(TendHomeBar.OnDrinkOrdered), this.Actor);
            ActivateBusiness.debugger.Debug("order ", "Tending the bar now");
            AddNeededSkills(this.Actor);
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
            ActivateBusiness.debugger.Debug("order ", "Going idletend");
            return base.TryPushAsContinuation(continuation);
        }
    }

    //public class WaitForOrders : Interaction<Sim, BarProfessional>
    //{
    //    public class Definition : InteractionDefinition<Sim, BarProfessional, WaitForOrders>
    //    {
    //        public override string GetInteractionName(Sim actor, BarProfessional target, InteractionObjectPair iop)
    //        {
    //            return string.Empty;
    //        }
    //        public override bool Test(Sim a, BarProfessional target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
    //        {
    //            return a.Posture.Container == target && !a.Posture.HasBeenCanceled;
    //        }
    //    }
    //    public static InteractionDefinition Singleton = new WaitForOrders.Definition();
    //    public override void ConfigureInteraction()
    //    {
    //        base.Hidden = true;
    //    }
    //    public override bool Run()
    //    {
    //        ActivateBusiness.debugger.Debug("order ", "Tending the IDLE now");
    //        if (!this.Target.RouteToBarAsBartender(this.Actor))
    //        {
    //            return false;
    //        }
    //        base.StandardEntry(false);
    //        base.BeginCommodityUpdates();
    //        base.EnterStateMachine("BarProfessional", "Enter", "x");
    //        base.SetActor("barProfessional", this.Target);
    //        base.AnimateSim("Tend - Tend Loop");
    //        bool flag = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.CheckForNextInteraction), this.mCurrentStateMachine);
    //        base.AnimateSim("Exit");
    //        base.EndCommodityUpdates(flag);
    //        base.StandardExit(false, false);
    //        if (flag && this.Actor.InteractionQueue.GetNextInteraction() == null)
    //        {
    //            this.Actor.InteractionQueue.PushAsContinuation(WaitForOrders.Singleton, this.Target, true);
    //        }

    //        if (this.Actor.InteractionQueue.GetNextInteraction() != null)
    //        {
    //            InteractionInstance interaction = this.Actor.InteractionQueue.GetNextInteraction();
    //            if (interaction.InteractionDefinition == BarProfessional.MakeDrink.Singleton || interaction.InteractionDefinition == BarProfessional.MakeRound.Singleton)
    //            {
    //                ActivateBusiness.debugger.Debug("order ", "make him pay! " + interaction.ToString());

    //            }
    //            else
    //            {
    //                ActivateBusiness.debugger.Debug("test ", "actino " + interaction.GetType().Name);
    //            }
    //        }
    //        return flag;
    //    }

    //    public void CheckForNextInteraction(StateMachineClient smc, InteractionInstance.LoopData ld)
    //    {
    //        if (this.Actor.InteractionQueue.GetNextInteraction() != null)
    //        {
    //            this.Actor.AddExitReason(ExitReason.CanceledByScript);
    //        }
    //    }

    //    public bool RunPaymentBehavior(Bartending.Drink drink, out int tipAmount)
    //    {
    //        tipAmount = 0;

    //        Sim instanceActor = this.LinkedInteractionInstance.InstanceActor;
    //        int num = 15;
    //        //int num = BarProfessional.GetCost(this.mDescription, this.Target.LotCurrent.GetMetaAutonomyType, this.Actor, this.Target, true) * this.mNumberOfDrinks;
    //        if (num == 0)
    //        {
    //            return true;
    //        }
    //        float celebrityDiscount = this.Actor.CelebrityManager.GetCelebrityDiscount(true);
    //        //tipAmount = Bartending.GetTipAmount(this.Actor, instanceActor, drink, this.Target.LotCurrent.GetMetaAutonomyType, this.mDescription);
    //        if (Bartending.HasTabOpen(this.Actor, this.Target.LotCurrent))
    //        {
    //            Bartending.AddToTab(this.Target.LotCurrent, num);
    //            if (this.Actor.IsSelectable)
    //            {
    //                this.Actor.ModifyFunds(-tipAmount);
    //            }
    //        }
    //        else
    //        {
    //            if (this.Actor.IsSelectable)
    //            {
    //                if (num > this.Actor.FamilyFunds)
    //                {
    //                    this.Actor.ShowTNSIfSelectable(BarProfessional.LocalizeString(this.Actor.IsFemale, "CantPayForDrink", new object[0]), StyledNotification.NotificationStyle.kSimTalking, instanceActor.ObjectId, this.Actor.ObjectId);
    //                    return false;
    //                }
    //                int cost = num + tipAmount;
    //                if (!CelebrityManager.TryModifyFundsWithCelebrityDiscount(this.Actor, this.Target, cost, celebrityDiscount, true))
    //                {
    //                    return false;
    //                }
    //            }
    //        }
    //        return true;
    //    }
    //}


}
