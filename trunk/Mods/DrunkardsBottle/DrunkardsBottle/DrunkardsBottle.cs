using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Misukisu.Drunkard;
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
using Misukisu.Sims3.Gameplay.Interactions.Drunkard;

namespace Sims3.Gameplay.Objects.Misukisu
{

    public class DrunkardsBottle : GameObject, IRoleGiver, IRoleGiverExtended
    {
        private Roles.Role mCurrentRole;
        private float mStartTime = 0F;
        private float mEndTime = 0F;
        private string mRoleTitle = "regular";//Localization.LocalizeString(Texts.CAREERTITLE, new string[0]);


        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(TuneDrunkard.Singleton);
            base.AddInteraction(ToggleDebugger.Singleton);

        }

        public override void AddBuildBuyInteractions(List<InteractionDefinition> buildBuyInteractions)
        {
            buildBuyInteractions.Add(TuneDrunkard.Singleton);
            buildBuyInteractions.Add(ToggleDebugger.Singleton);
            base.AddBuildBuyInteractions(buildBuyInteractions);
        }

        public Lot GetTargetLot()
        {
            return LotCurrent;
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

                        startTime += 1;
                        mStartTime = startTime;
                        endTime -= 1;
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
                Drunkard newRole = value as Drunkard;
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
                    Drunkard aRole = Drunkard.clone(value, currentActor);

                    this.mCurrentRole = aRole;
                    RoleManager.sRoleManager.AddRole(aRole);
                    if (Message.Sender.IsDebugging())
                    {
                        Message.Sender.Debug(this, "Role cloning succeeded: " + currentActor.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Message.Sender.ShowError(this, "Cannot create custom role", true, ex);
                    this.mCurrentRole = value;
                }
            }
        }

        public void PushRoleStartingInteraction(Actors.Sim sim)
        {
            try
            {

                //Message.Sender.Show("PushRoleStartingInteraction to " + (sim != null ? sim.FullName : "null"));
                if (sim != null && GetTargetLot().IsCommunityLot)
                {
                    IBarProfessional bar = findNearestBar(sim);
                    if (bar != null && bar.InUse)
                    {
                        Bartending.DrinkDescription bestDrink = Bartending.GetBestDrinkFor(sim, base.LotCurrent.GetMetaAutonomyType);
                        String bestDrinkName = null;
                        if (bestDrink != null)
                        {
                            bestDrinkName = bestDrink.GetLocalizedName();
                        }

                        if (bestDrinkName != null)
                        {
                            PushSimToDrink(sim, bar, bestDrinkName);
                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message.Sender.ShowError(this, "Cannot order drink from bar ", false, ex);
            }
        }

        private void PushSimToDrink(Actors.Sim sim, IBarProfessional bar, String bestDrinkName)
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "Best drink for " + sim.FullName + " is: " + bestDrinkName);
            }
            List<InteractionObjectPair> interactions = bar.GetAllInteractionsForActor(sim);
            InteractionDefinition drinkingDefinition = null;
            foreach (InteractionObjectPair interaction in interactions)
            {
                InteractionDefinition interactionDef = interaction.InteractionDefinition;
                string name = interactionDef.GetInteractionName(sim, bar, interaction);

                if (name != null && name.Contains(bestDrinkName))
                {
                    drinkingDefinition = interactionDef;
                    break;
                }
            }

            if (drinkingDefinition != null)
            {

                IEnumerable<InteractionInstance> actions = sim.InteractionQueue.InteractionList;
                List<InteractionInstance> toCancel = new List<InteractionInstance>();
                bool alreadyDrinking = false;
                foreach (InteractionInstance action in actions)
                {
                    if (!(action is BarProfessional.BarInteraction))
                    {
                        if (Message.Sender.IsDebugging())
                        {
                            Message.Sender.Debug(this, "Canceling " + action.GetInteractionName());
                        }
                        toCancel.Add(action);
                    }
                    else
                    {
                        if (Message.Sender.IsDebugging())
                        {
                            Message.Sender.Debug(this, "Sim is already drinking, no push added");
                        }
                        alreadyDrinking = true;
                        break;
                    }

                }

                foreach (InteractionInstance action in toCancel)
                {
                    sim.InteractionQueue.CancelInteraction(action, false);
                }
                if (!alreadyDrinking)
                {
                    sim.InteractionQueue.AddAfterCheckingForDuplicates(drinkingDefinition.CreateInstance(bar, sim, new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), true, false));
                    if (Message.Sender.IsDebugging())
                    {
                        Message.Sender.Debug(this, "Sim was pushed to drink: " + sim.FullName);
                    }
                }
            }
        }

        private IBarProfessional findNearestBar(Actors.Sim sim)
        {
            return GlobalFunctions.GetClosestObject<BarProfessional>(sim, false, true, new List<BarProfessional>(), null);

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
