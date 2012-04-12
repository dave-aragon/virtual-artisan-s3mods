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
using Misukisu.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.EventSystem;

namespace Sims3.Gameplay.Objects.CookingObjects.Misukisu
{
    public class TrueBlood : SnackVampireJuice, IMicrowavable, IRemovableFromFridgeAsInitialRecipeStep
    {
        public TrueBlood()
            : base()
        {
            RecipeReader.InitTrueBloodInstance(this);
        }

        

        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(Snack.Snack_Microwave.Singleton);
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
                return "BowlLarge";
            }
        }

       
    }
}
