using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.Counters;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using System.Collections.Generic;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay;
using Sims3.Gameplay.Utilities;
using System;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Misukisu;
using Misukisu.DancerSpot;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Roles.Misukisu;

namespace Misukisu.Sims3.Gameplay.Interactions
{


    public class PerformShow : Interaction<Sim, DancersStage>
    {
        // Fields
        private Slot mEnterSlot;

        private GameObject mReservedTile;
        public static readonly InteractionDefinition Singleton = new Definition();
        public bool Watchable = true;

        // Methods
        public override void Cleanup()
        {
            if (this.mReservedTile != null)
            {
                this.mReservedTile.Destroy();
                this.mReservedTile = null;
            }
            base.Cleanup();
        }

        private int ClosestRoutingSlotFirstComparer(Slot first, Slot second)
        {
            GameObject target = base.Target as GameObject;
            Vector3 position = base.Actor.Position;
            Vector3 positionOfSlot = target.GetPositionOfSlot(first);
            Vector3 vector3 = target.GetPositionOfSlot(second);
            float num = (positionOfSlot - position).LengthSqr();
            float num2 = (vector3 - position).LengthSqr();
            return num.CompareTo(num2);
        }



        private static bool DanceSpotObstructed(IDanceOnCounterOrTableObject danceObject)
        {
            if (danceObject is TableBar)
            {
                if (danceObject.UseCount > 0)
                {
                    return true;
                }
            }
            else if (danceObject.InUse)
            {
                return true;
            }

            return false;
        }

