using System;
using System.Collections.Generic;
using System.Text;
using Misukisu.DrinkTrueBlood;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Actors;

namespace Sims3.Gameplay.Objects.CookingObjects.Misukisu
{
    public class TrueBlood : SnackVampireJuice, IMicrowavable
    {
        //private Debugger debugger;
        public TrueBlood()
            : base()
        {
            RecipeReader.InitTrueBloodInstance(this);
            //debugger = new Debugger(this);
            //debugger.Debug(this, "Instance of true blood created");

            //debugger.EndDebugLog();
        }

        

        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(Snack.Snack_Microwave.Singleton);
            AddInteraction(TestStatus.Singleton);
        }

        public override bool CanBeCleanedUp
        {
            get
            {
                return LotCurrent.IsResidentialLot;
            }
        }

        public string ActorNameForMicrowave
        {
            get
            {
                return "Plate";
            }
        }
    }
}
