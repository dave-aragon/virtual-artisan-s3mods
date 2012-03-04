using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;

using Sims3.Gameplay.Roles.Misukisu;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Misukisu;
using Misukisu.Sims3.Gameplay.Interactions.Paintedlady;
using Sims3.UI;
using Misukisu.Common;


namespace Misukisu.Sims3.Gameplay.Interactions.Paintedlady
{

    public class TakeMistress : Interaction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            Courtesan role = Courtesan.AssignedRole(base.Target);
            if (role != null)
            {
                CourtesansPerfume perfume = role.GetPerfume();

                perfume.SlaveOwner = this.Actor;
                Message.Sender.Show(base.Target, "Sure, I'll go right away to your place");
                role.MakeSimComeToRoleLot();
            }

            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, CourtesansPerfume, TakeMistress>
        {

            public override string GetInteractionName(Sim a, CourtesansPerfume target, InteractionObjectPair interaction)
            {
                string bottleOwner = "Courtesan/Courter";
                if (target.CurrentRole != null)
                {
                    Sim sim = target.CurrentRole.SimInRole;
                    if (sim != null)
                    {
                        bottleOwner = sim.FullName;
                    }
                }


                return "Ask " + bottleOwner + " to Your House (non-revertable, experimental, §500)";
            }

            public override bool Test(Sim a, CourtesansPerfume target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!isAutonomous && target.CurrentRole != null && target.CurrentRole.SimInRole != null)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
