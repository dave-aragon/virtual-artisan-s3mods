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
using Misukisu.Common;

namespace Sims3.Gameplay.Roles.Misukisu
{
    public class Courtesan : Pianist
    {

        public Courtesan()
            : base()
        { }

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
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(CourtesansPerfume.NAME, "Cannot change courtesan's clothes: ", false, e);
            }
        }

        public void SwitchToProperClothing(Sim sim, Lot.MetaAutonomyType venueType)
        {
            SimIFace.CAS.OutfitCategories outfitType = SimIFace.CAS.OutfitCategories.Everyday;
            if (venueType == Lot.MetaAutonomyType.CocktailLoungeAsian || venueType == Lot.MetaAutonomyType.CocktailLoungeCelebrity || venueType == Lot.MetaAutonomyType.CocktailLoungeVampire)
            {
                outfitType = SimIFace.CAS.OutfitCategories.Formalwear;
            }

            sim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.GoingToWork, outfitType);
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "Role clothing change request for "
                    + mSim.FullName + " " + outfitType.ToString());
            }
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
                    if (this.mSim.CreatedSim != null)
                    {
                        AddNeededMotives();

                        if (base.RoleGivingObject != null)
                        {
                            this.mIsActive = true;
                            MakeSimComeToRoleLot();
                            ProtectSimFromStoryProgression();
                            if (base.mSim.CreatedSim != null)
                            {
                                base.mRoleGivingObject.AddRoleGivingInteraction(base.mSim.CreatedSim);
                            }


                            if (this.RoleGivingObject != null)
                            {
                                this.RoleGivingObject.PushRoleStartingInteraction(this.mSim.CreatedSim);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(CourtesansPerfume.NAME, "Cannot start the role", false, e);
            }
        }

        private void AddNeededMotives()
        {
            // Nothing needed at the moment
        }

        public void MakeSimComeToRoleLot()
        {
            Lot roleLot = (base.RoleGivingObject as CourtesansPerfume).GetTargetLot();

            if ((this.mSim.CreatedSim.LotCurrent == null) || !(this.mSim.CreatedSim.LotCurrent == roleLot))
            {
                this.mSim.CreatedSim.InteractionQueue.Add(GoToALot.Singleton.CreateInstance(this.mSim.CreatedSim, this.mSim.CreatedSim,
                    new InteractionPriority(InteractionPriorityLevel.High), true, true));
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Sim " + mSim + " called from "
                        + this.mSim.CreatedSim.LotCurrent.Name + " to " + RoleGivingObject.LotCurrent.Name);
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