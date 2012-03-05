using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Misukisu.Drunkard;
using Sims3.Gameplay.Roles.Misukisu;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Misukisu;

namespace Misukisu.Sims3.Gameplay.Interactions.Drunkard
{

    public class TakeDrunkardHome : Interaction<Sim, DrunkardsBottle>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
                if (base.Target.CurrentRole != null)
                {
                    //base.Target.SlaveOwner = this.Actor;
                    Message.Sender.Show("Thanks for taking care of me. Tomorrow I'll come to your place instead");
                }
          
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, DrunkardsBottle, TakeDrunkardHome>
        {

            public override string GetInteractionName(Sim a, DrunkardsBottle target, InteractionObjectPair interaction)
            {
                string bottleOwner = "Bottle Owner";
                if (target.CurrentRole != null)
                {
                    Sim sim = target.CurrentRole.SimInRole;
                    if (sim != null)
                    {
                        bottleOwner = sim.FullName;
                    }
                }
                

                return "Ask " + bottleOwner + " to Hang Around at Your Place (non-revertable, experimental)";
            }

            public override bool Test(Sim a, DrunkardsBottle target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
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
