﻿using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Misukisu.Sims3.Gameplay.Interactions.Paintedlady;
using Sims3.Gameplay.Objects.Misukisu;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay;
using Sims3.Gameplay.Socializing;
using Sims3.UI.Controller;
using Sims3.Gameplay.ActorSystems;
using Misukisu.Paintedlady;
using Sims3.Gameplay.Roles.Misukisu;
using Sims3.Gameplay.Core;

namespace Misukisu.Sims3.Gameplay.Interactions.Paintedlady
{
    class BuyWooHoo : Interaction<Sim, Sim>
    {

        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            try
            {
                Sim buyer = base.Actor;
                Sim seller = base.Target;

                CreateRelationship(buyer, seller);

                Bed place = FindNearestBed(seller);
                if (place != null)
                {
                    Pay(buyer, seller);
                    PushWooHooOnBed(seller, buyer, place, new Callback(this.CleanupActions));
                    //InteractionInstance sellerCleanupAction = AfterWooHooCleanup.Singleton.CreateInstanceWithCallbacks(seller, seller,
                    //    new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false,
                    //    new Callback(this.RestoreRelationship), new Callback(this.DoNothing), new Callback(this.DoNothing)
                    //    );
                    //sellerCleanupAction.MustRun = true;
                    //seller.InteractionQueue.AddAfterCheckingForDuplicates(sellerCleanupAction);

                    //InteractionInstance buyerCleanupAction =
                    //   AfterWooHooCleanup.Singleton.CreateInstance(buyer, buyer,
                    //   new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
                    //buyerCleanupAction.MustRun = true;
                    //buyer.InteractionQueue.AddAfterCheckingForDuplicates(buyerCleanupAction);
                }
                else
                {
                    Message.Sender.Show("All the beds in this lot are already in use");
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(base.Target, "Buy WooHoo failed", false, e);

            }
            return true;
        }
     
        private void PushWooHooOnBed(Sim actor, Sim recipient, Bed bed, Callback callback)
        {
            InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior);
            InteractionInstance relaxingAction = EnterRelaxing.Singleton.CreateInstance(bed, actor, priority, false, true);
            relaxingAction.MustRun = true;
            if (actor.InteractionQueue.Add(relaxingAction))
            {
                InteractionInstance entry = EnterRelaxing.Singleton.CreateInstance(bed, recipient, priority, false, true);
                entry.MustRun = true;
                recipient.InteractionQueue.Add(entry);
                InteractionInstance woohooAction = WooHoo.Singleton.CreateInstanceWithCallbacks(recipient, actor, priority, false, true
                    , new Callback(this.DoNothing),callback,callback);
                woohooAction.MustRun = true;
                woohooAction.GroupId = relaxingAction.GroupId;
                actor.InteractionQueue.Add(woohooAction);
            }
        }

        public void DoNothing(Sim s, float x)
        {
        }

        public void Pay(Sim buyer, Sim seller)
        {
            CourtesansPerfume perfume = Courtesan.GetPerfume(seller);
            if (perfume != null)
            {
                int amount = perfume.PricePerWoohoo;
                if (amount < base.Actor.FamilyFunds)
                {
                    base.Actor.ModifyFunds(-amount);
                }
            }
        }

        public void CleanupActions(Sim s, float x)
        {
            try
            {
                CourtesansPerfume perfume = Courtesan.GetPerfume(base.Target);
                if (perfume != null)
                {
                    perfume.restoreOldRelationship(base.Actor, base.Target);
                    base.Target.BuffManager.RemoveElement(BuffNames.StrideOfPride);
                    base.Target.BuffManager.RemoveElement(BuffNames.WalkOfShame);
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(base.Target, "Restoring original relationships failed", false, e);

            }

            try
            {
                Lot.MetaAutonomyType venueType = base.Actor.LotCurrent.GetMetaAutonomyType;
                Courtesan.SwitchToProperClothing(base.Actor, venueType);
                Courtesan.SwitchToProperClothing(base.Target, venueType);
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(base.Actor, "Cannot restore clothes", false, e);
            }

            
        }

        private void CreateRelationship(Sim buyer, Sim seller)
        {
            Relationship relationToTarget = buyer.GetRelationship(seller, true);
            CourtesansPerfume perfume = Courtesan.GetPerfume(seller);
            if (perfume != null)
            {
                perfume.storeOldRelationship(buyer, relationToTarget.LTR.CurrentLTR);
                relationToTarget.LTR.ForceChangeState(LongTermRelationshipTypes.Partner);
            }
        }


        private BedDouble FindNearestBed(Sim sim)
        {
            return GlobalFunctions.GetClosestObject<BedDouble>(sim, true, true, new List<BedDouble>(), null);

        }


        private sealed class Definition : InteractionDefinition<Sim, Sim, BuyWooHoo>
        {

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
            {

                string price = "0";
                Sim owner = null;
                CourtesansPerfume perfume = Courtesan.GetPerfume(target);
                if (perfume != null)
                {
                    price = perfume.PricePerWoohoo.ToString();

                    owner = perfume.SlaveOwner;
                }

                string name = "Ask to WooHoo for Money (§" + price + ")";
                if (owner == actor)
                {
                    name = "Ask to WooHoo";
                }

                return name;
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                bool result = false;
                CourtesansPerfume perfume = Courtesan.GetPerfume(target);
                if (perfume != null)
                {
                    if (perfume.SlaveOwner != null)
                    {
                        if (actor == perfume.SlaveOwner)
                        {
                            result = IsTalkingToOrAlreadyBuying(actor, target, result);
                        }
                    }
                    else if (!isAutonomous)
                    {
                        result = IsTalkingToOrAlreadyBuying(actor, target, result);
                    }
                }
                else {
                    if (Message.Sender.IsDebugging()) {
                        Message.Sender.Debug(this, "action is not enabled since perfume does not exist");
                    }
                }

                return result;
            }

            private static bool IsTalkingToOrAlreadyBuying(Sim actor, Sim target, bool result)
            {
                InteractionDefinition interaction = BuyWooHoo.Singleton;
                result = Courtesan.IsTalkingTo(actor, target, result);

                if (actor.InteractionQueue.HasInteractionOfTypeAndTarget(BuyWooHoo.Singleton, target))
                {
                    result = true;
                }
                return result;
            }


        }
    }
}