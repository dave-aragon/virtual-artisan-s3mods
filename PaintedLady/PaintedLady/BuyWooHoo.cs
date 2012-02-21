using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Misukisu.Common;
using Sims3.Gameplay.Objects.Misukisu;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay;
using Sims3.Gameplay.Socializing;
using Sims3.UI.Controller;
using Sims3.Gameplay.ActorSystems;

namespace Misukisu.Sims3.Gameplay.Interactions
{
    class BuyWooHoo : Interaction<Sim, Sim>
    {

        public static readonly InteractionDefinition Singleton = new Definition();
        private LongTermRelationshipTypes relationshipBeforeWooHoo;
        private global::Sims3.Gameplay.ActorSystems.BuffNames[] happyBuffs=new BuffNames[1]{BuffNames.StrideOfPride};

        protected override bool Run()
        {
            try
            {
                Sim buyer = base.Actor;
                Sim seller = base.Target;
                Message.Show("Ok, Let's Woo and Hoo then.");
                CreateRelationship(buyer, seller);

                Bed place = findNearestBed(seller);
                if (place != null)
                {
                    place.MakeSimsWooHooOnMe(buyer, seller);

                    seller.InteractionQueue.AddAfterCheckingForDuplicates(
                        PutClothesOnAndRestoreRelations.Singleton.CreateInstanceWithCallbacks(seller, seller, 
                        new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false,
                        new Callback(this.RestoreRelationshipAndPay), new Callback(this.DoNothing), new Callback(this.DoNothing)
                        ));

                    buyer.InteractionQueue.AddAfterCheckingForDuplicates(
                       PutClothesOnAndRestoreRelations.Singleton.CreateInstance(buyer, buyer,
                       new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false));
                }
            }
            catch (Exception e)
            {
                Message.ShowError(CourtesansPerfume.NAME, "Buy WooHoo failed", false, e);

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
                               
                Relationship relationToTarget = base.Actor.GetRelationship(base.Target, true);
                relationToTarget.LTR.ForceChangeState(relationshipBeforeWooHoo);
                Message.Show("Relations restored: " + relationToTarget.LTR.CurrentLTR + " - " + base.Target.GetRelationship(base.Actor, true).LTR.CurrentLTR);

                int amount = 100;
                if (amount < base.Actor.FamilyFunds)
                {
                    base.Actor.ModifyFunds(-amount);
                }


                //if (base.Actor.BuffManager.HasAnyElement(happyBuffs))
                //{
                //}
            }
            catch (Exception e)
            {
                Message.ShowError(CourtesansPerfume.NAME, "Restoring original relationships failed", false, e);

            }
        }

        private void CreateRelationship(Sim actor, Sim target)
        {
            Relationship relationToTarget = actor.GetRelationship(target, true);
            this.relationshipBeforeWooHoo = relationToTarget.LTR.CurrentLTR;
            relationToTarget.LTR.ForceChangeState(LongTermRelationshipTypes.Partner);
        }


        private Bed findNearestBed(Sim sim)
        {
            return GlobalFunctions.GetClosestObject<Bed>(sim, true, true, new List<Bed>(), null);

        }


        private sealed class Definition : InteractionDefinition<Sim, Sim, BuyWooHoo>
        {

            protected override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return "Buy WooHoo (§100)";
            }

            protected override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous)
                {
                    return false;
                }

                //SocialComponent social = target.SocialComponent;
                //if (social != null)
                //{
                //    Conversation conversation = social.Conversation;
                //    if (conversation != null && conversation.ContainsSim(target))
                //    {
                //        return true;
                //    }
                //}

                //if (target.LastTalkingTo == actor)
                //{
                //    return true;
                //}
                //Message.Show("Trying to buy woohoo is disabled");
                //return false;
                return true;
            }
        }
    }
}
