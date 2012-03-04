using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Misukisu.Sims3.Gameplay.Interactions.Paintedlady;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Misukisu;
using Sims3.Gameplay.Interfaces;
using Misukisu.Common;

namespace Misukisu.Sims3.Gameplay.Interactions.Paintedlady
{

    public class TuneCourtesan : ImmediateInteraction<IActor, CourtesansPerfume>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            try
            {
               
                float startTime;
                float endTime;
                base.Target.GetRoleTimes(out startTime, out endTime);

                float start;
                float end;
                bool ok=ShowTimeTuningDialog(startTime, endTime, out start, out end);
                if (ok)
                {
                    base.Target.TuningChanged( start, end);
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(CourtesansPerfume.NAME, "Tuning failed, please try again", false, e);

            }
            return true;
        }

        private static bool ShowTimeTuningDialog(float startTime, float endTime, out float start, out float end)
        {
            bool changes = false;
            string[] values = ThreeStringInputDialog.Show("The owner of the perfume", new string[] { "Arrives at (hour, 0-24, 0='reset')", "Leaves at (hour, 0-24, 0='reset')", "Spare field for future needs" }, new string[] { startTime.ToString(), endTime.ToString(), "" }, true);

            if (!string.IsNullOrEmpty(values[0]))
            {
                start = float.Parse(values[0]);
                changes = true;
            }
            else
            {
                start = 0;
            }

            if (!string.IsNullOrEmpty(values[1]))
            {
                end = float.Parse(values[1]);
                changes = true;
            }
            else
            {
                end = 0;
            }
            return changes;
        }


        private sealed class Definition : ActorlessInteractionDefinition<IActor, CourtesansPerfume, TuneCourtesan>
        {

            public override string GetInteractionName(IActor a, CourtesansPerfume target, InteractionObjectPair interaction)
            {
                return "Tuning Dialog...";
            }

            public override bool Test(IActor actor, CourtesansPerfume target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
