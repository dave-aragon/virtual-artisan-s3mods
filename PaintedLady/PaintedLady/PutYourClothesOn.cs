using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Misukisu.Common;
using Sims3.UI.Controller;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Misukisu;

namespace Misukisu.Sims3.Gameplay.Interactions
{
    public class PutClothesOnAndRestoreRelations : Interaction<Sim, Sim>
    {
        // Fields
        public static readonly Definition Singleton = new Definition();
        
       
        protected override bool Run()
        {
            try
            {
                Message.Show("Redressing sims");
                base.Actor.SwitchToPreviousOutfitWithSpin();
            }
            catch (Exception e)
            {
                Message.ShowError(CourtesansPerfume.NAME, "Cannot restore previous clothes", false, e);
            }
            return true;
        }

        // Nested Types
        public sealed class Definition : SoloSimInteractionDefinition<PutClothesOnAndRestoreRelations>
        {
            //LongTermRelationshipTypes relationshipToRestore;

            //public new InteractionInstance CreateInstance(LongTermRelationshipTypes toRestore,IGameObject target, IActor actor, InteractionPriority priority, bool isAutonomous, bool cancellableByPlayer)
            //{
            //    InteractionInstance instance = base.CreateInstance(target,actor,priority,isAutonomous,cancellableByPlayer);
            //    this.relationshipToRestore = toRestore;
            //    return instance;
            //}

            // Methods
            protected override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Put back clothes";
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