using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Objects.Tables.Mimics;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Situations;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using Sims3.Gameplay.EventSystem;


namespace Sims3.Gameplay.Roles.Misukisu
{
    public class Dancer : Pianist
    {
        protected override void StartRole()
        {
            //if ((base.RoleGivingObject != null) && (base.RoleGivingObject.LotCurrent.GetMetaAutonomyType != this.mOutfitVenueType))
            //{
            //    base.InvalidateOutfit();
            //}
            try
            {

                DancingSkill skill = base.mSim.SkillManager.GetSkill<DancingSkill>(SkillNames.ClubDancing);
                if (skill == null)
                {
                    base.mSim.SkillManager.AddAutomaticSkill(SkillNames.ClubDancing);
                    skill = base.mSim.SkillManager.GetSkill<DancingSkill>(SkillNames.ClubDancing);
                }
                skill.SkillLevel = skill.MaxSkillLevel;
               
                base.StartRole();
                if (base.mSim.CreatedSim != null)
                {
                    base.mRoleGivingObject.AddRoleGivingInteraction(base.mSim.CreatedSim);
                }
            }
            catch (Exception e)
            {
                Sims3.Gameplay.Objects.Tables.Misukisu.DancerPodium.showMsg("Error: " + e.Message);
            }
        }

    }

}

namespace Sims3.Gameplay.Objects.Tables.Misukisu
{
    public class DancerPodium : TableBarPubRound, IRoleGiverExtended, IRoleGiver
    {
        private Role mCurrentRole;

        public void GetRoleTimes(out float startTime, out float endTime)
        {
            if (base.LotCurrent == null)
            {
                startTime = 0f;
                endTime = 0f;
            }
            else
            {
                Bartending.GetRoleTimes(out startTime, out endTime, base.LotCurrent.GetMetaAutonomyType);
            }

        }

        public void AddRoleGivingInteraction(Sim sim)
        {
        }

        public Role CurrentRole
        {
            get
            {
                showMsg("getting current role");
                return this.mCurrentRole;
            }
            set
            {
                showMsg("setting current role");
               
                this.mCurrentRole = value;
            }

        }

        public void PushRoleStartingInteraction(Sim sim)
        {
            showMsg("PushRoleStartingInteraction");
            if (sim != null)
            {
                try
                {
                    InteractionInstance instance = DanceOnPodium.Singleton.CreateInstance(this, sim, new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), true, false);
                    sim.InteractionQueue.AddAfterCheckingForDuplicates(instance);
                    
                }
                catch (Exception e)
                {
                    showMsg("Error: " + e.Message);
                }
            }

        }

        public void RemoveRoleGivingInteraction(Sim sim)
        {
        }

        public string RoleName(bool isFemale)
        {
            return "Dancer";
        }

        public Role.RoleType RoleType
        {
            get
            {
                return Role.RoleType.Pianist;
            }
        }

        public static void showMsg(string msg)
        {
            StyledNotification.Format format = new StyledNotification.Format(msg, StyledNotification.NotificationStyle.kSystemMessage);
            StyledNotification.Show(format);
        }

