using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Misukisu.Common;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Misukisu;

namespace Misukisu.Sims3.Gameplay.Interactions
{

    public class TuneExoticDancer : Interaction<Sim, DancersStage>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        protected override bool Run()
        {
            try
            {

                float startTime;
                float endTime;
                base.Target.GetRoleTimes(out startTime, out endTime);

                float start;
                float end;
                bool ok = ShowTimeTuningDialog(startTime, endTime, out start, out end);
                if (ok)
                {
                    base.Target.TuningChanged(start, end);
                }
            }
            catch (Exception e)
            {
                Message.ShowError(DancersStage.NAME, "Tuning failed, please try again", false, e);

            }
            return true;
        }

        private static bool ShowTimeTuningDialog(float startTime, float endTime, out float start, out float end)
        {
            bool changes = false;
            string[] values = ThreeStringInputDialog.Show("The Dancer", new string[] { "Arrives at (hour, 0-24, 0='reset')", "Leaves at (hour, 0-24, 0='reset')", "Spare field for future needs" }, new string[] { startTime.ToString(), endTime.ToString(), "" }, true);

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



        private sealed class Definition : InteractionDefinition<Sim, DancersStage, TuneExoticDancer>
        {

            protected override string GetInteractionName(Sim a, DancersStage target, InteractionObjectPair interaction)
            {
                return "Tuning...";
            }

            protected override bool Test(Sim a, DancersStage target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
