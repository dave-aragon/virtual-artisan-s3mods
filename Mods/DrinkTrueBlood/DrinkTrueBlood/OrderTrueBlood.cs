using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.CelebritySystem;


namespace Misukisu.Interactions
{
    class OrderTrueBlood : BarProfessional.BarInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public override global::Sims3.Gameplay.Interactions.InteractionDefinition GetBartenderInteraction()
        {
            return ServeTrueBlood.Singleton;
        }

        public override bool RunBehavior()
        {
            PizzaBox pizzaBox = this.mDestination.Object.GetContainedObject(this.mDestination.PlacementSlot) as PizzaBox;
            if (pizzaBox != null)
            {
                this.Actor.InteractionQueue.PushAsContinuation(pizzaBox.GetServeInteractionDefinition(), pizzaBox, true);
                return true;
            }
            return false;
        }
        public override bool RunOrderBehavior()
        {
            int costForPizza = Bartending.GetCostForPizza(this.Target.LotCurrent.GetMetaAutonomyType);
            return CelebrityManager.TryModifyFundsWithCelebrityDiscount(this.Actor, this.Target, costForPizza, true);
        }

        public class Definition : InteractionDefinition<Sim, BarProfessional, OrderTrueBlood>
        {
            public override string GetInteractionName(Sim actor, BarProfessional target, InteractionObjectPair iop)
            {
                return BarProfessional.LocalizeString("OrderPizza", new object[]
			{
				Bartending.GetCostForPizza(target.LotCurrent.GetMetaAutonomyType)
			});
            }
            public override string[] GetPath(bool isFemale)
            {
                return new string[]
			{
				BarProfessional.LocalizeString("OrderFood", new object[0]), 
				BarProfessional.LocalizeString("OrderFoodGroup", new object[0])
			};
            }
            public override bool Test(Sim a, BarProfessional target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!target.IsBartenderAvailable())
                {
                    return false;
                }
                if (a == target.mBartender)
                {
                    return false;
                }
                if (isAutonomous && BarProfessional.IsRunningBarInteraction(a))
                {
                    return false;
                }
                if (!Bartending.IsPizzaAvailable(target.LotCurrent.GetMetaAutonomyType))
                {
                    return false;
                }
                if (a.FamilyFunds < Bartending.GetCostForPizza(target.LotCurrent.GetMetaAutonomyType))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(BarProfessional.LocalizeString("NotEnoughMoney", new object[0]));
                    return false;
                }
                return true;
            }
        }
    }
}
