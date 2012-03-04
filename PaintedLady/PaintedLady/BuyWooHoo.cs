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
using Misukisu.Common;
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
                //Message.Sender.Show("Ok, Let's Woo and Hoo then.");
                CreateRelationship(buyer, seller);

                Bed place = findNearestBed(seller);
                if (place != null)
                {
                    place.MakeSimsWooHooOnMe(buyer, seller);

                    seller.InteractionQueue.AddAfterCheckingForDuplicates(
                        AfterWooHooCleanup.Singleton.CreateInstanceWithCallbacks(seller, seller,
                        new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false,
                        new Callback(this.RestoreRelationshipAndPay), new Callback(this.DoNothing), new Callback(this.DoNothing)
                        ));

                    buyer.InteractionQueue.AddAfterCheckingForDuplicates(
                       AfterWooHooCleanup.Singleton.CreateInstance(buyer, buyer,
                       new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false));
                }
                else
                {
                    Message.Sender.Show("All the beds in this lot are already in use");
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(CourtesansPerfume.NAME, "Buy WooHoo failed", false, e);

            }
            return true;
        }


        public void DoNothing(Sim s, float x)
        {
        }

        public void RestoreRelationshipAndPay(Sim s, float x)
        {
            try
            {
                 CourtesansPerfume perfume = Courtesan.GetPerfume(base.Target);
                 if (perfume != null)
                 {
                     perfume.restoreOldRelationship(s.SimDescription, base.Target);
                     if (base.Actor.BuffManager.HasElement(BuffNames.StrideOfPride))
                     {
                         int amount = 100;
                         if (amount < base.Actor.FamilyFunds)
                         {
                             base.Actor.ModifyFunds(-amount);
                         }
                     }
                     base.Target.BuffManager.RemoveElement(BuffNames.StrideOfPride);
                 }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(CourtesansPerfume.NAME, "Restoring original relationships failed", false, e);

            }
        }

        private void CreateRelationship(Sim buyer, Sim seller)
        {
            Relationship relationToTarget = buyer.GetRelationship(seller, true);
            CourtesansPerfume perfume = Courtesan.GetPerfume(seller);
            if (perfume != null)
            {
                perfume.storeOldRelationship(buyer.SimDescription, relationToTarget.LTR.CurrentLTR);
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
                Courtesan role=Courtesan.AssignedRole(target);
                if (role != null)
                {
                   price= role.GetPerfume().PayPerWoohoo.ToString();
                }

                return "Ask to WooHoo for Money (§"+price+")";
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous)
                {
                    return false;
                }

                SocialComponent social = target.SocialComponent;
                if (social != null)
                {
                    Conversation conversation = social.Conversation;
                    if (conversation != null && conversation.ContainsSim(actor))
                    {
                        return true;
                    }
                }

                if (actor.InteractionQueue.HasInteractionOfTypeAndTarget(BuyWooHoo.Singleton, target))
                {
                    return true;
                }
                
                return false;
            }
        }
    }
}
