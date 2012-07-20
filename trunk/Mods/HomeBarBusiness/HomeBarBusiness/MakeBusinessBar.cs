using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace.RouteDestinations;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay;

using Sims3.Gameplay.TuningValues;

namespace Misukisu.HomeBarBusiness
{

    public class MakeBusinessBar : Interaction<Sim, BarProfessional>
    {

        public class Definition : InteractionDefinition<Sim, BarProfessional, MakeBusinessBar>, IAllowedOnClosedVenues
        {

            public override string GetInteractionName(Sim actor, BarProfessional target, InteractionObjectPair iop)
            {
                bool enabled = AreBarInteractionsEnabled(target);
                if (enabled)
                {
                    return "Set As Business Bar (No Pay - No Drinks)";
                }
                else
                {
                    return "Set As Home Bar (No Pay for Drinks)";
                }
            }


            public override bool Test(Sim a, BarProfessional target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous)
                {
                    return false;
                }

                if (target.LotCurrent.IsCommunityLot)
                {
                    return false;
                }
                return true;

            }
        }

        public static InteractionDefinition Singleton = new MakeBusinessBar.Definition();

        public static bool AreBarInteractionsEnabled(BarProfessional target)
        {
            bool enabled = false;
            foreach (InteractionObjectPair current in target.Interactions)
            {
                //ActivateBusiness.debugger.Debug("this", "Interaction is " + current.InteractionDefinition.GetType().FullName);

                if (current.InteractionDefinition is BarProfessional.Practice.Definition)
                {
                    enabled = true;
                    break;
                }
            }
            return enabled;
        }

        public override bool Run()
        {
            BarProfessional bar = this.Target;

            if (AreBarInteractionsEnabled(bar))
            {
                bar.RemoveInteractionByType(BarProfessional.ServeDrinks.SingletonSingle);
                bar.RemoveInteractionByType(BarProfessional.ServeDrinks.SingletonMultiple);
                bar.RemoveInteractionByType(BarProfessional.ServeNectar.Singleton);
                bar.RemoveInteractionByType(BarProfessional.Practice.Singleton);
                bar.RemoveInteractionByType(BarProfessional.PracticeWith.Singleton);
                bar.RemoveInteractionByType(BarProfessional.PracticeMakingNewDrinks.Singleton);
                bar.RemoveInteractionByType(BarProfessional.OrderFreeDrink.Singleton);
                bar.RemoveInteractionByType(BarProfessional.OrderDrink.Singleton);
                bar.RemoveInteractionByType(BarProfessional.OrderDrinkOnResidentalLot.Singleton);
                bar.RemoveInteractionByType(BarProfessional.OrderFood.Singleton);
                bar.RemoveInteractionByType(BarProfessional.OrderBirthdayCake.Singleton);
                bar.RemoveInteractionByType(BarProfessional.OrderPizza.Singleton);
                bar.RemoveInteractionByType(BarProfessional.OrderRound.Singleton);
                bar.RemoveInteractionByType(BarProfessional.ServeDrinksOnTheHouse.Singleton);
                bar.RemoveInteractionByType(BarProfessional.SelectIngredients.Singleton);
                bar.RemoveInteractionByType(BarProfessional.SelectIngredientsToggle.Singleton);
                bar.RemoveInteractionByType(BarProfessional.PlaceSnackBowl.Singleton);
            }
            else
            {
                EnableBarInteractions(bar);
            }
            return true;
        }

        public static void EnableBarInteractions(BarProfessional bar)
        {
            if (!AreBarInteractionsEnabled(bar))
            {
                bar.AddInteraction(BarProfessional.ServeDrinks.SingletonSingle);
                bar.AddInteraction(BarProfessional.ServeDrinks.SingletonMultiple);
                bar.AddInteraction(BarProfessional.ServeNectar.Singleton);
                bar.AddInteraction(BarProfessional.Practice.Singleton);
                bar.AddInteraction(BarProfessional.PracticeWith.Singleton);
                bar.AddInteraction(BarProfessional.PracticeMakingNewDrinks.Singleton);
                bar.AddInteraction(BarProfessional.OrderFreeDrink.Singleton);
                bar.AddInteraction(BarProfessional.OrderDrink.Singleton);
                bar.AddInteraction(BarProfessional.OrderDrinkOnResidentalLot.Singleton);
                bar.AddInteraction(BarProfessional.OrderFood.Singleton);
                bar.AddInteraction(BarProfessional.OrderBirthdayCake.Singleton);
                bar.AddInteraction(BarProfessional.OrderPizza.Singleton);
                bar.AddInteraction(BarProfessional.OrderRound.Singleton);
                bar.AddInteraction(BarProfessional.ServeDrinksOnTheHouse.Singleton);
                bar.AddInteraction(BarProfessional.SelectIngredients.Singleton);
                bar.AddInteraction(BarProfessional.SelectIngredientsToggle.Singleton);
                bar.AddInteraction(BarProfessional.PlaceSnackBowl.Singleton);
            }
        }
    }




}

