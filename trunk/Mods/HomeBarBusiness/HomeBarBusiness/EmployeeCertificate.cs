using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Gameplay.Objects.Counters;
using Misukisu.HomeBarBusiness;
using Sims3.Gameplay.Situations;

namespace Sims3.Gameplay.Objects.Misukisu
{

    public class EmployeeCertificate : GameObject, IRoleGiver, IRoleGiverExtended
    {
        private Roles.Role mCurrentRole;
     


        public override void OnStartup()
        {
            base.OnStartup();
            AddInteraction(AssignBartender.Singleton);
        }


        public void GetRoleTimes(out float startTime, out float endTime)
        {
            startTime = HomeBartender.startTime;
            endTime = HomeBartender.endTime;
        }

        public void AddRoleGivingInteraction(Actors.Sim sim)
        {

        }

        public Roles.Role CurrentRole
        {
            get
            {
                return this.mCurrentRole;
            }
            set
            {
                HomeBartender newRole = value as HomeBartender;
                if (newRole != null)
                {
                    this.mCurrentRole = newRole;
                }

            }

        }



        public void PushRoleStartingInteraction(Actors.Sim sim)
        {
            try
            {

                //Message.Sender.Show("PushRoleStartingInteraction to " + (sim != null ? sim.FullName : "null"));
                if (sim != null)
                {
                    if (sim.LotCurrent == null || sim.LotCurrent != this.LotCurrent)
                    {
                        HomeBartender.forceSimToLot(sim);
                    }
                    else
                    {
                        LetSimIn(sim);
                        List<BarProfessional> bars = this.LotCurrent.GetObjectsInRoom<BarProfessional>(this.RoomId);
                        BarProfessional theBar = null;

                        foreach (BarProfessional bar in bars)
                        {
                            if (!isBarManned(bar))
                            {
                                theBar = bar;
                                break;
                            }
                        }

                        if (theBar != null)
                        {
                            InteractionInstance instance = TendHomeBar.Singleton.CreateInstance(theBar, sim,
                                new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior), false, false);
                            sim.InteractionQueue.AddAfterCheckingForDuplicates(instance);

                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void LetSimIn(Sim s)
        {
            VisitSituation visitSituation = VisitSituation.FindVisitSituationInvolvingGuest(s);
            if (visitSituation != null)
            {
                VisitSituation.SetVisitToGreeted(s);
                VisitSituation.OnInvitedIn(s);
                visitSituation.AllowedToStayOver = true;
                visitSituation.SetStateSocializing();
            }
        }

        private bool isBarManned(BarProfessional bar)
        {
            if (bar.InUse)
            {
                List<Sim> users = bar.ActorsUsingMe;
                foreach (Sim user in users)
                {
                    if (user == bar.GetBartender())
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        private void PushSimToDrink(Actors.Sim sim, IBarProfessional bar, String bestDrinkName)
        {

            List<InteractionObjectPair> interactions = bar.GetAllInteractionsForActor(sim);
            InteractionDefinition drinkingDefinition = null;
            foreach (InteractionObjectPair interaction in interactions)
            {
                InteractionDefinition interactionDef = interaction.InteractionDefinition;
                string name = interactionDef.GetInteractionName(sim, bar, interaction);

                if (name != null && name.Contains(bestDrinkName))
                {
                    drinkingDefinition = interactionDef;
                    break;
                }
            }

            if (drinkingDefinition != null)
            {

                IEnumerable<InteractionInstance> actions = sim.InteractionQueue.InteractionList;
                List<InteractionInstance> toCancel = new List<InteractionInstance>();
                bool alreadyDrinking = false;
                foreach (InteractionInstance action in actions)
                {
                    if (!(action is BarProfessional.BarInteraction))
                    {

                        toCancel.Add(action);
                    }
                    else
                    {

                        alreadyDrinking = true;
                        break;
                    }

                }

                foreach (InteractionInstance action in toCancel)
                {
                    sim.InteractionQueue.CancelInteraction(action, false);
                }
                if (!alreadyDrinking)
                {
                    sim.InteractionQueue.AddAfterCheckingForDuplicates(drinkingDefinition.CreateInstance(bar, sim, new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), true, false));

                }
            }
        }

        private IBarProfessional findNearestBar(Actors.Sim sim)
        {
            return GlobalFunctions.GetClosestObject<BarProfessional>(sim, false, true, new List<BarProfessional>(), null);

        }

        public void RemoveRoleGivingInteraction(Actors.Sim sim)
        {

        }

        public string RoleName(bool isFemale)
        {
            return string.Empty;
        }

        public Roles.Role.RoleType RoleType
        {
            get { return Role.RoleType.Bartender; }
        }

        public ResourceKey GetRoleUniformKey(Sim Sim, Lot.MetaAutonomyType venueType)
        {
            return ResourceKey.kInvalidResourceKey;
        }
    }
}
