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
using Misukisu.Paintedlady;

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

                float newStartTime;
                float newEndTime;
         
                bool ok = ShowTimeTuningDialog(startTime, endTime, out newStartTime, out newEndTime);
                if (ok)
                {
                    base.Target.TuningChanged(newStartTime, newEndTime);
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(base.Target, "Tuning failed, please try again", false, e);

            }
            return true;
        }

        private bool ShowTimeTuningDialog(float startTime, float endTime,
                out float newStartTime, out float newEndTime)
        {
            bool changes = false;
            List<String> values = TwoStringInputDialog.Show(
           "The Courtesan", "Arrives at (hour, 0-24, 0='reset'):", "Leaves at (hour, 0-24, 0='reset'):", startTime.ToString(), endTime.ToString(), "OK", "Cancel");


            if (!string.IsNullOrEmpty(values[0]))
            {
                newStartTime = float.Parse(values[0]);
                changes = true;
            }
            else
            {
                newStartTime = 0;
            }

            if (!string.IsNullOrEmpty(values[1]))
            {
                newEndTime = float.Parse(values[1]);
                changes = true;
            }
            else
            {
                newEndTime = 0;
            }

            return changes;
        }


        private sealed class Definition : ActorlessInteractionDefinition<IActor, CourtesansPerfume, TuneCourtesan>
        {

            public override string GetInteractionName(IActor a, CourtesansPerfume target, InteractionObjectPair interaction)
            {
                string start = target.HourToNiceName(target.mStartTime);
                string end = target.HourToNiceName(target.mEndTime);
                return "Set Role Start and End Time, currently " + start + " - " + end;
            }

            public override bool Test(IActor actor, CourtesansPerfume target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
