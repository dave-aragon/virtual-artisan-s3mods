using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.UI.Controller;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Misukisu;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Roles.Misukisu;


namespace Misukisu.Sims3.Gameplay.Interactions.Paintedlady
{
    public class AfterWooHooCleanup : Interaction<Sim, Sim>
    {
        // Fields
        public static readonly Definition Singleton = new Definition();
        
       
        protected override bool Run()
        {
            try
            {
                Lot.MetaAutonomyType venueType =  base.Actor.LotCurrent.GetMetaAutonomyType;
                Courtesan.SwitchToProperClothing(base.Actor, venueType);
            }
            catch (Exception e)
            {
                Message.ShowError(CourtesansPerfume.NAME, "Cannot restore clothes", false, e);
            }
            return true;
        }

        public sealed class Definition : SoloSimInteractionDefinition<AfterWooHooCleanup>
        {
            
            protected override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Finish the deal";
            }            

            protected override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                
                if (a.OccultManager.DisallowClothesChange())
                {
                    return false;
                }
                return true;
            }
        }
    }



}