        private static bool ObjectHasSpaceAboveIt(GameObject gameObj)
        {
            LotLocation location = new LotLocation();
            if (World.GetLotLocation(gameObj.Position, ref location) == 0L)
            {
                return false;
            }
            foreach (ObjectGuid guid in World.GetObjects(gameObj.LotCurrent.LotId, location))
            {
                IScriptProxy proxy = Simulator.GetProxy(guid);
                if (proxy != null)
                {
                    GameObject target = proxy.Target as GameObject;
                    if (target != null)
                    {
                        if (target is Sim)
                        {
                            return false;
                        }
                        if ((target.CheckObjectPlacement(UserToolUtils.BuildBuyProductType.Ceiling) || target.CheckObjectPlacement(UserToolUtils.BuildBuyProductType.Wall)) && (!(target is IPainting) && !(target is Windows)))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        protected override bool Run()
        {
            OutfitCategories[] outfits = base.Target.ShowOutfits;
            OutfitCategories startOutfit = base.Target.GetFirstOutfit();
            base.Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToWork, startOutfit);

            List<Slot> danceOnRoutingSlots = base.Target.GetDanceOnRoutingSlots(base.Actor);
            danceOnRoutingSlots.Sort(new Comparison<Slot>(this.ClosestRoutingSlotFirstComparer));
            for (int i = 0; i < danceOnRoutingSlots.Count; i++)
            {
                Slot slotName = danceOnRoutingSlots[i];
                Route r = base.Actor.CreateRoute();
                r.DoRouteFail = i == (danceOnRoutingSlots.Count - 1);
                r.PlanToPoint(base.Target.GetSlotPosition(slotName));
                if (base.Actor.DoRoute(r) && base.Actor.RouteTurnToFace(base.Target.Position))
                {
                    this.mEnterSlot = slotName;
                    break;
                }
            }
            if (this.mEnterSlot == Slot.None)
            {
                return false;
            }
            if (DanceSpotObstructed(base.Target))
            {
                return false;
            }

            Vector3 slotPosition = base.Target.GetSlotPosition(this.mEnterSlot);
            Vector3 forwardOfSlot = base.Target.GetForwardOfSlot(this.mEnterSlot);
            this.mReservedTile = GlobalFunctions.CreateObject("DynamicFootprintPlacement", slotPosition, 0, forwardOfSlot) as GameObject;

            base.StandardEntry();

            //base.EnterStateMachine("clubdance_solo", "Enter", "x");
            base.EnterStateMachine("misudanceonspot", "Enter", "x", "counter");
            //base.EnterStateMachine("danceOnCounterAndTable", "Enter", "x", "counter");
            int skillLevel = base.Actor.SkillManager.GetSkillLevel(SkillNames.ClubDancing);
            base.SetParameter("ClubDanceSkill", (DanceExpertise)Math.Max(0, skillLevel - 1));
            bool slotIsBackSide = false;
            bool paramValue = base.Target.IsIslandCounter(this.mEnterSlot, ref slotIsBackSide);

            base.SetParameter("IsIslandCounter", paramValue);
            base.SetParameter("IsIslandCounterBack", slotIsBackSide);
            base.Actor.AddInteraction(WatchSimDancingOnCounterOrTable.Singleton);
            base.BeginCommodityUpdates();

            base.AddSynchronousOneShotScriptEventHandler(0x65, new SacsEventHandler(this.SnapSimToTop));
            base.Actor.LookAtManager.SetInteractionLookAtThreshold(150);

            base.AnimateSim("Dance");
            //base.AnimateSim("Club_Dance");
            float duration = base.Target.ShowDurationMins / outfits.Length;
            bool succeeded = this.DoTimedLoop(duration);
            for (int i = 1; i < outfits.Length; i++)
            {
                base.Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToWork, outfits[i]);
                succeeded = this.DoTimedLoop(duration);
            }
            if (this.mReservedTile != null)
            {
                foreach (uint num3 in this.mReservedTile.GetFootprintNameHashes())
                {
                    this.mReservedTile.PushSimsFromFootprint(num3, base.Actor, null, true);
                }
            }
            base.AddSynchronousOneShotScriptEventHandler(0x66, new SacsEventHandler(this.SnapSimToBottom));
            base.AnimateSim("ExitNeutral");
            //base.AnimateSim("Exit");

            base.Actor.RemoveInteractionByType(WatchSimDancingOnCounterOrTable.Singleton);
            this.Watchable = false;
            base.EndCommodityUpdates(true);
            base.EndCommodityUpdates(succeeded);
            base.StandardExit();

            Lot.MetaAutonomyType venueType = base.Target.LotCurrent.GetMetaAutonomyType;

            ExoticDancer.SwitchToProperClothing(venueType, base.Actor,true);

            return true;
        }

        private void SnapSimToBottom(StateMachineClient sender, IEvent evt)
        {
            GameObject target = base.Target as GameObject;
            base.Actor.SnapToSlot(target, this.mEnterSlot);
        }

        private void SnapSimToTop(StateMachineClient sender, IEvent evt)
        {


            Slot danceSlot = mEnterSlot;

            GameObject target = base.Target as GameObject;
            base.Actor.SnapToSlot(target, danceSlot);
            Vector3 forwardOfSlot = Objects.GetForward(base.Target.ObjectId);
            base.Actor.SetForward(forwardOfSlot);
        }

        // Nested Types
        private sealed class Definition : InteractionDefinition<Sim, DancersStage, PerformShow>
        {

            protected override string GetInteractionName(Sim a, DancersStage target, InteractionObjectPair interaction)
            {
                return target.GetDanceOnObjectInteractionLocalizedString();
            }

            private static string greyedOutTooltipCallbackForObjectAbove()
            {
                return Localization.LocalizeString("Gameplay/Objects/DanceOnCounterOrTable:ObjectAbove", new object[0]);
            }

            private static string greyedOutTooltipCallbackForSpotObstructed()
            {
                return Localization.LocalizeString("Gameplay/Objects/DanceOnCounterOrTable:Blocked", new object[0]);
            }

            protected override bool Test(Sim sim, DancersStage target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.CurrentRole != null && target.CurrentRole.SimInRole == sim && !isAutonomous)
                {
                    if (!PerformShow.ObjectHasSpaceAboveIt(target as GameObject))
                    {
                        //greyedOutTooltipCallback = new GreyedOutTooltipCallback(PerformShow.Definition.greyedOutTooltipCallbackForObjectAbove);
                        return false;
                    }
                    if (PerformShow.DanceSpotObstructed(target))
                    {
                        //greyedOutTooltipCallback = new GreyedOutTooltipCallback(PerformShow.Definition.greyedOutTooltipCallbackForSpotObstructed);
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }
    }
}