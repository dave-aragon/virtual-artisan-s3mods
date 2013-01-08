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
using Misukisu.Paintedlady;
using Sims3.UI.Controller;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.ActorSystems;

namespace Sims3.Gameplay.Objects.Misukisu
{
    public enum GenderPreference
    {
        Bi = 0,
        Straight = 1,
        Gay = 2
    }

    public class RelationData
    {
        public bool areFriends = false;
        public bool areRomantic = false;
        public bool buyerLostAFriend = false;
        public bool sellerLostAFriend = false;
        public float liking;
    }

    public class CourtesansPerfume : GameObject, IRoleGiver, IRoleGiverExtended
    {
        private Roles.Role mCurrentRole;
        public float mStartTime = 0F;
        public float mEndTime = 0F;
        private int mPricePerWoohoo = 100;
        private int mPimpShare = 0;
        public GenderPreference mGenderPreference = GenderPreference.Straight;
        public bool mConfirmWoohoo = true;
        private Sim mPimp = null;

        public int PimpShare
        {
            get { return mPimpShare; }
            set { mPimpShare = value; }
        }

        public int PricePerWoohoo
        {
            get { return mPricePerWoohoo; }
            set { mPricePerWoohoo = value; }
        }

        private Dictionary<Sim, RelationData> mRelationsToRestore =
            new Dictionary<Sim, RelationData>();

        private sealed class PickGenderPref : ImmediateInteraction<Sim, CourtesansPerfume>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, CourtesansPerfume, CourtesansPerfume.PickPricePerWoohoo>
            {
                public override string GetInteractionName(Sim a, CourtesansPerfume target, InteractionObjectPair interaction)
                {
                    return "Set Gender Preference - Currently " + target.mGenderPreference;
                }
                public override bool Test(Sim a, CourtesansPerfume target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new CourtesansPerfume.PickGenderPref.Definition();
            public override bool Run()
            {
                Target.mGenderPreference = ShowGenderDialog(Target.mGenderPreference);
                return true;
            }
        }

        private static GenderPreference ShowGenderDialog(GenderPreference gender)
        {
            Dictionary<string, object> regTypes = new Dictionary<string, object>();
            regTypes.Add(GenderPreference.Bi.ToString(), GenderPreference.Bi.ToString());
            regTypes.Add(GenderPreference.Gay.ToString(), GenderPreference.Gay.ToString());
            regTypes.Add(GenderPreference.Straight.ToString(), GenderPreference.Straight.ToString());


            object result = ComboSelectionDialog.Show("Courtesan's Gender Preference", regTypes, gender.ToString());
            GenderPreference userSelection = gender;
            if (result is string)
            {
                userSelection = (GenderPreference)Enum.Parse(typeof(GenderPreference), result as string, true);
            }
            return userSelection;
        }

        private sealed class PickPimp : ImmediateInteraction<Sim, CourtesansPerfume>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, CourtesansPerfume, CourtesansPerfume.PickPricePerWoohoo>
            {
                public override string GetInteractionName(Sim a, CourtesansPerfume target, InteractionObjectPair interaction)
                {
                    if (target.mPimp == null)
                    {
                        return "Pimp the Courtesan (get part of earnings)";
                    }
                    else
                    {
                        return "Fire the Pimp - Currently " + target.mPimp.FullName;
                    }

                }
                public override bool Test(Sim a, CourtesansPerfume target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new CourtesansPerfume.PickPimp.Definition();
            public override bool Run()
            {
                if (Target.mPimp != null)
                {
                    Target.mPimp = null;
                }
                else
                {
                    Target.mPimp = Actor;
                }

                return true;
            }
        }

        private sealed class PickPricePerWoohoo : ImmediateInteraction<Sim, CourtesansPerfume>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, CourtesansPerfume, CourtesansPerfume.PickPricePerWoohoo>
            {
                public override string GetInteractionName(Sim a, CourtesansPerfume target, InteractionObjectPair interaction)
                {
                    return "Set Price for WooHoo - Currently " + target.PricePerWoohoo;
                }
                public override bool Test(Sim a, CourtesansPerfume target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new CourtesansPerfume.PickPricePerWoohoo.Definition();
            public override bool Run()
            {
                int currentPrice = this.Target.PricePerWoohoo;
                string text = StringInputDialog.Show("Charge per WooHoo", "Enter number of simoleans to be charged per customer per woohoo", string.Concat(currentPrice), true);
                if (text == "")
                {
                    return false;
                }
                this.Target.PricePerWoohoo = this.Target.StringToInt(text, currentPrice);
                return true;
            }
        }

