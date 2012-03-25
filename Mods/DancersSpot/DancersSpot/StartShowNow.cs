using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Misukisu;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Interfaces;
using Misukisu.Dancer;

namespace Misukisu.Sims3.Gameplay.Interactions
{

    public class StartShowNow : ImmediateInteraction<IActor, DancersStage>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            try
            {
                if (base.Target.CurrentRole != null)
                {
                    base.Target.PushSimToPerformShow(base.Target.CurrentRole.SimInRole);
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(base.Target, "Cannot start show now", false, e);

            }
            return true;
        }

        private sealed class Definition : ActorlessInteractionDefinition<IActor, DancersStage, StartShowNow>
        {

            public override string GetInteractionName(IActor a, DancersStage target, InteractionObjectPair interaction)
            {
                return "Start Extra Show Now";
            }

            public override bool Test(IActor actor, DancersStage target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
