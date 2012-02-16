using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Misukisu;
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

namespace Sims3.Gameplay.Objects.Misukisu
{

    public class DrunkardsBottle : GameObject, IRoleGiver, IRoleGiverExtended
    {
        public static sealed string NAME="Drunkard's Bottle";
        public enum Owner { Hangaround, CivilizedDrinker, Drunkard };
        private Roles.Role mCurrentRole;
        private float mStartTime = 0F;
        private float mEndTime = 0F;
        private Owner mOwnerType = Owner.Drunkard;
        
        
        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(ShowTuningDialog.Singleton);
            base.AddInteraction(GetInfo.Singleton);
           
        }

        public void TuningChanged(Owner ownerType,float startTime, float endTime)
        {
            mOwnerType = ownerType;
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

                        startTime += 1;
                        mStartTime = startTime;
                        endTime -= 1;
                        mEndTime = endTime;
                        Message.Show("Setting relative role times from " + startTime + " to " + endTime);
                    }
                    else
                    {
                        Message.Show("Setting fixed role times");
                        startTime = 12;
                        mStartTime = startTime;
                        endTime = 11.5F;
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
           


            //try
            //{

            //    Message.Show("Adding role actions to " + (sim != null ? sim.FullName : "null"));
            //}
            //catch (Exception ex)
            //{
            //    Message.Show("Adding role actions to null " + ex.Message + " - " + new StackTrace().ToString());
            //}
        }

        public Owner OwnerType {
            get { return mOwnerType; }
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
                    //Message.Show("Custom role set! who the hack did this: " + new StackTrace().ToString());
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
                        Drunkard aRole = Drunkard.clone(value, currentActor);

                        if (aRole != null)
                        {
                            this.mCurrentRole = aRole;
                            RoleManager.sRoleManager.AddRole(aRole);
                            //Message.Show("Cloned pianist and swapped it to custom role");
                        }
                        else
                        {
                            Message.ShowError(DrunkardsBottle.NAME, "Cannot create custom role, clone failed", true, null);
                           
                        }


                    }
                    else
                    {
                        Message.ShowError(DrunkardsBottle.NAME, "Cannot create custom role, Pianist sim not instantiated", true, null);
                           }

                }
                catch (Exception ex)
                {
                    Message.ShowError(DrunkardsBottle.NAME, "Cannot create custom role", true, ex);
                    this.mCurrentRole = value;
                }
            }
        }

        public void PushRoleStartingInteraction(Actors.Sim sim)
        {
            try
            {
                if (mOwnerType != Owner.Hangaround)
                {
                    //Message.Show("PushRoleStartingInteraction to " + (sim != null ? sim.FullName : "null"));
                    if (sim != null)
                    {

                        IBarProfessional bar = findNearestBar(sim);
                        if (bar != null)
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
            }
            catch (Exception ex)
            {
                Message.Show("Virtual Artisan - Sim cannot perform role " + ex.Message + " : " + ex.StackTrace);
            }
        }

        private static void PushSimToDrink(Actors.Sim sim, IBarProfessional bar, String bestDrinkName)
        {
            //Message.Show("Best drink for me would be " + bestDrinkName);
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
                StringBuilder s = new StringBuilder();
                IEnumerable<InteractionInstance> actions = sim.InteractionQueue.InteractionList;
                List<InteractionInstance> toCancel = new List<InteractionInstance>();
                bool alreadyDrinking = false;
                foreach (InteractionInstance action in actions)
                {
                    if (!(action is BarProfessional.BarInteraction))
                    {
                        Message.Show("Canceling " + action.GetInteractionName());
                        toCancel.Add(action);
                    }
                    else
                    {

                        Message.Show("Sim is already drinking");
                        alreadyDrinking = true;
                        break;
                    }
                    s.Append(action.GetInteractionName());
                    s.Append(" - ");
                    s.Append(action.GetType().ToString());
                    s.Append("\n");
                    // BArProfessional order drink
                }

                foreach (InteractionInstance action in toCancel)
                {
                    sim.InteractionQueue.CancelInteraction(action, false);
                }
                //Message.Show("Heading for a " + bestDrinkName + " after I have done: " + s.ToString());
                if (!alreadyDrinking)
                {
                    sim.InteractionQueue.AddAfterCheckingForDuplicates(drinkingDefinition.CreateInstance(bar, sim, new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), true, false));
                }
            }
            else
            {
                Message.Show("I wanted " + bestDrinkName + " but it was not in the list");
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
            return "";
        }

        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Pianist; }
        }
    }
}
