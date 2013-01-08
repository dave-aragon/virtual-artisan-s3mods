using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;

using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using System.Diagnostics;
using Misukisu.Sims3.Gameplay.Interactions.Paintedlady;
using Sims3.Gameplay.Objects.Misukisu;
using Misukisu.Paintedlady;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.Gameplay.Utilities;

namespace Sims3.Gameplay.Roles.Misukisu
{
    public class Courtesan : Bouncer
    {

        public Courtesan()
            : base()
        { }

        public override string CareerTitleKey
        {
            get
            {
                return Texts.CAREERTITLE;
            }
        }

        public Courtesan(RoleData data, SimDescription s, IRoleGiver roleGiver)
            : base(data, s, roleGiver)
        { }

        public Courtesan(SimDescription sim, IRoleGiverExtended roleGiver)
        {
            this.mWorld = GameUtils.GetCurrentWorld();
            this.mType = RoleType.Bouncer;
            this.mSim = sim;
            this.mRoleGivingObject = roleGiver;
            this.mSim.AssignedRole = this;

            float hourOfDay;
            float hourOfDay2;
            roleGiver.GetRoleTimes(out hourOfDay, out hourOfDay2);
            SetAlarms(hourOfDay, hourOfDay2);
            ValidateAndSetupOutfit();
        }

        public void SetAlarms(float startTime, float endTime)
        {

            int num = this.Data.StartTime.Length;
            for (int i = 0; i < num; i++)
            {
                if (startTime != endTime)
                {
                    Lazy.Allocate<List<AlarmHandle>>(ref this.mAlarmHandles);
                    AlarmHandle alarmHandle2 = AlarmManager.Global.AddAlarmDay(startTime, DaysOfTheWeek.All,
                        new AlarmTimerCallback(this.StartRoleAlarmHandler), "Start NPC Role alarm index: " + i, AlarmType.AlwaysPersisted, this);
                    AlarmManager.Global.AlarmWillYield(alarmHandle2);
                    this.mAlarmHandles.Add(alarmHandle2);
                    this.mAlarmHandles.Add(AlarmManager.Global.AddAlarmDay(endTime, DaysOfTheWeek.All,
                        new AlarmTimerCallback(this.EndRole), "End NPC Role alarm index: " + i, AlarmType.AlwaysPersisted, this));
                }
            }
        }

        public override bool AllowToGoToOtherLots()
        {
            return true;
        }

