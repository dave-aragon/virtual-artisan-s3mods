using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Abstracts;

namespace Sims3.Gameplay.Objects.Misukisu
{
    public class ServeNectar : Interaction<Sim, NectarBottle>
    {
        // Fields
        public static readonly InteractionDefinition Singleton = new Definition();
        public static readonly InteractionDefinition TasteSingleton = new TasteDefinition();

        // Methods
        public override void Cleanup()
        {
            base.Cleanup();
        }

        public override ThumbnailKey GetIconKey()
        {
            if (base.Target != null)
            {
                return base.GetIconKey();
            }
            if (base.Actor != null)
            {
                GameObject objectInRightHand = base.Actor.GetObjectInRightHand();
                if (objectInRightHand != null)
                {
                    return objectInRightHand.GetThumbnailKey();
                }
            }
            return base.Actor.GetThumbnailKey();
        }

        public NectarBottle.NectarGlass MakeGlass()
        {
            NectarBottle.NectarGlass glass = GlobalFunctions.CreateObject("NectarGlass", base.Target.Position, base.Target.Level, Vector3.UnitZ) as NectarBottle.NectarGlass;
            glass.SetHiddenFlags(HiddenFlags.Model);
            //glass.mBottleInfo = base.Target.mBottleInfo;
            glass.ValueOfNectar = base.Target.Value;
            return glass;
        }

        protected override bool Run()
        {
            if (base.Actor.RouteToObjectRadiusAndCheckInUse(base.Target, 0.7f))
            {
                base.Actor.PlaySoloAnimation("a2o_object_genericSwipe_x", true);
                if (base.Actor.Inventory.TryToAdd(base.Target))
                {
                    return this.RunFromInventory();
                }
            }
            return false;
        }

        protected override bool RunFromInventory()
        {
            if (!base.Actor.Inventory.Contains(base.Target))
            {
                return false;
            }
            base.StandardEntry();
            base.BeginCommodityUpdates();
            base.EnterStateMachine("NectarBottle", "Enter", "x");
            base.SetActor("nectarbottle", base.Target);
            BarTray actor = GlobalFunctions.CreateObject("NectarTray", base.Target.Position, base.Target.Level, Vector3.UnitZ) as BarTray;
            actor.SetHiddenFlags(HiddenFlags.Model);
            base.SetActor("tray", actor);
            //base.Target.CheckStartVFx();
            //if (base.Target.mFxHelper != null)
            //{
            //    base.Target.mFxHelper.FlameOn();
            //}
            NectarBottle.NectarGlass[] glassArray = new NectarBottle.NectarGlass[4];
            for (int i = 0; i < glassArray.Length; i++)
            {
                glassArray[i] = this.MakeGlass();
                base.SetActor("glassBar" + i, glassArray[i]);
            }
            CarrySystem.EnterWhileHolding(base.Actor, actor);
            base.AnimateSim("Exit");
            foreach (NectarBottle.NectarGlass glass in glassArray)
            {
                glass.CheckStartVFx();
            }
            base.EndCommodityUpdates(true);
            base.StandardExit();
            base.Actor.Inventory.TryToRemove(base.Target);
            base.DestroyObject(base.Target);
            if (base.InteractionDefinition is TasteDefinition)
            {
                CarrySystem.PutDown(base.Actor, SurfaceType.Normal);
                CarrySystem.PickUp(base.Actor, glassArray[0]);
                glassArray[0].PushDrinkAsContinuation(base.Actor);
            }
            return true;
        }

        // Nested Types
        private class Definition : InteractionDefinition<Sim, NectarBottle, NectarBottle.ServeNectar>
        {
            // Methods
            public override string GetInteractionName(ref InteractionInstanceParameters parameters)
            {
                return Localization.LocalizeString("Gameplay/Abstracts/ScriptObject/ServeNectar:InteractionName", new object[0]);
            }

            protected override bool Test(Sim a, NectarBottle target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.Parent is NectarMaker)
                {
                    return false;
                }
                return true;
            }
        }

        private class TasteDefinition : ServeNectar.Definition
        {
            // Methods
            public override string GetInteractionName(ref InteractionInstanceParameters parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/CookingObjects/NectarBottle/ServeNectar:InteractionName", new object[0]);
            }
        }
    }


}
