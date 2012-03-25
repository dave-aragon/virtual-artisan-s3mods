using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;

using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using System.Diagnostics;
using Sims3.Gameplay.Objects.Misukisu;
using Sims3.Gameplay.Skills;
using System.Collections;
using Sims3.Gameplay.Autonomy;
using Misukisu.Dancer;

namespace Sims3.Gameplay.Roles.Misukisu
{
    public class ExoticDancer : Pianist
    {

        public ExoticDancer()
            : base()
        { }

        public ExoticDancer(RoleData data, SimDescription s, IRoleGiver roleGiver)
            : base(data, s, roleGiver)
        { }

        public override string CareerTitleKey
        {
            get
            {
                string title=Texts.CAREERTITLE;
                DancersStage stage= RoleGivingObject as DancersStage;
                if (stage != null && stage.IsExoticShow())
                {
                    title = Texts.CAREERTITLE_EXOTIC;
                }

                return title;
            }
        }

        public new void RemoveSimFromRole()
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, "Sim " + mSim.FullName + " will be removed from role");
            }

            base.RemoveSimFromRole();
        }

        public static ExoticDancer clone(Role toClone, SimDescription actor)
        {
            ExoticDancer newRole = new ExoticDancer(toClone.Data, actor, toClone.RoleGivingObject);
            newRole.StartRole();

            return newRole;
        }


        public override void SimulateRole(float minPassed)
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.Debug(this, mSim.FullName + " role push, minPassed=" + minPassed);
            }
            if (base.IsActive)
            {
                Sim createdSim = base.mSim.CreatedSim;
                if (((createdSim != null) && (base.RoleGivingObject != null)) && ((createdSim.LotCurrent == base.RoleGivingObject.LotCurrent)
                   ))
                {
                   
                    base.RoleGivingObject.PushRoleStartingInteraction(createdSim);
                }
            }
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
           
            base.RoleGivingObject.RemoveRoleGivingInteraction(base.mSim.CreatedSim);
            UnprotectSimFromStoryProgression();
            Sim createdSim = base.mSim.CreatedSim;
            if (IsActive && (createdSim != null))
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
                Lot.MetaAutonomyType venueType = base.RoleGivingObject.LotCurrent.GetMetaAutonomyType;
                Sim sim = this.mSim.CreatedSim;
                SwitchToProperClothing(venueType, sim, false);
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(this, "Cannot change sim's clothes", false, e);
            }

        }

        public new void FreezeMotivesWhilePlaying()
        {
            base.FreezeMotivesWhilePlaying();
            if (SimInRole != null)
            {
                SimInRole.Motives.SetDecay(CommodityKind.Hygiene, false);
            }
        }

        public new void UnfreezeMotivesWhileNotPlaying()
        {
            base.UnfreezeMotivesWhileNotPlaying();
            if (SimInRole != null)
            {
                base.SimInRole.Motives.SetDecay(CommodityKind.Hygiene, true);
            }
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
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Role clothing change request (with spin) for "
                        + mSim.FullName + " " + outfitType.ToString());
                }
            }
            else
            {
                sim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.GoingToWork, outfitType);
                if (Message.Sender.IsDebugging())
                {
                    Message.Sender.Debug(this, "Role clothing change request (without spin) for "
                        + mSim.FullName + " " + outfitType.ToString());
                }

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
                Message.Sender.ShowError(this, "Cannot start the role", false, e);
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
                        //Message.Sender.Show("Maxed the dancing skills!");
                    }
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(this, "Cannot add skills to dancer", false, e);
            }
        }

        public void MakeSimComeToRoleLot()
        {
            Lot roleLot = base.RoleGivingObject.LotCurrent;
            if ((this.mSim.CreatedSim.LotCurrent == null) || !(this.mSim.CreatedSim.LotCurrent == roleLot))
            {
                this.mSim.CreatedSim.InteractionQueue.Add(Sim.GoToLotThatSatisfiesMyRole.Singleton.CreateInstance(this.mSim.CreatedSim, this.mSim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.High), true, true));
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