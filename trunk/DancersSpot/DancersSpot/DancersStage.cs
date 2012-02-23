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
using Sims3.Gameplay.Objects.Tables.Mimics;
using Sims3.SimIFace.CAS;

namespace Sims3.Gameplay.Objects.Misukisu
{
    public class DancersStage : TableBarPubRound, IRoleGiverExtended, IRoleGiver
    {
        public static string NAME = "Dancer's Stage";

        private Roles.Role mCurrentRole;
        private float mStartTime = 0F;
        private float mEndTime = 0F;
        private OutfitCategories[] mShowOutfits = new OutfitCategories[] { OutfitCategories.Career, OutfitCategories.Sleepwear,OutfitCategories.Naked };
        //private float[] mShowtimes;


        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(TuneExoticDancer.Singleton);
            base.AddInteraction(PerformShow.Singleton);
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
                        //Message.Show("Setting relative role times from " + startTime + " to " + endTime);
                    }
                    else
                    {
                        //Message.Show("Setting fixed role times");
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

            Message.Show("Role started");
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
                ExoticDancer newRole = value as ExoticDancer;
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
                        ExoticDancer aRole = ExoticDancer.clone(value, currentActor);

                        if (aRole != null)
                        {
                            this.mCurrentRole = aRole;
                            RoleManager.sRoleManager.AddRole(aRole);
                        }
                        else
                        {
                            Message.ShowError(DancersStage.NAME, "Cannot create custom role, clone failed", true, null);

                        }


                    }
                    else
                    {
                        Message.ShowError(DancersStage.NAME, "Cannot create custom role, Pianist sim not instantiated", true, null);
                    }

                }
                catch (Exception ex)
                {
                    Message.ShowError(DancersStage.NAME, "Cannot create custom role", true, ex);
                    this.mCurrentRole = value;
                }
            }
        }

        public void PushRoleStartingInteraction(Actors.Sim sim)
        {
            try
            {
                if (sim != null)
                {
                    ExoticDancer currentRole = this.CurrentRole as ExoticDancer;
                    if (currentRole != null)
                    {
                        currentRole.FreezeMotivesWhilePlaying();
                    }

                    //Message.Show("Pushing sim to perform");

                    InteractionInstance instance = PerformShow.Singleton.CreateInstance(this, sim,
                        new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), false, false);
                    sim.InteractionQueue.AddAfterCheckingForDuplicates(instance);

                   
                }
            }
            catch (Exception ex)
            {
                Message.ShowError(DancersStage.NAME, "Sim cannot play the role", false, ex);
            }
        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {

        }

        public string RoleName(bool isFemale)
        {
            return "Exotic Dancer";
        }

        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Pianist; }
        }

        public OutfitCategories[] ShowOutfits
        {
            get { return mShowOutfits; }
        }
    }
}
