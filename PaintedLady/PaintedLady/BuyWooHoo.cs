using System;
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

                Bed place = findNearestBed(seller);
                if (place != null)
                {
                    Pay(buyer, seller);
                    PushWooHooOnBed(seller, buyer, place);
                    InteractionInstance sellerCleanupAction = AfterWooHooCleanup.Singleton.CreateInstanceWithCallbacks(seller, seller,
                        new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), false, false,
                        new Callback(this.RestoreRelationship), new Callback(this.DoNothing), new Callback(this.DoNothing)
                        );
                    sellerCleanupAction.MustRun = true;
                    seller.InteractionQueue.AddAfterCheckingForDuplicates(sellerCleanupAction);

                    InteractionInstance buyerCleanupAction =
                       AfterWooHooCleanup.Singleton.CreateInstance(buyer, buyer,
                       new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), false, false);
                    buyerCleanupAction.MustRun = true;
                    buyer.InteractionQueue.AddAfterCheckingForDuplicates(buyerCleanupAction);
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


        private void PushWooHooOnBed(Sim seller, Sim customer, Bed bed)
        {
            InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior);
            InteractionInstance relaxingAction = EnterRelaxing.Singleton.CreateInstance(bed, seller, priority, false, true);
            if (seller.InteractionQueue.Add(relaxingAction))
            {
                InteractionInstance entry = EnterRelaxing.Singleton.CreateInstance(bed, customer, priority, false, true);
                customer.InteractionQueue.Add(entry);
                InteractionInstance woohooAction = WooHoo.Singleton.CreateInstance(customer, seller,priority, false, true);
                woohooAction.GroupId = relaxingAction.GroupId;
                seller.InteractionQueue.Add(woohooAction);
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
                int amount = perfume.PayPerWoohoo;
                if (amount < base.Actor.FamilyFunds)
                {
                    base.Actor.ModifyFunds(-amount);
                }
            }
        }

        public void RestoreRelationship(Sim s, float x)
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


        private Bed findNearestBed(Sim sim)
        {
            return GlobalFunctions.GetClosestObject<Bed>(sim, true, true, new List<Bed>(), null);

        }


        private sealed class Definition : InteractionDefinition<Sim, Sim, BuyWooHoo>
        {

            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                string price = "0";
                Courtesan role = Courtesan.AssignedRole(target);
                if (role != null)
                {
                    price = role.GetPerfume().PayPerWoohoo.ToString();
                }

                return "Ask to WooHoo for Money (§" + price + ")";
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                bool result = false;
                if (!isAutonomous)
                {
                    InteractionDefinition interaction = BuyWooHoo.Singleton;
                    result = Courtesan.IsTalkingTo(actor, target, result);

                    if (actor.InteractionQueue.HasInteractionOfTypeAndTarget(BuyWooHoo.Singleton, target))
                    {
                        result = true;
                    }
                }

                return result;
            }


        }
    }
}
