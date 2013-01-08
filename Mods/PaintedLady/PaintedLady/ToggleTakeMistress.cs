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
using Misukisu.Paintedlady;


namespace Misukisu.Sims3.Gameplay.Interactions.Paintedlady
{

    public class ToggleTakeMistress : Interaction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            Courtesan role = Courtesan.AssignedRole(base.Target);
            if (role != null)
            {
                CourtesansPerfume perfume = role.GetPerfume();
                Sim owner = perfume.Pimp;
                if (owner == null)
                {
                    perfume.Pimp = this.Actor;
                    Message.Sender.Show(base.Target, "I have obligations today, but I'll come to you tomorrow");
                }
                else if (Actor == owner)
                {
                    perfume.Pimp = null;
                }
                else
                {
                    Message.Sender.Show(base.Target, "Only my employer can fire me!");
                }
            }

            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, ToggleTakeMistress>
        {

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
            {
                string price = "0";
                Sim owner = null;
                CourtesansPerfume perfume = Courtesan.GetPerfume(target);
                if (perfume != null)
                {
                    price = perfume.PimpShare.ToString();

                    owner = perfume.Pimp;
                }

                string name = "Ask for personal fulltime service(§" + price + ")";
                if (owner == actor)
                {
                    name = "Fire from your service";
                }
                else if (owner != null)
                {
                    name = "Ask to stop working for " + owner.FullName;
                }
                return name;


            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                bool result = false;
                if (!isAutonomous)
                {
                    CourtesansPerfume perfume = Courtesan.GetPerfume(target);
                    if (perfume != null)
                    {
                        if (perfume.Pimp == null)
                        {
                            InteractionDefinition interaction = BuyWooHoo.Singleton;
                            result = Courtesan.IsTalkingTo(actor, target, result);

                            if (actor.InteractionQueue.HasInteractionOfTypeAndTarget(ToggleTakeMistress.Singleton, target))
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }

                return result;
            }
        }

    }
}
