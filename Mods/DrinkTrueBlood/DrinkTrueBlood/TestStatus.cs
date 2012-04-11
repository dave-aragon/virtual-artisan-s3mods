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
using Sims3.Gameplay.ActorSystems;

namespace Misukisu.DrinkTrueBlood
{
    public class TestStatus : Interaction<Sim, TrueBlood>
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {

            //CheckRecipeStatus();
            MicrowaveBlood();
            return true;
        }

        private void MicrowaveBlood()
        {
            Recipe recipe = Recipe.NameToRecipeHash["TrueBlood"];
            Target.CookingProcess = createCookingProcess(recipe);
            Target.InitializeForRecipe(recipe);
            CookingProcess.MoveToNextStep(Target, Actor);
            CarrySystem.EnterWhileHolding(this.Actor, Target, false);
            InteractionDefinition followup = (Target as IRemovableFromFridgeAsInitialRecipeStep).FollowupInteraction;
            Debugger log = new Debugger("MicrowaveTest");
            log.Debug(this,"Interaction is:  "+followup.GetType().FullName);

            log.Debug(this,"Target Interaction test is: "+Target.CookingProcess.InteractionTest(Actor, Target));


           log.Debug(this,"Interaction test is: "+ Microwave.InteractionTestForPutInMicrowave(Actor, Target));
           log.Debug(this, "Trying to push item to micro: "+Microwave.InteractionBodyForPutInMicrowave(this));
            //this.Actor.InteractionQueue.PushAsContinuation(followup, Target, true);
             //log.Debug(this,"Action pushed to sim");
            log.EndDebugLog();
        }

        private void CheckRecipeStatus()
        {
            Debugger log = new Debugger("RecipeTest");
            Recipe recipe = Recipe.NameToRecipeHash["TrueBlood"];
            CookingProcessData data = recipe.CookingProcessData;
            log.Debug(this, "Cooking process is: " + data.ToString());
            log.Debug(this, "Micro usage is: " + data.UsesAMicrowave.ToString());
            log.Debug(this, "Actor is: " + Actor.ToString());

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
                    Target, Actor);
            return process;
        }

        private sealed class Definition : InteractionDefinition<Sim, TrueBlood, TestStatus>
        {

            public override string GetInteractionName(Sim a, TrueBlood target, InteractionObjectPair interaction)
            {
                return "Check status";
            }

           

            public override bool Test(Sim actor, TrueBlood target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
        }
    }
}
