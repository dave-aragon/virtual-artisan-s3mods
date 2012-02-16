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
               DrunkardsBottle.Owner newOwnerType = ShowOwnerTuningDialog(ownerType);
                
                float startTime;
                float endTime;
                base.Target.GetRoleTimes(out startTime, out endTime);

                float start;
                float end;
                ShowTimeTuningDialog(startTime, endTime, out start, out end);
                if (!(ownerType == newOwnerType && startTime.Equals(start) && endTime.Equals(end)))
                {
                    base.Target.TuningChanged(newOwnerType, start, end);
                }
            }
            catch (Exception e)
            {
                Message.ShowError(DrunkardsBottle.NAME, "Tuning failed, please try again",false,e);

            }
            return true;
        }

        private static void ShowTimeTuningDialog(float startTime, float endTime, out float start, out float end)
        {
            string[] values = ThreeStringInputDialog.Show("The owner of the bottle", new string[] { "Arrives at (hour, 0-24, 0='don't come')", "Leaves at (hour, 0-24, 0='don't come')", "Spare field for future needs" }, new string[] { startTime.ToString(), endTime.ToString(), "" }, true);

            start = float.Parse(values[0]);
            end = float.Parse(values[1]);
        }

        private static DrunkardsBottle.Owner ShowOwnerTuningDialog(DrunkardsBottle.Owner ownerType)
        {
            Dictionary<string, object> regTypes = new Dictionary<string, object>();
            regTypes.Add("Just likes to hang around", DrunkardsBottle.Owner.Hangaround);
            regTypes.Add("Is a civilized and responsible drinker", DrunkardsBottle.Owner.CivilizedDrinker);
            regTypes.Add("Is alcoholist", DrunkardsBottle.Owner.Drunkard);

            object result = ComboSelectionDialog.Show("The owner of the bottle", regTypes, ownerType);
            DrunkardsBottle.Owner newOwnerType = ownerType;
            if (result is DrunkardsBottle.Owner)
            {
                newOwnerType = (DrunkardsBottle.Owner)result;
            }
            return newOwnerType;
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
