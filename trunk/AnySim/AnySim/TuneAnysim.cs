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
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;

namespace Misukisu.Sims3.Gameplay.Interactions.Anysim
{

    public class TuneAnysim : ImmediateInteraction<IActor, AnysimObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            try
            {
                float startTime;
                float endTime;
                base.Target.GetRoleTimes(out startTime, out endTime);
                string title = base.Target.RoleName(false);
                float newStartTime;
                float newEndTime;
                string newTitle;
                bool ok = ShowTuningDialog(startTime, endTime, title, out newStartTime, out newEndTime, out newTitle);
                if (ok)
                {
                    base.Target.TuningChanged(newStartTime, newEndTime, newTitle);
                }
            }
            catch (Exception e)
            {
                // TODO: maybe localize?
                Message.Sender.ShowError(Texts.NAME, "Tuning failed, please try again", false, e);
            }
            return true;
        }

        private static bool ShowTuningDialog(float startTime, float endTime, string roleTitle, out float newStartTime, out float newEndTime, out string newRoleTitle)
        {
            bool changes = false;
            string dialogTitle = Localization.LocalizeString(Texts.TUNING_DIALOG_TITLE, new string[0]);
            string[] fieldNames = new string[] 
            {
                Localization.LocalizeString(Texts.TUNING_DIALOG_ROLE_START_TIME, new string[0]),
                Localization.LocalizeString(Texts.TUNING_DIALOG_ROLE_END_TIME, new string[0]),
                Localization.LocalizeString(Texts.TUNING_DIALOG_ROLE_TOOLTIP, new string[0])
            };

            string[] values = ThreeStringInputDialog.Show(dialogTitle, fieldNames, 
                new string[] { startTime.ToString(), endTime.ToString(), roleTitle }, false);

            if (!string.IsNullOrEmpty(values[0]))
            {
                newStartTime = float.Parse(values[0]);
                changes = true;
            }
            else
            {
                newStartTime = startTime;
            }

            if (!string.IsNullOrEmpty(values[1]))
            {
                newEndTime = float.Parse(values[1]);
                changes = true;
            }
            else
            {
                newEndTime = endTime;
            }

            if (!string.IsNullOrEmpty(values[2]))
            {
                newRoleTitle = values[2].ToString();
                changes = true;
            }
            else
            {
                newRoleTitle = roleTitle;
            }
            return changes;
        }


        private sealed class Definition : ActorlessInteractionDefinition<IActor, AnysimObject, TuneAnysim>
        {

            public override string GetInteractionName(IActor a, AnysimObject target, InteractionObjectPair interaction)
            {
                return Localization.LocalizeString(Texts.TUNING_DIALOG_INTERACTION, new string[0]); ;
            }

            public override bool Test(IActor actor, AnysimObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
