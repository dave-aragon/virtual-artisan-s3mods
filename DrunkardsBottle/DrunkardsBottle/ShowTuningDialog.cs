using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Misukisu;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;
using Sims3.SimIFace;

namespace Sims3.Gameplay.Objects.Misukisu
{

    public class ShowTuningDialog : Interaction<Sim, DrunkardsBottle>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        protected override bool Run()
        {
            try
            {
               DrunkardsBottle.Owner ownerType =base.Target.OwnerType;
                Dictionary<string, object> regTypes = new Dictionary<string, object>();
                regTypes.Add("Hangaround", DrunkardsBottle.Owner.Hangaround);
                regTypes.Add("Civilized Drinker", DrunkardsBottle.Owner.CivilizedDrinker);
                regTypes.Add("Drunkard", DrunkardsBottle.Owner.Drunkard);

                object result = ComboSelectionDialog.Show("The owner of this bottle is a", regTypes, ownerType);
                if (result is DrunkardsBottle.Owner) {
                    ownerType = (DrunkardsBottle.Owner)result;
                }
                
                float startTime;
                float endTime;
                base.Target.GetRoleTimes(out startTime, out endTime);

                string[] values = ThreeStringInputDialog.Show("The owner of the bottle", new string[] { "Arrives at (hour, 0-24, 0='don't come')", "Leaves at (hour, 0-24, 0='don't come')", "This is just for testing, don't touch " }, new string[] { startTime.ToString(), endTime.ToString(), "12" }, true);
              // List<string> values =TwoStringInputDialog.Show("The owner of this bottle","Arrives at:", "Leaves at:",startTime.ToString(),endTime.ToString(),"OK","Cancel");
             
               float start = float.Parse(values[0]);
               float end = float.Parse(values[1]);
               base.Target.TuningChanged(ownerType,start,end);
            }
            catch (Exception e)
            {
                Message.Show("Virtual Artisan Error: " + e.Message + " - " + e.StackTrace);

            }
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, DrunkardsBottle, ShowTuningDialog>
        {

            protected override string GetInteractionName(Sim a, DrunkardsBottle target, InteractionObjectPair interaction)
            {
                return "Tuning...";
            }

            protected override bool Test(Sim a, DrunkardsBottle target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
