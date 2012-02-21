using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;

using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using System.Diagnostics;
using Misukisu.Common;
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
            SimDescription actor = null;
            List<SimDescription> townies = Household.AllTownieSimDescriptions();
            foreach (SimDescription townie in townies)
            {
                if (simInRole == townie.CreatedSim)
                {
                    actor = townie;
                    //Message.Show("Clone found the actor");
                    break;
                }
            }

            if (actor == null)
            {
                List<SimDescription> residents = Household.AllSimsLivingInWorld();
                foreach (SimDescription townie in residents)
                {
                    if (simInRole == townie.CreatedSim)
                    {
                        actor = townie;
                        //Message.Show("Clone found the actor");
                        break;
                    }
                }
            }

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

        protected override void EndRole()
        {
            //Message.Show("Custom role ending " + new StackTrace().ToString());
            base.EndRole();
            //if (this.mSim.CreatedSim != null)
            //{
            //    this.mSim.CreatedSim.SwitchToPreviousOutfitWithoutSpin();
            //}
        }

        protected override void SwitchIntoOutfit()
        {
            //Message.Show("Switching into outft");
            try
            {
                this.mSim.CreatedSim.InteractionQueue.CancelAllInteractions();
                Lot.MetaAutonomyType venueType = base.RoleGivingObject.LotCurrent.GetMetaAutonomyType;
                Sim sim = this.mSim.CreatedSim;
                SwitchToProperClothing(venueType, sim);

            }
            catch (Exception e)
            {
                Message.ShowError(CourtesansPerfume.NAME,"Cannot change sim's clothes",false, e);
            }

        }

        public static void SwitchToProperClothing(Lot.MetaAutonomyType venueType, Sim sim)
        {
            SimIFace.CAS.OutfitCategories outfitType = SimIFace.CAS.OutfitCategories.Everyday;
            if (venueType == Lot.MetaAutonomyType.CocktailLoungeAsian || venueType == Lot.MetaAutonomyType.CocktailLoungeCelebrity || venueType == Lot.MetaAutonomyType.CocktailLoungeVampire)
            {
                outfitType = SimIFace.CAS.OutfitCategories.Formalwear;
            }

            sim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.GoingToWork, outfitType);
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
                        AddNeededMotives();
                        
                        if (base.RoleGivingObject != null)
                        {
                            this.mIsActive = true;
                            MakeSimComeToRoleLot();

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
            //DrunkardsBottle bottle = this.RoleGivingObject as DrunkardsBottle;
            //if (bottle != null)
            //{
            //    if (bottle.OwnerType != DrunkardsBottle.Owner.Hangaround)
            //    {
            //        Lot.MetaAutonomyType venueType = base.RoleGivingObject.LotCurrent.GetMetaAutonomyType;
            //        if (venueType == Lot.MetaAutonomyType.DiveBarCriminal || venueType == Lot.MetaAutonomyType.DiveBarIrish || venueType == Lot.MetaAutonomyType.DiveBarSports)
            //        {
            //            this.mSim.CreatedSim.Motives.CreateMotive(Autonomy.CommodityKind.BeInDiveBar);
            //        }
            //    }
            //}
        }

        public void MakeSimComeToRoleLot()
        {
            Lot roleLot = base.RoleGivingObject.LotCurrent;
            if ((this.mSim.CreatedSim.LotCurrent == null) || !(this.mSim.CreatedSim.LotCurrent == roleLot))
            {
                this.mSim.CreatedSim.InteractionQueue.Add(Sim.GoToLotThatSatisfiesMyRole.Singleton.CreateInstance(this.mSim.CreatedSim, this.mSim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.High), true, true));
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

    }

}