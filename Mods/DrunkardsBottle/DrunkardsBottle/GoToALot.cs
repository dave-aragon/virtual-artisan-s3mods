﻿using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Roles;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Misukisu;
using Misukisu.Drunkard;
using Sims3.Gameplay.Situations;

namespace Misukisu.Sims3.Gameplay.Interactions.Drunkard
{
    class GoToALot : Interaction<Sim, Sim>, IRouteFromInventoryOrSelfWithoutCarrying
    {
        // Fields
        public static readonly ISoloInteractionDefinition Singleton = new Definition();

        // Methods
        public override bool Run()
        {
            try
            {
                Role assignedRole = base.Actor.SimDescription.AssignedRole;
                if ((assignedRole != null) && assignedRole.IsActive)
                {
                    Lot target = null;
                    DrunkardsBottle bottle = assignedRole.RoleGivingObject as DrunkardsBottle;
                    if (bottle != null)
                    {
                        target = bottle.GetTargetLot();
                    }

                    if (target != null)
                    {

                        InteractionInstance instance = null;
                        if (target.IsCommunityLot)
                        {
                            instance = VisitCommunityLot.Singleton
                            .CreateInstance(target, base.Actor, base.GetPriority(), false, false);
                        }
                        else
                        {
                            instance = GoToLot.Singleton
                              .CreateInstance(target, base.Actor, base.GetPriority(), false, false);
                            // Greet to get inside, visitsituation to really do something inside 
                           
                            if (VisitSituation.FindVisitSituationInvolvingGuest(base.Actor) == null)
                            {
                                VisitSituation.Create(base.Actor, target);
                            }
                            base.Actor.GreetSimOnLot(target);
                        }

                        if (base.TryPushAsContinuation(instance))
                        {
                            assignedRole.UpdateFulfillingLot(target.LotId);
                            return true;
                        }
                    }
                    assignedRole.UpdateFulfillingLot(0L);

                }
            }
            catch (Exception ex)
            {
                Message.Sender.ShowError(base.Actor, "Cannot take role sims to role lot", false, ex);
            }
            return false;
        }

        // Nested Types
        private sealed class Definition : SoloSimInteractionDefinition<GoToALot>
        {
            // Methods
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a.SimDescription.HasActiveRole)
                {
                    return true;
                }
                return false;
            }
        }
    }



}
