using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;

using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using System.Diagnostics;
using Misukisu.Sims3.Gameplay.Interactions.Paintedlady;
using Sims3.Gameplay.Objects.Misukisu;
using Misukisu.Paintedlady;
using Sims3.Gameplay.Socializing;

namespace Sims3.Gameplay.Roles.Misukisu
{
    public class Courtesan : Pianist
    {

        public Courtesan()
            : base()
        { }

        public override string CareerTitleKey
        {
            get
            {
                return Texts.CAREERTITLE;
            }
        }

        public Courtesan(RoleData data, SimDescription s, IRoleGiver roleGiver)
            : base(data, s, roleGiver)
        { }

        public new void RemoveSimFromRole()
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "Sim " + mSim.FullName + " will be removed from role");
            }
            base.RemoveSimFromRole();
        }

        public static bool IsTalkingTo(Sim actor, Sim target, bool result)
        {
            SocialComponent social = target.SocialComponent;
            if (social != null)
            {
                Conversation conversation = social.Conversation;
                if (conversation != null && conversation.ContainsSim(actor))
                {
                    result = true;
                }
            }
            return result;
        }

        public static Courtesan AssignedRole(Sim sim)
        {
            Role role = sim.SimDescription.AssignedRole;
            Courtesan courtesan = role as Courtesan;
            return courtesan;
        }

        public static Courtesan clone(Role toClone, SimDescription actor)
        {
            Courtesan newRole = new Courtesan(toClone.Data, actor, toClone.RoleGivingObject);
            newRole.StartRole();

            return newRole;
        }


        public override void SimulateRole(float minPassed)
        {
            // No Push needed, sim will work on its on
        }

        public override bool IsStoryProgressionProtected
        {
            get
            {
                return true;
            }
        }

        public override void EndRole()
        {
            bool isActive = base.IsActive;
            base.RoleGivingObject.RemoveRoleGivingInteraction(base.mSim.CreatedSim);
            UnprotectSimFromStoryProgression();
            Sim createdSim = base.mSim.CreatedSim;
            if (isActive && (createdSim != null))
            {
                mIsActive = false;
                // CreatedSim.Motives.RemoveMotive(kind);
                createdSim.Motives.RestoreDecays();
                createdSim.InteractionQueue.CancelAllInteractions();
                //this.mSim.CreatedSim.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday);
                Sim.MakeSimGoHome(createdSim, false);
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Sim " + mSim.FullName + " was sent home");
                }
            }
        }


        public override void SwitchIntoOutfit()
        {
            try
            {
                Lot roleLot = (base.RoleGivingObject as CourtesansPerfume).GetTargetLot();
                if (roleLot != null)
                {
                    Sim sim = this.mSim.CreatedSim;
                    this.mSim.CreatedSim.InteractionQueue.CancelAllInteractions();

                    Lot.MetaAutonomyType venueType = roleLot.GetMetaAutonomyType;
                    SwitchToProperClothing(sim, venueType);
                    if (Message.Sender.IsDebugging())
                    {
                        Message.Sender.Debug(this, "Role clothing change request made "
                            + mSim.FullName + " " + venueType.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(this, "Cannot change courtesan's clothes: ", false, e);
            }
        }

        public static void SwitchToProperClothing(Sim sim, Lot.MetaAutonomyType venueType)
        {
            SimIFace.CAS.OutfitCategories outfitType = SimIFace.CAS.OutfitCategories.Everyday;
            if (venueType == Lot.MetaAutonomyType.CocktailLoungeAsian || venueType == Lot.MetaAutonomyType.CocktailLoungeCelebrity || venueType == Lot.MetaAutonomyType.CocktailLoungeVampire)
            {
                outfitType = SimIFace.CAS.OutfitCategories.Formalwear;
            }

            sim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.GoingToWork, outfitType);

        }

        public override void StartRole()
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, mSim.FullName + " role starting...");
            }
            try
            {
                if (!this.mIsActive && this.mSim.IsValidDescription)
                {
                    InstantiateSim();
                    this.SwitchIntoOutfit();
                    Sim sim = this.mSim.CreatedSim;
                    if (sim != null)
                    {
                        AddNeededMotives();
                        CourtesansPerfume perfume = GetPerfume();
                        if (perfume != null)
                        {
                            this.mIsActive = true;
                            perfume.PayIfNecessary(sim);
                            MakeSimComeToRoleLot();
                            ProtectSimFromStoryProgression();

                            perfume.AddRoleGivingInteraction(sim);
                            perfume.PushRoleStartingInteraction(sim);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(this, "Cannot start the role", false, e);
            }
        }

        private void AddNeededMotives()
        {
            // Nothing needed at the moment
        }

        public void MakeSimComeToRoleLot()
        {
            Lot roleLot = (base.RoleGivingObject as CourtesansPerfume).GetTargetLot();

            if (roleLot != null)
            {
                this.mSim.CreatedSim.InteractionQueue.Add(GoToALot.Singleton.CreateInstance(this.mSim.CreatedSim, this.mSim.CreatedSim,
                    new InteractionPriority(InteractionPriorityLevel.High), true, true));
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Sim called from "
                        + this.mSim.CreatedSim.LotCurrent.Name + " to " + roleLot.Name);
                }
            }
        }

        public void InstantiateSim()
        {
            if (this.mSim.CreatedSim == null)
            {
                Lot lot = LotManager.SelectRandomLotForNPCMoveIn(null);
                this.mSim.CreatedSim = this.mSim.Instantiate(lot);
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Sim instantiated: " + mSim.FullName);
                }
            }

        }

        public CourtesansPerfume GetPerfume()
        {
            return (RoleGivingObject as CourtesansPerfume);
        }

        public static CourtesansPerfume GetPerfume(Sim sim)
        {
            CourtesansPerfume perfume = null;
            Courtesan role = Courtesan.AssignedRole(sim);
            if (role != null)
            {
                perfume = role.GetPerfume();
            }
            return perfume;
        }
    }

}