        private sealed class PickPimpsShare : ImmediateInteraction<Sim, CourtesansPerfume>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, CourtesansPerfume, CourtesansPerfume.PickPimpsShare>
            {
                public override string GetInteractionName(Sim a, CourtesansPerfume target, InteractionObjectPair interaction)
                {
                    return "Pimp's Share Per WooHoo - Currently " + target.PimpShare;
                }
                public override bool Test(Sim a, CourtesansPerfume target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.mPimp == null)
                    {
                        return false;
                    }
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new CourtesansPerfume.PickPimpsShare.Definition();
            public override bool Run()
            {
                int currentPrice = this.Target.PimpShare;
                string text = StringInputDialog.Show("Pimp's Share", "Enter number of simoleans the courtesan gives to the pimp from each WooHoo", string.Concat(currentPrice), true);
                if (text == "")
                {
                    return false;
                }
                this.Target.PimpShare = this.Target.StringToInt(text, currentPrice);
                return true;
            }
        }

        // By Inge :)
        public float HourToFloat(string hour, float defaultHour)
        {
            int num;
            if (!int.TryParse(hour, out num))
            {
                return defaultHour;
            }
            if (num > 23 || num < 0)
            {
                return defaultHour;
            }
            return (float)num;
        }
        // By Inge :)
        public string HourToNiceName(float hour)
        {
            if (hour == 0f)
            {
                return "midnight";
            }
            if (hour == 12f)
            {
                return "noon";
            }
            if (hour < 12f)
            {
                return hour.ToString() + " AM";
            }
            hour -= 12f;
            return hour.ToString() + " PM";
        }

        // By Inge :)
        public int StringToInt(string price, int defaultValue)
        {
            int num;
            if (!int.TryParse(price, out num))
            {
                return defaultValue;
            }
            if (num < 0)
            {
                return defaultValue;
            }
            return num;
        }

        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(TuneCourtesan.Singleton);
            base.AddInteraction(PickPimp.Singleton);
            base.AddInteraction(PickGenderPref.Singleton);
            base.AddInteraction(PimpCourtesan.Singleton);
            //base.AddInteraction(AssignCourtesan.Singleton);
            base.AddInteraction(ToggleDebugger.Singleton);
            base.AddInteraction(PickPricePerWoohoo.Singleton);
            base.AddInteraction(PickPimpsShare.Singleton);

        }

        private void RestoreAllOldRelationships()
        {
            if (CurrentRole != null)
            {
                Sim rolesim = CurrentRole.SimInRole;
                if (rolesim != null)
                {
                    List<Sim> sims = new List<Sim>(mRelationsToRestore.Keys);
                    foreach (Sim customer in sims)
                    {
                        restoreOldRelationship(customer);
                    }
                }
            }
        }

        public Sim SimInRole()
        {
            if (CurrentRole != null)
            {
                return CurrentRole.SimInRole;
            }
            return null;
        }

        public void storeAndIncreaseRelationship(Sim buyer, Relationship relationship)
        {
            Sim seller = SimInRole();
            if (!mRelationsToRestore.ContainsKey(buyer) && seller != null)
            {
                RelationData data = new RelationData();
                data.buyerLostAFriend = HasMoodlet(buyer, BuffNames.LostAFriend, Origin.FromSocialization);
                data.sellerLostAFriend = HasMoodlet(seller, BuffNames.LostAFriend, Origin.FromSocialization);
                data.liking = relationship.CurrentLTRLiking;

                if (relationship.AreRomantic())
                {
                    data.areRomantic = true;
                }
                else
                {
                    relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Romantic);
                }

                if (relationship.AreFriends())
                {
                    data.areFriends = true;
                }
                else
                {
                    relationship.LTR.SetLiking(100f);
                }

                mRelationsToRestore.Add(buyer, data);


            }
        }

