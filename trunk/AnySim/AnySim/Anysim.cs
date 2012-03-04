using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;
using Misukisu.Common;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using System.Diagnostics;
using Sims3.Gameplay.Objects.Misukisu;
using Misukisu.Sims3.Gameplay.Interactions.Anysim;
using Misukisu.Anysim;

namespace Sims3.Gameplay.Roles.Misukisu
{
    public class Anysim : Sims3.Gameplay.Skills.Bartending.Bartender
    {

        public override string CareerTitleKey
        {
            get
            {
                return Texts.CAREERTITLE;
            }
        }

        public Anysim(RoleData data, SimDescription s, IRoleGiver roleGiver)
            : base(data, s, roleGiver)
        {
            base.mIsStoryProgressionProtected = false;
        }

        public new void RemoveSimFromRole()
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "Sim " + mSim.FullName + " will be removed from AnySim role");
            }
            base.RemoveSimFromRole();
        }

        public static Anysim clone(Role toClone, SimDescription actor)
        {
            Anysim newRole = new Anysim(toClone.Data, actor, toClone.RoleGivingObject);
            newRole.StartRole();

            return newRole;
        }

        public override void SimulateRole(float minPassed)
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, mSim.FullName + " AnySim role push, minPassed=" + minPassed);
            }
            base.SimulateRole(minPassed);
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
                //        createdSim.Motives.RestoreDecays();
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
                Lot roleLot = (base.RoleGivingObject as AnysimObject).GetTargetLot();
                if (roleLot != null)
                {
                    this.mSim.CreatedSim.InteractionQueue.CancelAllInteractions();

                    Lot.MetaAutonomyType venueType = roleLot.GetMetaAutonomyType;
                    SimIFace.CAS.OutfitCategories outfitType = SimIFace.CAS.OutfitCategories.Everyday;
                    if (venueType == Lot.MetaAutonomyType.CocktailLoungeAsian || venueType == Lot.MetaAutonomyType.CocktailLoungeCelebrity || venueType == Lot.MetaAutonomyType.CocktailLoungeVampire)
                    {
                        outfitType = SimIFace.CAS.OutfitCategories.Formalwear;
                    }

                    this.mSim.CreatedSim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.GoingToWork, outfitType);
                    if (Message.Sender.IsDebugging())
                    {
                        Message.Sender.Debug(this, "AnySim role clothing change request for "
                            + mSim.FullName + " " + outfitType.ToString());
                    }
                }

            }
            catch (Exception e)
            {
                Message.Sender.ShowError(Texts.NAME, "Cannot change anysim clothes: ", false, e);
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
                        //AddNeededMotives();

                        if (base.RoleGivingObject != null)
                        {
                            this.mIsActive = true;
                            MakeSimComeToRoleLot();
                            ProtectSimFromStoryProgression();
                            if (base.mSim.CreatedSim != null)
                            {
                                base.mRoleGivingObject.AddRoleGivingInteraction(base.mSim.CreatedSim);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(Texts.NAME, "Cannot start the role", false, e);
            }
        }



        public void MakeSimComeToRoleLot()
        {


            if (this.mSim.CreatedSim.LotCurrent == null || this.mSim.CreatedSim.LotCurrent != RoleGivingObject.LotCurrent)
            {
                this.mSim.CreatedSim.InteractionQueue.Add(
                    Sim.GoToLotThatSatisfiesMyRole.Singleton.CreateInstance(this.mSim.CreatedSim, this.mSim.CreatedSim,
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

    }

}