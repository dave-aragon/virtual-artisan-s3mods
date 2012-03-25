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
using Misukisu.Dancer;

namespace Sims3.Gameplay.Objects.Misukisu
{
    public class DancersStage : TableBarPubRound, IRoleGiverExtended, IRoleGiver
    {
        private Roles.Role mCurrentRole;

        private float[] mShowTimes = new float[] { 19F, 23F };
        private float mShowDurationMins = 60F;
        private OutfitCategories[] mShowOutfits = new OutfitCategories[] { OutfitCategories.Career };
        private int mShowIndex = 0;
        private long mNextShowTime = 0;
        private float mStartRoleAt = 18F;
        private float mEndRoleAt = 1F;

        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(TuneExoticDancer.Singleton);
           
            base.AddInteraction(ToggleDebugger.Singleton);
        }

        public override void AddBuildBuyInteractions(List<InteractionDefinition> buildBuyInteractions)
        {
            buildBuyInteractions.Add(ToggleDebugger.Singleton);
            buildBuyInteractions.Add(TuneExoticDancer.Singleton);
            base.AddBuildBuyInteractions(buildBuyInteractions);
        }

        public void TuningChanged(float[] newShowTimes, float newShowDuration, OutfitCategories newFirstOutfit, OutfitCategories newLastOutfit)
        {

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

            CalculateRoleTimes();

            Message.Sender.Show(this, "new Show outfits are: " + OutfitsToString(mShowOutfits, " - "));
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "new Show outfits are: " + OutfitsToString(mShowOutfits, " - "));
            }
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
            startTime = mStartRoleAt;
            endTime = mEndRoleAt;
        }

        private void CalculateRoleTimes()
        {
            float startTime = mShowTimes[0] - 1F;
            float endTime = mShowTimes[mShowTimes.Length - 1] + (mShowDurationMins / 60) + 1F;
            if (startTime != 0)
            {
                startTime = (startTime % 24);
            }

            if (endTime != 0)
            {
                endTime = (endTime % 24);
            }

            mStartRoleAt = startTime;
            mEndRoleAt = endTime;

            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "Role times are - startTime="
                    + mStartRoleAt + " endTime=" + mEndRoleAt);
            }
        }

        public void AddRoleGivingInteraction(Actors.Sim sim)
        {
            this.mShowIndex = 0;
            calculateNextShowTime();
            base.AddInteraction(StartShowNow.Singleton);
        }

        private void calculateNextShowTime()
        {
            if (mShowIndex < ShowTimes.Length)
            {
                float timeUntilShow = SimClock.HoursUntil(ShowTimes[mShowIndex]);

                long timeAsLong = SimClock.ConvertToTicks(timeUntilShow, TimeUnit.Hours);
                this.mNextShowTime = SimClock.CurrentTicks + timeAsLong;
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Show " + mShowIndex + " is in " + timeUntilShow.ToString() + " hours");
                }
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
                CalculateRoleTimes();
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
                    ExoticDancer aRole = ExoticDancer.clone(value, currentActor);
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
                if (sim != null && this.mNextShowTime <= SimClock.CurrentTicks)
                {
                    if (Message.Sender.IsDebugging())
                    {
                        Message.Sender.Debug(this, "It is SHOWTIME");
                    }
                    calculateNextShowTime();
                    PushSimToPerformShow(sim);
                }
            }
            catch (Exception ex)
            {
                Message.Sender.ShowError(this, "Sim cannot play the role", false, ex);
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
                    if (Message.Sender.IsDebugging())
                    {
                        Message.Sender.Debug(this, sim.FullName + " pushed to dance");
                    }
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
                Message.Sender.ShowError(this, "Sim cannot pee before show", false, ex);
            }
        }

        private IToilet findNearestToilet(Actors.Sim sim)
        {
            return GlobalFunctions.GetClosestObject<IToilet>(sim, true, true, new List<IToilet>(), null);

        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {
            base.RemoveInteractionByType(StartShowNow.Singleton.GetType());
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

        internal bool IsExoticShow()
        {
            bool exotic = false;

            foreach (OutfitCategories outfit in mShowOutfits) {
                if (outfit == OutfitCategories.Naked)
                {
                    exotic = true;
                    break;
                }
            }
            return exotic;
        }
    }
}
