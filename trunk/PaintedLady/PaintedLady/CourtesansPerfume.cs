using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Misukisu.Sims3.Gameplay.Interactions.Paintedlady;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Roles.Misukisu;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Gameplay.Objects.Counters;
using Misukisu.Common;

namespace Sims3.Gameplay.Objects.Misukisu
{
    public class CourtesansPerfume : GameObject, IRoleGiver, IRoleGiverExtended
    {
        public static string NAME = "Courtesan's Perfume";

        private Roles.Role mCurrentRole;
        private float mStartTime = 0F;
        private float mEndTime = 0F;
        private Sim mSlaveOwner;

        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(TuneCourtesan.Singleton);
            base.AddInteraction(TakeMistress.Singleton);

        }

        public override void AddBuildBuyInteractions(List<InteractionDefinition> buildBuyInteractions)
        {
            buildBuyInteractions.Add(TuneCourtesan.Singleton);
            base.AddBuildBuyInteractions(buildBuyInteractions);
        }

        public Lot GetTargetLot()
        {
            if (SlaveOwner != null)
            {
                return SlaveOwner.LotHome;
            }
            else
            {
                return LotCurrent;
            }
        }

        public void TuningChanged(float startTime, float endTime)
        {

            mStartTime = startTime;
            mEndTime = endTime;
            ResetRole();
        }

        private void ResetRole()
        {
            if (CurrentRole != null)
            {
                EndRoleAndReplaceWithNew(CurrentRole);
            }
        }

        public void GetRoleTimes(out float startTime, out float endTime)
        {
            startTime = mStartTime;
            endTime = mEndTime;

            //Message.Sender.Show("Someone is asking role times " + new StackTrace().ToString());
            if (startTime == 0)
            {
                if (base.LotCurrent != null)
                {
                    if (Bartending.TryGetHoursOfOperation(base.LotCurrent, ref startTime, ref endTime))
                    {
                        mStartTime = startTime;
                        mEndTime = endTime;
                        //Message.Sender.Show("Setting relative role times from " + startTime + " to " + endTime);
                    }
                    else
                    {
                        //Message.Sender.Show("Setting fixed role times");
                        startTime = 18F;
                        mStartTime = startTime;
                        endTime = 24F;
                        mEndTime = endTime;
                    }

                }
            }
            else
            {
                //Message.Sender.Show("Role time is from " + startTime + " to " + endTime);
            }

        }

        public void AddRoleGivingInteraction(Actors.Sim sim)
        {
            sim.AddInteraction(BuyWooHoo.Singleton);
        }

        public Sim SlaveOwner
        {
            get
            {
                return this.mSlaveOwner;
            }
            set
            {
                this.mSlaveOwner = value;
            }
        }

        public Roles.Role CurrentRole
        {
            get
            {
                return this.mCurrentRole;
            }
            set
            {
                Courtesan newRole = value as Courtesan;
                if (newRole != null)
                {
                    this.mCurrentRole = newRole;
                }
                else if (value != null)
                {
                    EndRoleAndReplaceWithNew(value);
                }
                else
                {
                    //Message.Sender.Show("Null role was set " + new StackTrace().ToString());
                    this.mCurrentRole = value;
                }

            }

        }

        private void EndRoleAndReplaceWithNew(Role value)
        {
            if (value != null)
            {
                try
                {
                    Sim currentActor = value.SimInRole;
                    if (currentActor != null)
                    {
                        value.RemoveSimFromRole();
                        Courtesan aRole = Courtesan.clone(value, currentActor);

                        if (aRole != null)
                        {
                            this.mCurrentRole = aRole;
                            RoleManager.sRoleManager.AddRole(aRole);
                        }
                        else
                        {
                            Message.Sender.ShowError(CourtesansPerfume.NAME, "Cannot create custom role, clone failed", true, null);
                        }
                    }
                    else
                    {
                        Message.Sender.ShowError(CourtesansPerfume.NAME, "Cannot create custom role, Pianist sim not instantiated", true, null);
                    }

                }
                catch (Exception ex)
                {
                    Message.Sender.ShowError(CourtesansPerfume.NAME, "Cannot create custom role", true, ex);
                    this.mCurrentRole = value;
                }
            }
        }

        public void PushRoleStartingInteraction(Actors.Sim sim)
        {
            //try
            //{
            //    //Message.Sender.Show("PushRoleStartingInteraction to " + (sim != null ? sim.FullName : "null"));
            //}
            //catch (Exception ex)
            //{
            //    Message.Sender.ShowError(CourtesansPerfume.NAME, "Sim cannot play the role", false, ex);
            //}
        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {
            sim.RemoveInteractionByType(BuyWooHoo.Singleton.GetType());

        }

        public string RoleName(bool isFemale)
        {
            return "";
        }

        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Pianist; }
        }

        public ResourceKey GetRoleUniformKey(Sim Sim, Lot.MetaAutonomyType venueType)
        {
            return ResourceKey.kInvalidResourceKey;
        }
    }
}
