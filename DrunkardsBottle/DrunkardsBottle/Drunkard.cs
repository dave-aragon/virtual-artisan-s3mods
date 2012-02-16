using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;
using Misukisu;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using System.Diagnostics;

namespace Sims3.Gameplay.Roles.Misukisu
{
    public class Drunkard : Pianist
    {
        public Drunkard()
            : base()
        { }

        public Drunkard(RoleData data, SimDescription s, IRoleGiver roleGiver)
            : base(data, s, roleGiver)
        { }

        public new void RemoveSimFromRole()
        {
            //Message.Show("Sim is removed from role " + new StackTrace().ToString());
            base.RemoveSimFromRole();
        }

        public static Drunkard clone(Role toClone, Sim simInRole)
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

            Drunkard newRole = null;
            if (actor != null)
            {
                newRole = new Drunkard(toClone.Data, actor, toClone.RoleGivingObject);
                newRole.StartRole();
            }
            return newRole;
        }

  
        public override void SimulateRole(float minPassed)
        {
            //Message.Show("Custom role in simulation " + new StackTrace().ToString());
            base.SimulateRole(minPassed);
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
                SimIFace.CAS.OutfitCategories outfitType = SimIFace.CAS.OutfitCategories.Everyday;
                if (venueType == Lot.MetaAutonomyType.CocktailLoungeAsian || venueType == Lot.MetaAutonomyType.CocktailLoungeCelebrity || venueType == Lot.MetaAutonomyType.CocktailLoungeVampire)
                {
                    outfitType = SimIFace.CAS.OutfitCategories.Formalwear;
                }

                this.mSim.CreatedSim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.GoingToWork, outfitType);

            }
            catch (Exception e)
            {
                Message.Show("Virtual Artisan - Cannot change Regular's clothes: " + e.Message + " - " + e.StackTrace);
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
                        // Give her some drinking money
                        this.mSim.CreatedSim.ModifyFunds(100);
                        // And a motive to drink
                       // this.mSim.CreatedSim.Motives.CreateMotive(Sims3.Gameplay.Autonomy.CommodityKind.);


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
                Message.Show("Virtual Artisan - Cannot start role: " + e.Message + " - " + e.StackTrace);
            }
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