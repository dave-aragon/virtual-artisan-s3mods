using System;
using System.Collections.Generic;

using System.Text;
using Misukisu.Common;
using System.Diagnostics;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Actors;

namespace Sims3.Gameplay.Roles.Misukisu
{
    public class CustomRole:Pianist
    {
        public CustomRole()
            : base()
        { }

        public CustomRole(RoleData data, SimDescription s, IRoleGiver roleGiver): base(data, s, roleGiver)
        { }

        public new void RemoveSimFromRole()
        {
            Message.Show("Sim is removed from role " + new StackTrace().ToString());
            base.RemoveSimFromRole();
        }

        public static CustomRole clone(Role toClone, Sim simInRole)
        {
            SimDescription actor = null;
            List<SimDescription> townies = Household.AllTownieSimDescriptions();
            foreach (SimDescription townie in townies)
            {
                if (simInRole == townie.CreatedSim)
                {
                    actor = townie;
                    Message.Show("Clone found the actor");
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
                        Message.Show("Clone found the actor");
                        break;
                    }
                }
            }

            CustomRole newRole = null;
            if (actor != null)
            {
                newRole = new CustomRole(toClone.Data, actor, toClone.RoleGivingObject);
                newRole.StartRole();
            }
            return newRole;
        }

        protected override void StartRole()
        {
            Message.Show("Custom role starting " + new StackTrace().ToString());
            base.StartRole();
        }

        public override void SimulateRole(float minPassed)
        {
            //Message.Show("Custom role in simulation " + new StackTrace().ToString());
            base.SimulateRole(minPassed);
        }

        protected override void EndRole()
        {
            Message.Show("Custom role ending " + new StackTrace().ToString());
            base.EndRole();
        }
    }
}
