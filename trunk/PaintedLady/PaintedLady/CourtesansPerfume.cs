using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Misukisu.Common;
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
using Misukisu.Sims3.Gameplay.Interactions;

namespace Sims3.Gameplay.Objects.Misukisu
{
    public class CourtesansPerfume : GameObject, IRoleGiver, IRoleGiverExtended
    {
        public static string NAME = "Courtesan's Perfume";

        private Roles.Role mCurrentRole;
        private float mStartTime = 0F;
        private float mEndTime = 0F;



        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(TuneCourtesan.Singleton);
            //base.AddInteraction(GetInfo.Singleton);

        }

        public void TuningChanged(float startTime, float endTime)
        {

            mStartTime = startTime;
            mEndTime = endTime;
            ResetRole();
        }

        private void ResetRole()
        {
            EndRoleAndReplaceWithNew(CurrentRole);
        }

        public void GetRoleTimes(out float startTime, out float endTime)
        {
            startTime = mStartTime;
            endTime = mEndTime;

            //Message.Show("Someone is asking role times " + new StackTrace().ToString());
            if (startTime == 0)
            {
                if (base.LotCurrent != null)
                {
                    if (Bartending.TryGetHoursOfOperation(base.LotCurrent, ref startTime, ref endTime))
                    {
                        mStartTime = startTime;
                        mEndTime = endTime;
                        Message.Show("Setting relative role times from " + startTime + " to " + endTime);
                    }
                    else
                    {
                        Message.Show("Setting fixed role times");
                        startTime = 18F;
                        mStartTime = startTime;
                        endTime = 24F;
                        mEndTime = endTime;
                    }

                }
            }
            else
            {
                //Message.Show("Role time is from " + startTime + " to " + endTime);
            }

        }

        public void AddRoleGivingInteraction(Actors.Sim sim)
        {

            sim.AddInteraction(BuyWooHoo.Singleton);

            //try
            //{

            //    Message.Show("Adding role actions to " + (sim != null ? sim.FullName : "null"));
            //}
            //catch (Exception ex)
            //{
            //    Message.Show("Adding role actions to null " + ex.Message + " - " + new StackTrace().ToString());
            //}
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
                    //Message.Show("Null role was set " + new StackTrace().ToString());
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
                            Message.ShowError(CourtesansPerfume.NAME, "Cannot create custom role, clone failed", true, null);

                        }


                    }
                    else
                    {
                        Message.ShowError(CourtesansPerfume.NAME, "Cannot create custom role, Pianist sim not instantiated", true, null);
                    }

                }
                catch (Exception ex)
                {
                    Message.ShowError(CourtesansPerfume.NAME, "Cannot create custom role", true, ex);
                    this.mCurrentRole = value;
                }
            }
        }

        public void PushRoleStartingInteraction(Actors.Sim sim)
        {
            try
            {
                //Message.Show("PushRoleStartingInteraction to " + (sim != null ? sim.FullName : "null"));
            }
            catch (Exception ex)
            {
                Message.ShowError(CourtesansPerfume.NAME, "Sim cannot play the role", false, ex);
            }
        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {
            sim.RemoveInteractionByType(BuyWooHoo.Singleton.GetType());
            
        }

        public string RoleName(bool isFemale)
        {
            return "Courtesan";
        }

        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Pianist; }
        }
    }
}
