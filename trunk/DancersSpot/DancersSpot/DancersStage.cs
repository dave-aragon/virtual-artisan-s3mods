using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
using Sims3.Gameplay.ActorSystems;
using Misukisu.Common;

namespace Sims3.Gameplay.Objects.Misukisu
{
    public class DancersStage : TableBarPubRound, IRoleGiverExtended, IRoleGiver
    {
        public static string NAME = "Dancer's Stage";

        private Roles.Role mCurrentRole;
        private float mTimeToPee = 0.75F;
        private float[] mShowTimes = new float[] { 19F, 23F };
        private float mShowDurationMins = 60F;
        private OutfitCategories[] mShowOutfits = new OutfitCategories[] { OutfitCategories.Career };
        private int mShowIndex = 0;
        private long mNextShowTime = 0;

        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(TuneExoticDancer.Singleton);
            base.AddInteraction(StartShowNow.Singleton);
            //base.AddInteraction(PerformShow.Singleton);
        }

        public override void AddBuildBuyInteractions(List<InteractionDefinition> buildBuyInteractions)
        {

            buildBuyInteractions.Add(TuneExoticDancer.Singleton);
            base.AddBuildBuyInteractions(buildBuyInteractions);
        }

        public void TuningChanged(float[] newShowTimes, float newShowDuration, OutfitCategories newFirstOutfit, OutfitCategories newLastOutfit)
        {
            //Message.Sender.Show("New tuning, role will reset");
            if (newShowTimes.Length > 0)
            {
                mShowTimes = newShowTimes;
                for (int i = 0; i < mShowTimes.Length; i++)
                {
                    if (mShowTimes[i] < 0) { mShowTimes[i] = 0; }
                    if (mShowTimes[i] > 24) { mShowTimes[i] = 24; }
                }
            }

            mShowDurationMins = newShowDuration;
            if (mShowDurationMins < 0) { mShowDurationMins = 5; }
            if (mShowDurationMins > 300) { mShowDurationMins = 300; }

            if (newFirstOutfit == newLastOutfit)
            {
                mShowOutfits = new OutfitCategories[] { newFirstOutfit };
            }
            else if (newLastOutfit == OutfitCategories.Naked)
            {
                mShowOutfits = new OutfitCategories[] { newFirstOutfit, OutfitCategories.Sleepwear, newLastOutfit };
            }
            else
            {
                mShowOutfits = new OutfitCategories[] { newFirstOutfit, newLastOutfit };
            }

            Message.Sender.Show("new Show outfits are: " + OutfitsToString(mShowOutfits, " - "));

            ResetRole();
        }

