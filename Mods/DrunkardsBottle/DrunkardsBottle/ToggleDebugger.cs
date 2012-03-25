using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;

namespace Misukisu.Drunkard
{


    public class ToggleDebugger : ImmediateInteraction<IActor, IGameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
            if (Message.Sender.IsDebugging())
            {
                Message.Sender.getDebugger().EndDebugLog();
                Message.Sender.setDebugger(null);
            }
            else
            {
                Message.Sender.setDebugger(new Debugger(base.Target));
            }

            return true;
        }

        private sealed class Definition : ActorlessInteractionDefinition<IActor, IGameObject, ToggleDebugger>
        {

            public override string GetInteractionName(IActor a, IGameObject target, InteractionObjectPair interaction)
            {
                if (Message.Sender.IsDebugging())
                {
                    return I18n.Localize(CommonTexts.DEBUG_STOP,"Stop Debugger");
                }
                else
                {
                    return I18n.Localize(CommonTexts.DEBUG_START, "Start Debugger");
                }

            }

            public override bool Test(IActor actor, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
