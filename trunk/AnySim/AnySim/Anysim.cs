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
            //Message.Send.Show("Sim is removed from role " + new StackTrace().ToString());
            base.RemoveSimFromRole();
        }

        public static Anysim clone(Role toClone, Sim simInRole)
        {
            SimDescription actor = toClone.mSim;
            Anysim newRole = new Anysim(toClone.Data, actor, toClone.RoleGivingObject);
            newRole.StartRole();

            return newRole;
        }

        public override void SimulateRole(float minPassed)
        {
            //Message.Send.Show("Custom role in simulation " + new StackTrace().ToString());
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
                    //Message.Send.Show("Clone found the actor");
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
                        //Message.Send.Show("Clone found the actor");
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
                        //Message.Send.Show("Clone found the actor");
                        break;
                    }
                }
            }

            //Message.Send.Show("Roles were: "+ s.ToString());

            return actor;
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

            }
        }

        public override void SwitchIntoOutfit()
        {
            //Message.Send.Show("Switching into outft");
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
                Message.Sender.ShowError(Texts.NAME, "Cannot change anysim clothes: ", false, e);
            }

        }

        public override void StartRole()
        {
            //Message.Send.Show("starting new custom role ");
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
                //Message.Send.Show("Sim instantiated: " + SimInRole);
            }

        }

    }

}