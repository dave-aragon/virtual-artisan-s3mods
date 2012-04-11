using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.CookingObjects.Misukisu;

namespace Misukisu.DrinkTrueBlood
{
    public class TestStatus : ImmediateInteraction<Sim, TrueBlood>
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {

            CheckRecipeStatus();
            return true;
        }

        private void CheckRecipeStatus()
        {
            Debugger log = new Debugger("RecipeTest");
            Recipe recipe = Recipe.NameToRecipeHash["TrueBlood"];
            CookingProcessData data = recipe.CookingProcessData;
            log.Debug(this, "Cooking process is: " + data.ToString());
            log.Debug(this, "Micro usage is: " + data.UsesAMicrowave.ToString());


            CookingProcess process = createCookingProcess(recipe);
            log.Debug(this, "This should show");
            log.Debug(this, "Cooking process created " + process.ToString());
            List<CookingStep> steps = process.RemainingCookingProcess;
            StringBuilder sb = new StringBuilder();
            foreach (CookingStep step in steps)
            {
                sb.Append(step.ToString() + " --- ");

            }
            log.Debug(this, "Cooking process is: " + sb.ToString());
            log.EndDebugLog();

        }

        private CookingProcess createCookingProcess(Recipe recipe)
        {
            CookingProcess process =
                new CookingProcess(recipe, new List<Ingredient>(), null, Actor.LotCurrent, Recipe.MealDestination.SurfaceOrEat,
                    Recipe.MealQuantity.Single, Recipe.MealRepetition.MakeOne, "Have", new String[] { "Tadaa" },
                    Target, base.Actor);
            return process;
        }

        private sealed class Definition : ActorlessInteractionDefinition<IActor, IGameObject, TestStatus>
        {

            public override string GetInteractionName(IActor a, IGameObject target, InteractionObjectPair interaction)
            {
                return "Check status";
            }

            public override bool Test(IActor actor, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
