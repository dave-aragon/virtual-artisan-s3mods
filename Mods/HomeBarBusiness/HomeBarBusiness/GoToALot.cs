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

using Sims3.Gameplay.Situations;


namespace Misukisu.HomeBarBusiness
{
    class GoToALot : Interaction<Sim, Sim>, IRouteFromInventoryOrSelfWithoutCarrying
    {
        // Fields
        public static readonly InteractionDefinition Singleton = new Definition();

        // Methods
        public override bool Run()
        {
            Debugger debugger = HomeBartender.debugger;
            debugger.Debug(base.Actor, "Trying to activate role");
            try
            {
                Role assignedRole = base.Actor.SimDescription.AssignedRole;
                if ((assignedRole != null) && assignedRole.IsActive)
                {
                    debugger.Debug(base.Actor, "Role is active");
                    Lot target = null;
                    GameObject roleGivingObject = assignedRole.RoleGivingObject as GameObject;
                    if (roleGivingObject != null)
                    {
                        target = roleGivingObject.LotCurrent;
                    }

                    if (target != null)
                    {
                        debugger.Debug(base.Actor, "Lot is found");
                        InteractionInstance instance = null;
                        if (target.IsCommunityLot)
                        {
                            instance = VisitCommunityLot.Singleton
                            .CreateInstance(target, base.Actor, base.GetPriority(), false, false);
                            debugger.Debug(base.Actor, "going to community");
                        }
                        else
                        {
                            instance = GoToLot.Singleton
                              .CreateInstance(target, base.Actor, base.GetPriority(), false, false);
                            // Greet to get inside, visitsituation to really do something inside 
                            debugger.Debug(base.Actor, "going to residential");
                            if (VisitSituation.FindVisitSituationInvolvingGuest(base.Actor) == null)
                            {
                                VisitSituation.Create(base.Actor, target);
                            }
                            base.Actor.GreetSimOnLot(target);
                        }
                        debugger.Debug(base.Actor, "instance created");
                        if (base.TryPushAsContinuation(instance))
                        {
                            assignedRole.UpdateFulfillingLot(target.LotId);
                            debugger.Debug(base.Actor, "Sim is coming! "+Actor.FullName);
                            return true;
                        }
                    }
                    assignedRole.UpdateFulfillingLot(0L);
              
                }
                
            }
            catch (Exception ex)
            {
                debugger.DebugError(base.Actor, "Cannot take role sims to role lot",  ex);
            }
            return false;
        }

        

        // Nested Types
        private sealed class Definition : InteractionDefinition<Sim,Sim,GoToALot>
        {
            // Methods
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                Debugger debugger = HomeBartender.debugger;
                debugger.Debug(a, "testing availabitity");
                if (isAutonomous)
                {
                    debugger.Debug(a, "notavailable nonautonomous");
                    return false;
                }
                Role assignedRole = a.SimDescription.AssignedRole;
                if (a.SimDescription.HasActiveRole && assignedRole.RoleGivingObject != null && a.LotCurrent != assignedRole.RoleGivingObject.LotCurrent)
                {
                    debugger.Debug(a, "is available");
                    return true;
                }
                debugger.Debug(a, "not available");
                return false;
            }
        }
    }



}
