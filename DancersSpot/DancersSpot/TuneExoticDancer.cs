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
using Sims3.SimIFace.CAS;

namespace Misukisu.Sims3.Gameplay.Interactions
{

    public class TuneExoticDancer : Interaction<Sim, DancersStage>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        protected override bool Run()
        {
            try
            {

                OutfitCategories firstOutfit = base.Target.GetFirstOutfit();
                OutfitCategories newFirstOutfit = ShowFirstOutfitTuningDialog(firstOutfit);
                OutfitCategories lastOutfit = base.Target.GetLastOutfit();
                OutfitCategories newLastOutfit = ShowLastOutfitTuningDialog(lastOutfit);


               float showDuration=base.Target.ShowDurationMins;
               float[] showTimes=base.Target.ShowTimes;
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
                Message.ShowError(DancersStage.NAME, "Tuning failed, please try again", false, e);

            }
            return true;
        }

        private static OutfitCategories ShowFirstOutfitTuningDialog(OutfitCategories outfit)
        {
            Dictionary<string, object> regTypes = new Dictionary<string, object>();
            regTypes.Add("Career Uniform", OutfitCategories.Career);
            regTypes.Add("Everyday Clothing", OutfitCategories.Everyday);
            regTypes.Add("Party Clothing", OutfitCategories.Formalwear);
            regTypes.Add("Underwear", OutfitCategories.Sleepwear);

            object result = ComboSelectionDialog.Show("Dancer Starts In", regTypes, outfit);
            OutfitCategories newOutfit = outfit;
            if (result is OutfitCategories)
            {
                newOutfit = (OutfitCategories)result;
            }
            return newOutfit;
        }

        private static OutfitCategories ShowLastOutfitTuningDialog(OutfitCategories outfit)
        {
            Dictionary<string, object> regTypes = new Dictionary<string, object>();
            regTypes.Add("In Career Uniform", OutfitCategories.Career);
            regTypes.Add("In Everyday Clothing", OutfitCategories.Everyday);
            regTypes.Add("In Party Clothing", OutfitCategories.Formalwear);
            regTypes.Add("In Underwear", OutfitCategories.Sleepwear);
            regTypes.Add("Naked", OutfitCategories.Sleepwear);

            object result = ComboSelectionDialog.Show("Dancer Ends The Show", regTypes, outfit);
            OutfitCategories newOutfit = outfit;
            if (result is OutfitCategories)
            {
                newOutfit = (OutfitCategories)result;
            }
            return newOutfit;
        }

        private static bool ShowTimeTuningDialog(float showDuration, float[] showTimes, out float newShowDuration, out float[] newShowTimes)
        {
            bool changes = false;
            string showTimesString = ShowTimesToString(showTimes, ":");
            // TODO: why only numbers allowed?
            string[] values = ThreeStringInputDialog.Show("The Dancer", 
                new string[] { "Show Times Are (hour, 0-24, separate with ':')", "Show Lasts (minutes, 0-300)", "Spare field for future needs" }, 
                new string[] { showTimesString, showDuration.ToString(), "" }, true);

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
            string[] splitted=value.Split(':'); 
            List<float> result= new List<float>();
           
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

        private static string ShowTimesToString(float[] showTimes,string separator)
        {
            StringBuilder showTimesString = new StringBuilder();
            for (int i = 0; i < showTimes.Length; ++i)
            {
                showTimesString.Append(showTimes[i].ToString());

                if (i < showTimes.Length-1)
                {
                    showTimesString.Append(separator);
                }
            }
            return showTimesString.ToString();
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