        public override void OnStartup()
        {
            base.AddInteraction(DanceOnPodium.Singleton);
        }


 
    }

    public class DanceOnPodium : Interaction<Sim, IDanceOnCounterOrTableObject>
    {
        // Fields
        private Slot mEnterSlot;
        private List<GameObject> mFadedOutObjects = new List<GameObject>();
        private GameObject mReservedTile;
        public static readonly InteractionDefinition Singleton = new Definition();
        public bool Watchable = true;

        // Methods
        public override void Cleanup()
        {
            foreach (GameObject obj2 in this.mFadedOutObjects)
            {
                obj2.FadeIn();
            }
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

        private void DanceLoop(StateMachineClient smc, InteractionInstance.LoopData ld)
        {
            int skillLevel = base.Actor.SkillManager.GetSkillLevel(SkillNames.ClubDancing);
            if (skillLevel > 1)
            {
                base.SetParameter("ClubDanceSkill", (Sims3.SimIFace.Enums.DanceExpertise)Math.Max(0, skillLevel - 1));
            }
            if (!base.Actor.BuffManager.HasElement(BuffNames.HasToPee))
            {
                base.Actor.AddExitReason(ExitReason.Finished);
            }
            TimePassedOnLotEvent e = new TimePassedOnLotEvent(EventTypeId.kSimDanced, base.Actor, base.Target, ld.mDeltaTime, base.Target.LotCurrent.LotId);
            EventTracker.SendEvent(e);
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
            foreach (GameObject obj2 in base.Target.GetContainedObjects())
            {
                if (Counter.Clean.TestFadeOut(obj2))
                {
                    obj2.FadeOutThisAndChildren(this.mFadedOutObjects);
                }
            }
            base.StandardEntry();
            base.EnterStateMachine("danceOnCounterAndTable", "Enter", "x", "counter");
            int skillLevel = base.Actor.SkillManager.GetSkillLevel(SkillNames.ClubDancing);
            base.SetParameter("ClubDanceSkill", (Sims3.SimIFace.Enums.DanceExpertise)Math.Max(0, skillLevel - 1));
            bool slotIsBackSide = false;
            bool paramValue = base.Target.IsIslandCounter(this.mEnterSlot, ref slotIsBackSide);
            base.SetParameter("IsIslandCounter", paramValue);
            base.SetParameter("IsIslandCounterBack", slotIsBackSide);
            base.Actor.AddInteraction(WatchSimDancingOnCounterOrTable.Singleton);
            base.BeginCommodityUpdates();
            base.AddSynchronousOneShotScriptEventHandler(0x65, new SacsEventHandler(this.SnapSimToTop));
            base.Actor.LookAtManager.SetInteractionLookAtThreshold(150);
            base.AnimateSim("Dance");
            bool succeeded = this.DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new InteractionInstance.InsideLoopFunction(this.DanceLoop), base.mCurrentStateMachine);
            if (this.mReservedTile != null)
            {
                foreach (uint num3 in this.mReservedTile.GetFootprintNameHashes())
                {
                    this.mReservedTile.PushSimsFromFootprint(num3, base.Actor, null, true);
                }
            }
            base.AddSynchronousOneShotScriptEventHandler(0x66, new SacsEventHandler(this.SnapSimToBottom));
            base.AnimateSim("GetDown");
            base.Actor.RemoveInteractionByType(WatchSimDancingOnCounterOrTable.Singleton);
            this.Watchable = false;
            base.EndCommodityUpdates(succeeded);
            base.StandardExit();
            return true;
        }

        private void SnapSimToBottom(StateMachineClient sender, IEvent evt)
        {
            GameObject target = base.Target as GameObject;
            base.Actor.SnapToSlot(target, this.mEnterSlot);
        }

        private void SnapSimToTop(StateMachineClient sender, IEvent evt)
        {
            Slot danceSlot = base.Target.GetDanceSlot();
            GameObject target = base.Target as GameObject;
            base.Actor.SnapToSlot(target, danceSlot);
            Vector3 forwardOfSlot = base.Target.GetForwardOfSlot(this.mEnterSlot);
            base.Actor.SetForward(-forwardOfSlot);
        }

        // Nested Types
        private sealed class Definition : InteractionDefinition<Sim, IDanceOnCounterOrTableObject, DanceOnCounterOrTable>
        {
           
            protected override string GetInteractionName(Sim a, IDanceOnCounterOrTableObject target, InteractionObjectPair interaction)
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

            protected override bool Test(Sim a, IDanceOnCounterOrTableObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {

                if (!DanceOnPodium.ObjectHasSpaceAboveIt(target as GameObject))
                {
                    greyedOutTooltipCallback = new GreyedOutTooltipCallback(DanceOnPodium.Definition.greyedOutTooltipCallbackForObjectAbove);
                    return false;
                }
                if (DanceOnPodium.DanceSpotObstructed(target))
                {
                    greyedOutTooltipCallback = new GreyedOutTooltipCallback(DanceOnPodium.Definition.greyedOutTooltipCallbackForSpotObstructed);
                    return false;
                }
                return true;
            }
        }
    }


}
