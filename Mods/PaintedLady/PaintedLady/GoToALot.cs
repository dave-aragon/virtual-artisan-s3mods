using System;
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
using Misukisu.Sims3.Gameplay.Interactions.Paintedlady;
using Sims3.Gameplay.Situations;
using Misukisu.Paintedlady;

namespace Misukisu.Sims3.Gameplay.Interactions.Paintedlady
{
    class GoToALot : Interaction<Sim, Sim>, IRouteFromInventoryOrSelfWithoutCarrying
    {
        // Fields
        public static readonly InteractionDefinition Singleton = new Definition();

        // Methods
        public override bool Run()
        {
            try
            {
                Role assignedRole = base.Actor.SimDescription.AssignedRole;
                if ((assignedRole != null) && assignedRole.IsActive)
                {
                    Lot target = null;
                    GameObject roleGivingObject = assignedRole.RoleGivingObject as GameObject;
                    if (roleGivingObject != null)
                    {
                        target = roleGivingObject.LotCurrent;
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
                
            }
            return false;
        }



        // Nested Types
        private sealed class Definition : InteractionDefinition<Sim, Sim, GoToALot>
        {
            // Methods
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
               
                if (isAutonomous)
                {
                    return false;
                }
                Role assignedRole = a.SimDescription.AssignedRole;
                if (a.SimDescription.HasActiveRole && assignedRole.RoleGivingObject != null && a.LotCurrent != assignedRole.RoleGivingObject.LotCurrent)
                {
                  
                    return true;
                }
                return false;
            }
        }
    }



}
