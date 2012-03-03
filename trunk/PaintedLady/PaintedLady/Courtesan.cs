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
            //Message.Show("Sim is removed from role " + new StackTrace().ToString());
            base.RemoveSimFromRole();
        }

        public static Courtesan clone(Role toClone, Sim simInRole)
        {
            SimDescription actor = findSimInRole(toClone, simInRole);

            Courtesan newRole = null;
            if (actor != null)
            {
                newRole = new Courtesan(toClone.Data, actor, toClone.RoleGivingObject);
                newRole.StartRole();
            }
            return newRole;
        }


        public override void SimulateRole(float minPassed)
        {
            // No Push needed, sim will work on its on
            //Message.Show("Custom role in simulation " + new StackTrace().ToString());
            // base.SimulateRole(minPassed);
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

            }
        }

        public override void SwitchIntoOutfit()
        {
            //Message.Show("Switching into outft");
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
                Message.ShowError(CourtesansPerfume.NAME, "Cannot change courtesan's clothes: ", false, e);
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
            //Message.Show("starting new custom role ");
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
                Message.ShowError(CourtesansPerfume.NAME, "Cannot start the role", false, e);
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
                //Message.Show("Called sim to arrive");
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

       

       
    }

}