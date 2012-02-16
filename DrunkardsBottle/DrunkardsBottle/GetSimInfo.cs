using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Misukisu;
using Sims3.Gameplay.Roles.Misukisu;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.UI;

namespace Sims3.Gameplay.Objects.Misukisu
{

    public class GetInfo : Interaction<Sim, DrunkardsBottle>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        protected override bool Run()
        {
            try
            {
                if (base.Target.CurrentRole != null)
                {
                    Sim sim = base.Target.CurrentRole.SimInRole;
                    StringBuilder s= new StringBuilder();
                    IEnumerable<InteractionInstance> actions=sim.InteractionQueue.InteractionList;
                    foreach( InteractionInstance action in actions){
                        s.Append(action.GetInteractionName());
                            s.Append("\n");
                    }
                    base.Target.PushRoleStartingInteraction(sim);

                    Message.Show(sim.FullName + " has actions " + s.ToString());
                }
                else
                {
                    Message.Show("Currentrole is null");
                }

            }
            catch (Exception e)
            {
                Message.Show("Error: " + e.Message );
               
            }
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, DrunkardsBottle, GetInfo>
        {
            
            protected override string GetInteractionName(Sim a, DrunkardsBottle target, InteractionObjectPair interaction)
            {
                return "Show Sim Info";
            }

            protected override bool Test(Sim a, DrunkardsBottle target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
