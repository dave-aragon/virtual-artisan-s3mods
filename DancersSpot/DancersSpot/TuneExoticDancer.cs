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

    public class TuneExoticDancer : ImmediateInteraction<IActor, DancersStage>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            try
            {

                OutfitCategories firstOutfit = base.Target.GetFirstOutfit();
                OutfitCategories newFirstOutfit = ShowFirstOutfitTuningDialog(firstOutfit);
                OutfitCategories lastOutfit = base.Target.GetLastOutfit();
                OutfitCategories newLastOutfit = ShowLastOutfitTuningDialog(lastOutfit);
                //Message.Sender.Show("Old outfits were: " + firstOutfit + " - " + lastOutfit);

                float showDuration = base.Target.ShowDurationMins;
                float[] showTimes = base.Target.ShowTimes;
                float newShowDuration;
                float[] newShowTimes;

                bool ok = ShowTimeTuningDialog(showDuration, showTimes, out newShowDuration, out newShowTimes);
                if (ok || firstOutfit != newFirstOutfit || lastOutfit != newLastOutfit)
                {
                    base.Target.TuningChanged(newShowTimes, newShowDuration, newFirstOutfit, newLastOutfit);
                }
            }
            catch (Exception e)
            {
                Message.Sender.ShowError(base.Target, "Tuning failed, please try again", false, e);

            }
            return true;
        }

        private static OutfitCategories ShowFirstOutfitTuningDialog(OutfitCategories outfit)
        {
            Dictionary<string, object> regTypes = new Dictionary<string, object>();
            regTypes.Add(OutfitCategories.Career.ToString(), OutfitCategories.Career.ToString());
            regTypes.Add(OutfitCategories.Everyday.ToString(), OutfitCategories.Everyday.ToString());
            regTypes.Add(OutfitCategories.Formalwear.ToString(), OutfitCategories.Formalwear.ToString());
            regTypes.Add(OutfitCategories.Sleepwear.ToString(), OutfitCategories.Sleepwear.ToString());
            regTypes.Add(OutfitCategories.Swimwear.ToString(), OutfitCategories.Swimwear.ToString());
            regTypes.Add(OutfitCategories.Athletic.ToString(), OutfitCategories.Athletic.ToString());

            object result = ComboSelectionDialog.Show("Dancer Starts In", regTypes, outfit.ToString());
            OutfitCategories newOutfit = outfit;
            if (result is string)
            {
                newOutfit = (OutfitCategories)Enum.Parse(typeof(OutfitCategories), result as string, true);
            }
            return newOutfit;
        }

        private static OutfitCategories ShowLastOutfitTuningDialog(OutfitCategories outfit)
        {
            Dictionary<string, object> regTypes = new Dictionary<string, object>();
            regTypes.Add(OutfitCategories.Career.ToString(), OutfitCategories.Career.ToString());
            regTypes.Add(OutfitCategories.Everyday.ToString(), OutfitCategories.Everyday.ToString());
            regTypes.Add(OutfitCategories.Formalwear.ToString(), OutfitCategories.Formalwear.ToString());
            regTypes.Add(OutfitCategories.Sleepwear.ToString(), OutfitCategories.Sleepwear.ToString());
            regTypes.Add(OutfitCategories.Swimwear.ToString(), OutfitCategories.Swimwear.ToString());
            regTypes.Add(OutfitCategories.Athletic.ToString(), OutfitCategories.Athletic.ToString());
            regTypes.Add(OutfitCategories.Naked.ToString(), OutfitCategories.Naked.ToString());

            object result = ComboSelectionDialog.Show("Outfit When Show Ends", regTypes, outfit.ToString());
            OutfitCategories newOutfit = outfit;
            if (result is string)
            {
                newOutfit = (OutfitCategories)Enum.Parse(typeof(OutfitCategories), result as string, true);
            }
            return newOutfit;
        }

        private static bool ShowTimeTuningDialog(float showDuration, float[] showTimes, out float newShowDuration, out float[] newShowTimes)
        {
            bool changes = false;
            string showTimesString = ShowTimesToString(showTimes, "H");

            string[] values = ThreeStringInputDialog.Show("The Dancer",
                new string[] { "Show Times Are (hour, 0-24, separate with 'H')", "Show Lasts (minutes, 0-300)", "Spare field for future needs" },
                new string[] { showTimesString, showDuration.ToString(), "" }, false);

            if (!string.IsNullOrEmpty(values[0]))
            {
                newShowTimes = StringToShowTimes(values[0]);
                changes = true;
            }
            else
            {
                newShowTimes = showTimes;
            }

            if (!string.IsNullOrEmpty(values[1]))
            {
                newShowDuration = float.Parse(values[1]);
                changes = true;
            }
            else
            {
                newShowDuration = showDuration;
            }
            return changes;
        }

        private static float[] StringToShowTimes(string value)
        {
            string[] splitted = value.Split('H');
            List<float> result = new List<float>();

            for (int i = 0; i < splitted.Length; ++i)
            {
                string time = splitted[i];
                if (!string.IsNullOrEmpty(time))
                {
                    result.Add(float.Parse(time));
                }
            }
            return result.ToArray();
        }

        private static string ShowTimesToString(float[] showTimes, string separator)
        {
            StringBuilder showTimesString = new StringBuilder();
            for (int i = 0; i < showTimes.Length; ++i)
            {
                showTimesString.Append(showTimes[i].ToString());
                showTimesString.Append(separator);
            }
            return showTimesString.ToString();
        }




        private sealed class Definition : ActorlessInteractionDefinition<IActor, DancersStage, TuneExoticDancer>
        {

            public override string GetInteractionName(IActor a, DancersStage target, InteractionObjectPair interaction)
            {
                return "Tuning Dialogs...";
            }

            public override bool Test(IActor actor, DancersStage target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