        public void restoreOldRelationship(Sim buyer)
        {
            Sim seller = SimInRole();
            if (mRelationsToRestore.ContainsKey(buyer) && seller != null)
            {
                RelationData relationshipBefore = mRelationsToRestore[buyer];
                Relationship relationship = seller.GetRelationship(buyer, true);

                relationship.LTR.SetLiking(relationshipBefore.liking);
                
                if (!relationshipBefore.buyerLostAFriend && HasMoodlet(buyer, BuffNames.LostAFriend, Origin.FromSocialization))
                {
                    buyer.BuffManager.RemoveElement(BuffNames.LostAFriend);
                }
                if (!relationshipBefore.sellerLostAFriend && HasMoodlet(seller, BuffNames.LostAFriend, Origin.FromSocialization))
                {
                    seller.BuffManager.RemoveElement(BuffNames.LostAFriend);
                }
                
                if (!relationshipBefore.areRomantic)
                {
                    relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Romantic);
                }
                
                mRelationsToRestore.Remove(buyer);
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Relationship to " + buyer.FullName + " restored ");
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
            if (Pimp != null)
            {
                return Pimp.LotHome;
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
            sim.AddInteraction(ToggleTakeMistress.Singleton);
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "Role interactions added to " + sim.FullName);
            }

        }

        public Sim Pimp
        {
            get
            {
                return this.mPimp;
            }
            set
            {
                this.mPimp = value;
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Courtesan/Courter now pimped by: " + mPimp.FullName);
                }
            }
        }

        public string getRoleSimName()
        {
            string name = "not assigned";
            Role role = CurrentRole;
            if (role != null)
            {
                Sim sim = role.SimInRole;
                if (sim != null)
                {
                    name = sim.FullName;
                }
            }
            return name;
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
                    RestoreAllOldRelationships();
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
                    Message.Sender.ShowError(this, "Cannot create custom role", true, ex);
                    this.mCurrentRole = value;
                }
            }
        }

        public void PushRoleStartingInteraction(Actors.Sim sim)
        {
            try
            {
                Role role = CurrentRole;
                if (role != null && role.IsActive)
                {
                    Message.Sender.Show("PushRoleStartingInteraction to " + (sim != null ? sim.FullName : "null"));
                    if (sim != null)
                    {
                        if (sim.LotCurrent == null || sim.LotCurrent != this.LotCurrent)
                        {
                            Courtesan.forceSimToLot(sim);
                        }
                        else
                        {
                            LetSimIn(sim);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void LetSimIn(Sim s)
        {
            VisitSituation visitSituation = VisitSituation.FindVisitSituationInvolvingGuest(s);
            if (visitSituation != null)
            {
                VisitSituation.SetVisitToGreeted(s);
                VisitSituation.OnInvitedIn(s);
                visitSituation.AllowedToStayOver = true;
                visitSituation.SetStateSocializing();
            }
        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {
            sim.RemoveInteractionByType(BuyWooHoo.Singleton.GetType());
            sim.RemoveInteractionByType(ToggleTakeMistress.Singleton.GetType());
            RestoreAllOldRelationships();
        }



        public string RoleName(bool isFemale)
        {
            return "";
        }

        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Bouncer; }
        }

        public bool Pay(Sim buyer, Sim seller, bool singleWoohoo)
        {
            bool paid = false;
            if (singleWoohoo)
            {
                if (true)//(buyer.LotCurrent != buyer.LotHome)
                {
                    int amount = PricePerWoohoo;
                    if (amount < buyer.FamilyFunds)
                    {
                        buyer.ModifyFunds(-amount);
                        paid = true;
                    }
                }
            }
            return paid;
        }

        public static bool HasMoodlet(Sim sim, BuffNames b, Origin o)
        {
            if (sim.BuffManager.HasElement(b))
            {
                if (sim.BuffManager.GetElement(b).BuffOrigin == o)
                {
                    return true;
                }
                if (sim.BuffManager.GetElement(b).BuffOrigin == Origin.None)
                {
                    return true;
                }
            }
            return false;
        }

        public ResourceKey GetRoleUniformKey(Sim Sim, Lot.MetaAutonomyType venueType)
        {
            return ResourceKey.kInvalidResourceKey;
        }

        internal bool GenderMatches(Sim actor, Sim target)
        {
            if (mGenderPreference == GenderPreference.Bi)
            {
                return true;
            }
            else if (mGenderPreference == GenderPreference.Straight && (actor.IsFemale != target.IsFemale))
            {
                return true;
            }
            else if (mGenderPreference == GenderPreference.Gay && (actor.IsFemale == target.IsFemale))
            {
                return true;
            }

            return false;
        }
    }
}
