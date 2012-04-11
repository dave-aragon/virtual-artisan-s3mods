using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.CookingObjects.Misukisu;
using Sims3.Gameplay;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Objects.FoodObjects;

namespace Misukisu.Interactions
{
    class ServeTrueBlood : BarProfessional.BartenderInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool RunMakeBehavior()
        {
            Recipe recipe = Recipe.NameToRecipeHash["TrueBlood"];
            TrueBlood tb = GlobalFunctions.CreateObjectOutOfWorld(recipe.ObjectToCreateInFridge, recipe.CodeVersion) as TrueBlood;
           tb.ParentToSlot(this.Actor, Sim.ContainmentSlots.RightHand);
            CarrySystem.EnterWhileHolding(this.Actor, tb);
            return true;
        }

        public class Definition : InteractionDefinition<Sim, BarProfessional, ServeTrueBlood>
        {
            public override bool Test(Sim actor, BarProfessional target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