        public new void RemoveSimFromRole()
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "Sim " + mSim.FullName + " will be removed from role");
            }
            base.RemoveSimFromRole();
        }

        public static bool IsTalkingTo(Sim actor, Sim target, bool result)
        {
            SocialComponent social = target.SocialComponent;
            if (social != null)
            {
                Conversation conversation = social.Conversation;
                if (conversation != null && conversation.ContainsSim(actor))
                {
                    result = true;
                }
            }
            return result;
        }

        public static Courtesan AssignedRole(Sim sim)
        {
            Role role = sim.SimDescription.AssignedRole;
            Courtesan courtesan = role as Courtesan;
            return courtesan;
        }

        public static Courtesan clone(Role toClone, SimDescription actor)
        {
            Courtesan newRole = new Courtesan(toClone.Data, actor, toClone.RoleGivingObject);
            newRole.StartRole();

            return newRole;
        }


        public override void SimulateRole(float minPassed)
        {
            if (base.IsActive && isRoleEnabledOnThisLot())
            {
                Sim createdSim = this.mSim.CreatedSim;
                if (createdSim != null && base.RoleGivingObject != null && createdSim.LotCurrent == base.RoleGivingObject.LotCurrent)
                {
                    base.RoleGivingObject.PushRoleStartingInteraction(createdSim);
                }
            }
        }

        public bool isRoleEnabledOnThisLot()
        {
            Sim createdSim = this.mSim.CreatedSim;
            if (createdSim != null && base.RoleGivingObject != null)
            {
                Lot lot = RoleGivingObject.LotCurrent;
                if ((Household.ActiveHousehold.LotHome == lot) || (lot.IsCommunityLot))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsStoryProgressionProtected
        {
            get
            {
                return true;
            }
        }

        public override void EndRole()
        {
            if (isRoleEnabledOnThisLot())
            {
                bool isActive = base.IsActive;
                base.RoleGivingObject.RemoveRoleGivingInteraction(base.mSim.CreatedSim);
                UnprotectSimFromStoryProgression();

                Sim createdSim = base.mSim.CreatedSim;
                if (isActive && (createdSim != null))
                {
                    mIsActive = false;
                    // CreatedSim.Motives.RemoveMotive(kind);
                    // createdSim.Motives.RestoreDecays();
                    createdSim.InteractionQueue.CancelAllInteractions();
                    //this.mSim.CreatedSim.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday);
                    createdSim.RemoveInteractionByType(GoToALot.Singleton);
                    Sim.MakeSimGoHome(createdSim, false);
                    if (Message.Sender.IsDebugging())
                    {
                        Message.Sender.Debug(this, "Sim " + mSim.FullName + " was sent home");
                    }
                }
            }

        }


        public override void SwitchIntoOutfit()
        {
            try
            {
                Lot roleLot = (base.RoleGivingObject as CourtesansPerfume).GetTargetLot();
                if (roleLot != null)
                {
                    Sim sim = this.mSim.CreatedSim;
                    this.mSim.CreatedSim.InteractionQueue.CancelAllInteractions();

                    Lot.MetaAutonomyType venueType = roleLot.GetMetaAutonomyType;
                    SwitchToProperClothing(sim, venueType);
                    if (Message.Sender.IsDebugging())
                    {
                        Message.Sender.Debug(this, "Role clothing change request made "
                            + mSim.FullName + " " + venueType.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(this, "Cannot change courtesan's clothes: ", false, e);
            }
        }

        public static void SwitchToProperClothing(Sim sim, Lot.MetaAutonomyType venueType)
        {
            SimIFace.CAS.OutfitCategories outfitType = SimIFace.CAS.OutfitCategories.Everyday;
            if (venueType == Lot.MetaAutonomyType.CocktailLoungeAsian || venueType == Lot.MetaAutonomyType.CocktailLoungeCelebrity || venueType == Lot.MetaAutonomyType.CocktailLoungeVampire)
            {
                outfitType = SimIFace.CAS.OutfitCategories.Formalwear;
            }

            sim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.GoingToWork, outfitType);

        }

        public static void forceSimToLot(Sim theSim)
        {
            InteractionInstance instance = GoToALot.Singleton.CreateInstance(
                              theSim, theSim, new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior), false, false);
            theSim.InteractionQueue.AddAfterCheckingForDuplicates(instance);

        }

        private Lot getRoleLot()
        {
            Lot roleLot = null;
            if (mRoleGivingObject != null)
            {
                roleLot = mRoleGivingObject.LotCurrent;
            }
            return roleLot;
        }

        public override void StartRole()
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, mSim.FullName + " role starting...");
            }
            try
            {
                if (isRoleEnabledOnThisLot())
                {
                    if (!this.mIsActive && this.mSim.IsValidDescription)
                    {
                        InstantiateSim();
                        this.SwitchIntoOutfit();
                        Sim sim = this.mSim.CreatedSim;
                        if (sim != null)
                        {
                            AddNeededMotives();
                            CourtesansPerfume perfume = GetPerfume();
                            if (perfume != null)
                            {
                                this.mIsActive = true;
                                sim.RemoveInteractionByType(GoToALot.Singleton);
                                sim.AddInteraction(GoToALot.Singleton);
                                MakeSimComeToRoleLot();
                                ProtectSimFromStoryProgression();

                                perfume.AddRoleGivingInteraction(sim);
                                perfume.PushRoleStartingInteraction(sim);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(this, "Cannot start the role", false, e);
            }
        }

        private void AddNeededMotives()
        {
            // Nothing needed at the moment
        }

        public void MakeSimComeToRoleLot()
        {
            if (mRoleGivingObject != null)
            {
                if (this.mSim.CreatedSim.LotCurrent == null || this.mSim.CreatedSim.LotCurrent != mRoleGivingObject.LotCurrent)
                {
                    forceSimToLot(this.mSim.CreatedSim);
                }
            }

            Lot roleLot = (base.RoleGivingObject as CourtesansPerfume).GetTargetLot();

            if (roleLot != null)
            {
                this.mSim.CreatedSim.InteractionQueue.Add(GoToALot.Singleton.CreateInstance(this.mSim.CreatedSim, this.mSim.CreatedSim,
                    new InteractionPriority(InteractionPriorityLevel.High), true, true));
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Sim called from "
                        + this.mSim.CreatedSim.LotCurrent.Name + " to " + roleLot.Name);
                }
            }
        }

        public void InstantiateSim()
        {
            if (this.mSim.CreatedSim == null)
            {
                Lot lot = LotManager.SelectRandomLotForNPCMoveIn(null);
                this.mSim.CreatedSim = this.mSim.Instantiate(lot);
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Sim instantiated: " + mSim.FullName);
                }
            }

        }

        public CourtesansPerfume GetPerfume()
        {
            return (RoleGivingObject as CourtesansPerfume);
        }

        public static CourtesansPerfume GetPerfume(Sim sim)
        {
            CourtesansPerfume perfume = null;
            Courtesan role = Courtesan.AssignedRole(sim);
            if (role != null)
            {
                perfume = role.GetPerfume();
            }
            return perfume;
        }
    }

}