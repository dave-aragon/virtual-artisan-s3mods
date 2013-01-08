using System;
using System.Collections.Generic;
using System.Text;
using Sims3.SimIFace;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.Counters;
using Sims3.UI;
using Sims3.Gameplay.Objects.Misukisu;
using Sims3.Gameplay.Skills;

namespace Misukisu.HomeBarBusiness
{

    class HomeBartender : Sims3.Gameplay.Skills.Bartending.Bartender
    {
        public static Debugger debugger = new Debugger("role");

        [Persistable]
        private int payPerDay = 20;
        public static int startTime = 19;
        public static int endTime = 2;

        public HomeBartender()
            : base()
        {
        }

        public HomeBartender(SimDescription sim, EmployeeCertificate bar)
        {
            this.mWorld = GameUtils.GetCurrentWorld();
            this.mType = RoleType.Bartender;
            this.mSim = sim;
            this.mRoleGivingObject = bar;
            this.mSim.AssignedRole = this;

           // float hourOfDay;
          //  float hourOfDay2;
           // roleGiverExtended.GetRoleTimes(out hourOfDay, out hourOfDay2);
            SetAlarms(19, 2);
            ValidateAndSetupOutfit();
            debugger.Debug(this, "role created for " + sim.FullName);
        }

        public override bool AllowToGoToOtherLots()
        {
            return true;
        }

        public override void StartRole()
        {
            if (!this.mIsActive && this.mSim.IsValidDescription)
            {
                debugger.Debug(this, "description is valid");
                this.ValidateAndSetupOutfit();
                if (this.mSim.CreatedSim == null)
                {
                    Lot lot = LotManager.SelectRandomLotForNPCMoveIn(null);
                    this.mSim.Instantiate(lot);

                }
                this.SwitchIntoOutfit();

                Sim theSim = this.mSim.CreatedSim;
                if (theSim != null)
                {
                    Bartending skill = theSim.SkillManager.GetSkill<Bartending>(Sims3.Gameplay.Skills.SkillNames.Bartending);
                    if (skill == null)
                    {
                        theSim.SkillManager.AddAutomaticSkill(SkillNames.Bartending);

                    }

                    theSim.RemoveInteractionByType(GoToALot.Singleton);
                    theSim.AddInteraction(GoToALot.Singleton);

                    foreach (CommodityKind current in this.Data.Motives)
                    {
                        this.mSim.CreatedSim.Motives.CreateMotive(current);
                    }
                    foreach (CommodityKind current2 in this.Data.MotivesToFreeze)
                    {
                        this.mSim.CreatedSim.Motives.SetMax(current2);
                        this.mSim.CreatedSim.Motives.SetDecay(current2, false);
                    }
                    this.mIsActive = true;
                    debugger.Debug(this, "Role is active");
                    if (mRoleGivingObject != null)
                    {
                        if (this.mSim.CreatedSim.LotCurrent == null || this.mSim.CreatedSim.LotCurrent != mRoleGivingObject.LotCurrent)
                        {
                            forceSimToLot(theSim);
                        }
                    }
                    if (this.IsStoryProgressionProtected)
                    {
                        this.ProtectSimFromStoryProgression();
                    }
                    if (this.RoleGivingObject != null)
                    {
                        RoleGivingObject.PushRoleStartingInteraction(this.mSim.CreatedSim);
                        debugger.Debug(this, "Role action pushed" + this.mSim.CreatedSim.InteractionQueue.ToString());
                    }
                }
            }
        }

        public static void forceSimToLot(Sim theSim)
        {
            InteractionInstance instance = GoToALot.Singleton.CreateInstance(
                              theSim, theSim, new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior), false, false);
            theSim.InteractionQueue.AddAfterCheckingForDuplicates(instance);
            debugger.Debug(theSim, "sim called");
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

        public override void EndRole()
        {
            bool isActive = base.IsActive;
            this.mRoleGivingObject.RemoveRoleGivingInteraction(this.mSim.CreatedSim);
            base.EndRole();
            Sim theSim = this.mSim.CreatedSim;
            if (isActive && theSim != null)
            {
                makeEmployeePay(theSim);
                theSim.RemoveInteractionByType(GoToALot.Singleton);
                Sim.MakeSimGoHome(theSim, false);
            }
        }

        private void makeEmployeePay(Sim theSim)
        {
            theSim.ModifyFunds(payPerDay);
            Lot roleLot = getRoleLot();
            Household houseHold = Household.ActiveHousehold;
            if (roleLot == houseHold.LotHome)
            {
                houseHold.ModifyFamilyFunds(-payPerDay);
            }

            StyledNotification.Format format = new StyledNotification.Format("I earned §" + payPerDay + " today", theSim.ObjectId, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSimTalking);
            StyledNotification.Show(format);
        }

        public override void SimulateRole(float minPassed)
        {
            debugger.Debug(this, "simulate role");
            if (base.IsActive)
            {
                Sim createdSim = this.mSim.CreatedSim;
                if (createdSim != null && base.RoleGivingObject != null)
                {
                    RoleGivingObject.PushRoleStartingInteraction(createdSim);
                    debugger.Debug(this, "pushed for action");
                }
            }
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
    }
}
