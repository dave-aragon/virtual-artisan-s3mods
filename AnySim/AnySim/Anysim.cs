using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;
using Misukisu.Anysim;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using System.Diagnostics;
using Sims3.Gameplay.Objects.Misukisu;
using Misukisu.Sims3.Gameplay.Interactions.Anysim;

namespace Sims3.Gameplay.Roles.Misukisu
{
    public class Anysim : Sims3.Gameplay.Skills.Bartending.Bartender
    {
        private bool mIsStoryProgressionProtected = false;

        public override string CareerTitleKey
        {
            get
            {
                return Texts.CAREERTITLE;
            }
        }
        public Anysim(RoleData data, SimDescription s, IRoleGiver roleGiver)
            : base(data, s, roleGiver)
        { }

        public new void RemoveSimFromRole()
        {
            //Message.Show("Sim is removed from role " + new StackTrace().ToString());
            base.RemoveSimFromRole();
        }

        public static Anysim clone(Role toClone, Sim simInRole)
        {
            SimDescription actor = findSimInRole(toClone, simInRole);

            Anysim newRole = null;
            if (actor != null)
            {
                newRole = new Anysim(toClone.Data, actor, toClone.RoleGivingObject);
                newRole.StartRole();
            }
            return newRole;
        }

        public override void SimulateRole(float minPassed)
        {
            //Message.Show("Custom role in simulation " + new StackTrace().ToString());
            base.SimulateRole(minPassed);
        }

        private static SimDescription findSimInRole(Role toClone, Sim simInRole)
        {
            SimDescription actor = null;
            //StringBuilder s = new StringBuilder();
            List<SimDescription> townies = Household.AllTownieSimDescriptions();
            foreach (SimDescription townie in townies)
            {

                //if (townie.AssignedRole != null)
                //{
                //    s.Append("\n"+townie.FullName + " has role " + townie.AssignedRole.GetType().Name);
                //}

                if (townie.AssignedRole == toClone || simInRole == townie.CreatedSim)
                {
                    actor = townie;
                    //Message.Show("Clone found the actor");
                    break;
                }
            }

            if (actor == null)
            {
                List<SimDescription> sims = Household.AllSimsLivingInWorld();
                foreach (SimDescription townie in sims)
                {
                    //if (townie.AssignedRole != null)
                    //{
                    //    s.Append("\n" + townie.FullName + " has role " + townie.AssignedRole.GetType().Name);
                    //}
                    if (townie.AssignedRole == toClone || simInRole == townie.CreatedSim)
                    {
                        actor = townie;
                        //Message.Show("Clone found the actor");
                        break;
                    }
                }
            }

            if (actor == null)
            {
                List<SimDescription> sims = Household.EveryHumanSimDescription();
                foreach (SimDescription townie in sims)
                {
                    //if (townie.AssignedRole != null)
                    //{
                    //    s.Append("\n" + townie.FullName + " has role " + townie.AssignedRole.GetType().Name);
                    //}
                    if (townie.AssignedRole == toClone || simInRole == townie.CreatedSim)
                    {
                        actor = townie;
                        //Message.Show("Clone found the actor");
                        break;
                    }
                }
            }

            //Message.Show("Roles were: "+ s.ToString());

            return actor;
        }

        private void ProtectSimFromStoryProgression()
        {
            if ((this.SimInRole != null) && !this.mIsStoryProgressionProtected)
            {
                this.mSim.GetMiniSimForProtection().AddProtection(MiniSimDescription.ProtectionFlag.FullFromOccupationJob);
                this.mIsStoryProgressionProtected = true;
            }
        }

        private void UnprotectSimFromStoryProgression()
        {
            if (((this.SimInRole != null) && this.mIsStoryProgressionProtected) && !GameStates.IsGameShuttingDown)
            {
                this.mSim.GetMiniSimForProtection().RemoveProtection(MiniSimDescription.ProtectionFlag.FullFromOccupationJob);
                this.mIsStoryProgressionProtected = false;
            }
        }

        protected override void EndRole()
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

            }
        }

        protected override void SwitchIntoOutfit()
        {
            //Message.Show("Switching into outft");
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
                }

            }
            catch (Exception e)
            {
                Message.ShowError(Texts.NAME, "Cannot change anysim clothes: ", false, e);
            }

        }

        protected override void StartRole()
        {
            //Message.Show("starting new custom role ");
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
                Message.ShowError(Texts.NAME, "Cannot start the role", false, e);
            }
        }



        public void MakeSimComeToRoleLot()
        {
            if (this.mSim.CreatedSim.LotCurrent == null || !this.mSim.CreatedSim.LotCurrent.IsCommunityLot)
            {
                this.mSim.CreatedSim.InteractionQueue.Add(
                    Sim.GoToLotThatSatisfiesMyRole.Singleton.CreateInstance(this.mSim.CreatedSim, this.mSim.CreatedSim,
                    new InteractionPriority(InteractionPriorityLevel.High), true, true));
            }
        }



        public void InstantiateSim()
        {
            if (this.mSim.CreatedSim == null)
            {
                Lot lot = LotManager.SelectRandomLotForNPCMoveIn(null);
                this.mSim.CreatedSim = this.mSim.Instantiate(lot);
                //Message.Show("Sim instantiated: " + SimInRole);
            }

        }

    }

}