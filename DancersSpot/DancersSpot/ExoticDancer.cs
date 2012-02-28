using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;

using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using System.Diagnostics;
using Misukisu.DancerSpot;
using Sims3.Gameplay.Objects.Misukisu;
using Sims3.Gameplay.Skills;
using System.Collections;
using Sims3.Gameplay.Autonomy;

namespace Sims3.Gameplay.Roles.Misukisu
{
    public class ExoticDancer : Pianist
    {
        private bool mIsStoryProgressionProtected = false;

        public ExoticDancer()
            : base()
        { }

        public ExoticDancer(RoleData data, SimDescription s, IRoleGiver roleGiver)
            : base(data, s, roleGiver)
        { }

        public new void RemoveSimFromRole()
        {
            //Message.Show("Sim is removed from role " + new StackTrace().ToString());
            base.RemoveSimFromRole();
        }

        public static ExoticDancer clone(Role toClone, Sim simInRole)
        {
            SimDescription actor = findSimInRole(toClone, simInRole);

            ExoticDancer newRole = null;
            if (actor != null)
            {
                newRole = new ExoticDancer(toClone.Data, actor, toClone.RoleGivingObject);
                newRole.StartRole();
            }
            return newRole;
        }


        public override void SimulateRole(float minPassed)
        {
            if (base.IsActive)
            {
                Sim createdSim = base.mSim.CreatedSim;
                if (((createdSim != null) && (base.RoleGivingObject != null)) && ((createdSim.LotCurrent == base.RoleGivingObject.LotCurrent) && !this.IsOnBreak()))
                {
                    base.RoleGivingObject.PushRoleStartingInteraction(createdSim);
                }
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
                createdSim.Motives.RestoreDecays();
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
                Lot.MetaAutonomyType venueType = base.RoleGivingObject.LotCurrent.GetMetaAutonomyType;
                Sim sim = this.mSim.CreatedSim;
                SwitchToProperClothing(venueType, sim,false);

            }
            catch (Exception e)
            {
                Message.ShowError(DancersStage.NAME, "Cannot change sim's clothes", false, e);
            }

        }

        public new  void FreezeMotivesWhilePlaying()
        {
            base.FreezeMotivesWhilePlaying();
            base.SimInRole.Motives.SetDecay(CommodityKind.Hygiene, false);
        }

        public new void UnfreezeMotivesWhileNotPlaying()
        {
            base.UnfreezeMotivesWhileNotPlaying();
            base.SimInRole.Motives.SetDecay(CommodityKind.Hygiene, true);
        }

        public void AfterShowTasks()
        {
            SwitchIntoOutfit();
            UnfreezeMotivesWhileNotPlaying();
        }


       private void SwitchToProperClothing(Lot.MetaAutonomyType venueType, Sim sim, bool spin)
        {
            SimIFace.CAS.OutfitCategories outfitType = SimIFace.CAS.OutfitCategories.Everyday;
            if (venueType == Lot.MetaAutonomyType.CocktailLoungeAsian || venueType == Lot.MetaAutonomyType.CocktailLoungeCelebrity || venueType == Lot.MetaAutonomyType.CocktailLoungeVampire)
            {
                outfitType = SimIFace.CAS.OutfitCategories.Formalwear;
            }

            if (spin)
            {
                sim.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToWork, outfitType);
            }
            else
            {
                sim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.GoingToWork, outfitType);
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
                   
                    if (this.mSim.CreatedSim != null)
                    {
                        this.mSim.CreatedSim.InteractionQueue.CancelAllInteractions();
                        this.SwitchIntoOutfit();
                        //AddNeededMotives();
                        AddNeededSkills();

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
                Message.ShowError(DancersStage.NAME, "Cannot start the role", false, e);
            }
        }

        private void AddNeededSkills()
        {
            try
            {
                SkillManager skillManager = base.mSim.SkillManager;
                if (skillManager != null)
                {
                    Skill skill = skillManager.GetSkill<Skill>(SkillNames.ClubDancing);
                    if (skill == null)
                    {
                        skillManager.AddAutomaticSkill(SkillNames.ClubDancing);
                        skill = skillManager.GetSkill<Skill>(SkillNames.ClubDancing);
                    }

                    if (skill != null)
                    {
                        skill.SkillLevel = skill.MaxSkillLevel;
                        //Message.Show("Maxed the dancing skills!");
                    }

                    //StringBuilder s = new StringBuilder();
                    //ICollection<Skill> skills = skillManager.List;
                    //foreach (Skill i in skills)
                    //{
                    //    s.Append(i.Name);
                    //    s.Append(" - ");
                    //    s.Append(i.GetType().Name);
                    //    s.Append("\n");
                    //}

                    //Message.Show("Skills if this sim: " + s.ToString());

                }
            }
            catch (Exception e)
            {
                Message.ShowError(DancersStage.NAME, "Cannot add skills to dancer", false, e);
            }
        }


        // TODO: set the career title keys
        public override string CareerTitleKey
        {
            get
            {
                return "Gameplay/Objects/Miscellaneous/VelvetRopes:BouncerRoleName";
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