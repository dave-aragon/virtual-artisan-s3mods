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
using Sims3.UI.Controller;
using Sims3.Gameplay.Socializing;

namespace Sims3.Gameplay.Objects.Misukisu
{
    public class CourtesansPerfume : GameObject, IRoleGiver, IRoleGiverExtended
    {
        public static string NAME = "Courtesan's Perfume";

        private Roles.Role mCurrentRole;
        private float mStartTime = 0F;
        private float mEndTime = 0F;
        private int mPayPerWoohoo = 100;
        private int mPayPerDay = 500;

        private Dictionary<SimDescription, LongTermRelationshipTypes> mRelationsToRestore =
            new Dictionary<SimDescription, LongTermRelationshipTypes>();

        private Sim mSlaveOwner;

        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(TuneCourtesan.Singleton);
            base.AddInteraction(ToggleDebugger.Singleton);

        }

        public void storeOldRelationship(SimDescription target, LongTermRelationshipTypes relation)
        {
            mRelationsToRestore.Add(target, relation);
        }

        public void restoreOldRelationship(SimDescription buyer, Sim seller)
        {
            if (mRelationsToRestore.ContainsKey(buyer))
            {
                LongTermRelationshipTypes relationshipBefore = mRelationsToRestore[buyer];
                Relationship relationToTarget = seller.GetRelationship(buyer.CreatedSim, true);
                relationToTarget.LTR.ForceChangeState(relationshipBefore);
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Relationship to " + buyer.FullName + " restored to " + relationshipBefore.ToString());
                }
            }
        }

        public override void AddBuildBuyInteractions(List<InteractionDefinition> buildBuyInteractions)
        {
            buildBuyInteractions.Add(TuneCourtesan.Singleton);
            buildBuyInteractions.Add(ToggleDebugger.Singleton);
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
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "Role tuning changed - startTime="
                    + startTime + " endTime=" + endTime);
            }
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
                        startTime = 18F;
                        mStartTime = startTime;
                        endTime = 24F;
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
            sim.AddInteraction(BuyWooHoo.Singleton);
            sim.AddInteraction(TakeMistress.Singleton);
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
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Courtesan/Courter now owned by: " + mSlaveOwner.FullName);
                }
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
                    Courtesan aRole = Courtesan.clone(value, currentActor);
                    this.mCurrentRole = aRole;
                    RoleManager.sRoleManager.AddRole(aRole);
                    if (Message.Sender.IsDebugging())
                    {
                        Message.Sender.Debug(this, "Role cloning succeeded: " + currentActor.FullName);
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
            // Nothing needed, works on her own
        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {
            sim.RemoveInteractionByType(BuyWooHoo.Singleton.GetType());
            sim.RemoveInteractionByType(TakeMistress.Singleton.GetType());

        }

        public string RoleName(bool isFemale)
        {
            return "";
        }

        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Pianist; }
        }

        public int PayPerDay
        {
            get { return mPayPerDay; }
        }

        public int PayPerWoohoo
        {
            get { return mPayPerWoohoo; }
        }

        public ResourceKey GetRoleUniformKey(Sim Sim, Lot.MetaAutonomyType venueType)
        {
            return ResourceKey.kInvalidResourceKey;
        }
    }
}
