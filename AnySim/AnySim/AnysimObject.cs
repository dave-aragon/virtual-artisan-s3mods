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
using Misukisu.Sims3.Gameplay.Interactions.Anysim;
using Misukisu.Anysim;

namespace Sims3.Gameplay.Objects.Misukisu
{

    public class AnysimObject : GameObject, IRoleGiver, IRoleGiverExtended
    {
        private Roles.Role mCurrentRole;
        private float mStartTime = 0F;
        private float mEndTime = 0F;
        private string mRoleTitle = Localization.LocalizeString(Texts.CAREERTITLE, new string[0]);


        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(TuneAnysim.Singleton);
            base.AddInteraction(ToggleDebugger.Singleton);
        }

        public override void AddBuildBuyInteractions(List<InteractionDefinition> buildBuyInteractions)
        {
            buildBuyInteractions.Add(TuneAnysim.Singleton);
            buildBuyInteractions.Add(ToggleDebugger.Singleton);
            base.AddBuildBuyInteractions(buildBuyInteractions);
        }

        public Lot GetTargetLot()
        {
            return LotCurrent;
        }

        public void TuningChanged(float startTime, float endTime, string roleTitle)
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "Role tuning changed - startTime="
                    + startTime + " endTime=" + endTime + " roleTitle=" + roleTitle);
            }
            mStartTime = startTime;
            mEndTime = endTime;
            mRoleTitle = roleTitle;
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

            if (startTime == 0)
            {
                if (base.LotCurrent != null)
                {
                    if (Bartending.TryGetHoursOfOperation(base.LotCurrent, ref startTime, ref endTime))
                    {
                        mStartTime = startTime;
                        mEndTime = endTime;
                        if (Message.Sender.IsDebugging())
                        {
                            Message.Sender.Debug(this, "Role times set automatically - startTime="
                                + startTime + " endTime=" + endTime);
                        }
                    }
                    else
                    {
                        startTime = 12;
                        mStartTime = startTime;
                        endTime = 11.5F;
                        mEndTime = endTime;
                        if (Message.Sender.IsDebugging())
                        {
                            Message.Sender.Debug(this, "Role times set fixed - startTime="
                                + startTime + " endTime=" + endTime);
                        }
                    }

                }
            }

        }

        public void AddRoleGivingInteraction(Actors.Sim sim)
        {

        }

        public Roles.Role CurrentRole
        {
            get
            {
                return this.mCurrentRole;
            }
            set
            {
                Anysim newRole = value as Anysim;
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
                    SimDescription currentActor = value.mSim;

                    value.RemoveSimFromRole();
                    Anysim aRole = Anysim.clone(value, currentActor);

                    this.mCurrentRole = aRole;
                    RoleManager.sRoleManager.AddRole(aRole);
                    if (Message.Sender.IsDebugging())
                    {
                        Message.Sender.Debug(this, "Role cloning succeeded: " + currentActor.FullName);
                    }
                    
                }
                catch (Exception ex)
                {
                    Message.Sender.ShowError(Texts.NAME, "Cannot create custom role", true, ex);
                    this.mCurrentRole = value;
                }
            }
        }

        public void PushRoleStartingInteraction(Actors.Sim sim)
        {

        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {

        }

        public string RoleName(bool isFemale)
        {
            return mRoleTitle;
        }

        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Bartender; }
        }


        public ResourceKey GetRoleUniformKey(Sim Sim, Lot.MetaAutonomyType venueType)
        {
            return ResourceKey.kInvalidResourceKey;
        }
    }
}
