using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Misukisu.Drunkard;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Misukisu;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;

namespace Misukisu.Sims3.Gameplay.Interactions.Drunkard
{

    public class TuneDrunkard : ImmediateInteraction<IActor, DrunkardsBottle>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            try
            {
                //DrunkardsBottle.Owner ownerType = base.Target.OwnerType;
                //DrunkardsBottle.Owner newOwnerType = ShowOwnerTuningDialog(ownerType);

                float startTime;
                float endTime;
                base.Target.GetRoleTimes(out startTime, out endTime);

                float start;
                float end;
                bool ok = ShowTimeTuningDialog(startTime, endTime, out start, out end);
                if ( ok)
                {
                    base.Target.TuningChanged( start, end);
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(base.Target, "Tuning failed, please try again", false, e);
            }
            return true;
        }

        private static bool ShowTimeTuningDialog(float startTime, float endTime, out float start, out float end)
        {
            bool changes = false;
            string[] values = ThreeStringInputDialog.Show("The owner of the bottle", new string[] { "Arrives at (hour, 0-24, 0='don't come')", "Leaves at (hour, 0-24, 0='don't come')", "Spare field for future needs" }, new string[] { startTime.ToString(), endTime.ToString(), "" }, true);

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

        //private static DrunkardsBottle.Owner ShowOwnerTuningDialog(DrunkardsBottle.Owner ownerType)
        //{
        //    Dictionary<string, object> regTypes = new Dictionary<string, object>();
        //    regTypes.Add(DrunkardsBottle.Owner.Hangaround.ToString(), DrunkardsBottle.Owner.Hangaround.ToString());
        //    regTypes.Add(DrunkardsBottle.Owner.Tippler.ToString(), DrunkardsBottle.Owner.Tippler.ToString());
        //    string title = Localization.LocalizeString("Misukisu.Drunkard.Tuning.TypeDialog:Title", new object[0]);
        //    object result = ComboSelectionDialog.Show("The owner of the bottle is a", regTypes, ownerType.ToString());
        //    DrunkardsBottle.Owner newOwnerType = ownerType;

        //    string userSelection = result as string;
        //    if (userSelection != null)
        //    {
        //        newOwnerType = (DrunkardsBottle.Owner)Enum.Parse(typeof(DrunkardsBottle.Owner), userSelection, true);
        //    }
        //    return newOwnerType;
        //}

        private sealed class Definition : ActorlessInteractionDefinition<IActor, DrunkardsBottle, TuneDrunkard>
        {

            public override string GetInteractionName(IActor a, DrunkardsBottle target, InteractionObjectPair interaction)
            {
                return "Tuning Dialog...";
            }

            public override bool Test(IActor actor, DrunkardsBottle target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
