using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;
using Misukisu.Drunkard;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using System.Diagnostics;
using Sims3.Gameplay.Objects.Misukisu;
using Misukisu.Sims3.Gameplay.Interactions.Drunkard;

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
            SimDescription actor = findSimInRole(toClone, simInRole);

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
                Lot roleLot = (base.RoleGivingObject as DrunkardsBottle).GetTargetLot();
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
                Message.ShowError(DrunkardsBottle.NAME, "Cannot change tippler's clothes: ", false, e);
            }

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

                        // Give her some drinking money
                        this.mSim.CreatedSim.ModifyFunds(100);

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
                Message.ShowError(DrunkardsBottle.NAME, "Cannot start the role", false, e);
            }
        }

        public void AddNeededMotives()
        {
            Sim sim = this.mSim.CreatedSim;
            DrunkardsBottle bottle = this.RoleGivingObject as DrunkardsBottle;

            if (bottle != null)
            {
                if (sim != null)
                {
                    if (bottle.OwnerType != DrunkardsBottle.Owner.Hangaround)
                    {
                        Lot targetLot = bottle.GetTargetLot();
                        if (targetLot.IsCommunityLot)
                        {
                            sim.Motives.CreateMotive(Autonomy.CommodityKind.BeInDiveBar);
                            //Message.Show("Added motives to drink");
                        }
                    }
                }
            }
        }

        public void MakeSimComeToRoleLot()
        {
            Lot roleLot = (base.RoleGivingObject as DrunkardsBottle).GetTargetLot();

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

    }

}