        private string OutfitsToString(OutfitCategories[] array, string separator)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < array.Length; ++i)
            {
                result.Append(array[i].ToString());

                if (i < array.Length - 1)
                {
                    result.Append(separator);
                }
            }
            return result.ToString();
        }


        private void ResetRole()
        {
            if (CurrentRole != null)
            {
                if (CurrentRole.SimInRole != null)
                {
                    CurrentRole.SimInRole.InteractionQueue.CancelAllInteractions();
                }
                EndRoleAndReplaceWithNew(CurrentRole);
            }
        }

        public void GetRoleTimes(out float startTime, out float endTime)
        {
            startTime = mShowTimes[0] - mTimeToPee - 1F;
            endTime = mShowTimes[mShowTimes.Length - 1] + mTimeToPee + (mShowDurationMins / 60) + 1F;

        }

        public void AddRoleGivingInteraction(Actors.Sim sim)
        {
            this.mShowIndex = 0;
            calculateNextShowTime();
        }

        private void calculateNextShowTime()
        {
            if (mShowIndex < ShowTimes.Length)
            {
                float timeUntilShow = SimClock.HoursUntil(ShowTimes[mShowIndex] - mTimeToPee);

                long timeAsLong = SimClock.ConvertToTicks(timeUntilShow, TimeUnit.Hours);
                this.mNextShowTime = SimClock.CurrentTicks + timeAsLong;
                //Message.Sender.Show("Show " + mShowIndex + " is in " + timeUntilShow.ToString() + " hours");
                mShowIndex++;
            }
            else
            {
                mNextShowTime = long.MaxValue;
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
                        ExoticDancer aRole = ExoticDancer.clone(value, currentActor);

                        if (aRole != null)
                        {
                            this.mCurrentRole = aRole;
                            RoleManager.sRoleManager.AddRole(aRole);
                        }
                        else
                        {
                            Message.Sender.ShowError(DancersStage.NAME, "Cannot create custom role, clone failed", true, null);
                        }
                    }
                    else
                    {
                        Message.Sender.ShowError(DancersStage.NAME, "Cannot create custom role, Pianist sim not instantiated", true, null);
                    }

                }
                catch (Exception ex)
                {
                    Message.Sender.ShowError(DancersStage.NAME, "Cannot create custom role", true, ex);
                    this.mCurrentRole = value;
                }
            }
        }

        public void PushRoleStartingInteraction(Actors.Sim sim)
        {
            try
            {
                if (sim != null && this.mNextShowTime <= SimClock.CurrentTicks)
                {
                    //Message.Sender.Show("It is SHOWTIME");
                    calculateNextShowTime();
                    PushSimToPerformShow(sim);
                }
            }
            catch (Exception ex)
            {
                Message.Sender.ShowError(DancersStage.NAME, "Sim cannot play the role", false, ex);
            }
        }

        public void PushSimToPerformShow(Actors.Sim sim)
        {
            if (sim != null)
            {
                ExoticDancer currentRole = this.CurrentRole as ExoticDancer;
                if (currentRole != null)
                {
                    currentRole.FreezeMotivesWhilePlaying();


                    //pushSimToPeeBeforeShow(sim);

                    InteractionInstance instance = PerformShow.Singleton.CreateInstance(this, sim,
                        new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), false, false);
                    sim.InteractionQueue.AddAfterCheckingForDuplicates(instance);
                }
            }
        }

        private void TODOCheckThatShowInteractionsDontDuplicate()
        {
        }

        private void pushSimToPeeBeforeShow(Sim sim)
        {
            try
            {
                IToilet toilet = findNearestToilet(sim);
                if (toilet != null && !toilet.InUse)
                {
                    List<InteractionObjectPair> interactions = toilet.GetAllInteractionsForActor(sim);
                    InteractionDefinition peeDefinition = null;
                    foreach (InteractionObjectPair interaction in interactions)
                    {
                        InteractionDefinition interactionDef = interaction.InteractionDefinition;
                        string name = interactionDef.GetType().ToString();


                        if (name != null && name.Contains("UseToilet"))
                        {

                            peeDefinition = interactionDef;
                            break;
                        }
                    }

                    if (peeDefinition != null)
                    {

                        InteractionInstance instance = peeDefinition.CreateInstance(toilet, sim,
                                    new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), false, false);
                        sim.InteractionQueue.AddAfterCheckingForDuplicates(instance);
                    }
                }
            }
            catch (Exception ex)
            {
                Message.Sender.ShowError(DancersStage.NAME, "Sim cannot pee before show", false, ex);
            }
        }

        private IToilet findNearestToilet(Actors.Sim sim)
        {
            return GlobalFunctions.GetClosestObject<IToilet>(sim, true, true, new List<IToilet>(), null);

        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {

        }

        public string RoleName(bool isFemale)
        {
            return "Dancer";
        }


        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Pianist; }
        }


        public float[] ShowTimes
        {
            get { return mShowTimes; }
        }

        public float ShowDurationMins
        {
            get { return mShowDurationMins; }
        }

        public OutfitCategories[] ShowOutfits
        {
            get { return mShowOutfits; }
        }

        public OutfitCategories GetFirstOutfit()
        {
            if (mShowOutfits.Length > 0)
            {
                return mShowOutfits[0];
            }
            else
            {
                return OutfitCategories.Career;
            }
        }

        public OutfitCategories GetLastOutfit()
        {
            if (mShowOutfits.Length > 0)
            {
                return mShowOutfits[mShowOutfits.Length - 1];
            }
            else
            {
                return OutfitCategories.Career;
            }
        }

        public ResourceKey GetRoleUniformKey(Sim Sim, Lot.MetaAutonomyType venueType)
        {
            return ResourceKey.kInvalidResourceKey;
        }
    }
}
