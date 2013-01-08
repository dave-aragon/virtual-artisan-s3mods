using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Abstracts;
using Misukisu.Decorator;

namespace Sims3.Gameplay.Objects.Misukisu
{

    class MoveRightProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.TurnRight(itemToMove);
            MoveThings.MoveForward(itemToMove, 1f);
            MoveThings.TurnAround(itemToMove);
            MoveThings.TurnRight(itemToMove);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveRightProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove() && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Right 1 Tile";
            }
        }
    }

    class MoveRightUserDefinedProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Left", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                GameObject itemToMove = Target.GetItemToMove();
                MoveThings.TurnRight(itemToMove);
                MoveThings.MoveForward(itemToMove, centimeters / 100);
                MoveThings.TurnAround(itemToMove);
                MoveThings.TurnRight(itemToMove);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveRightUserDefinedProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Right...";
            }
        }
    }

    class MoveLeftProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.TurnAround(itemToMove);
            MoveThings.TurnRight(itemToMove);
            MoveThings.MoveForward(itemToMove, 1f);
            MoveThings.TurnRight(itemToMove);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveLeftProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Left 1 Tile";
            }
        }
    }

    class MoveLeftUserDefinedProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Left", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                GameObject itemToMove = Target.GetItemToMove();
                MoveThings.TurnAround(itemToMove);
                MoveThings.TurnRight(itemToMove);
                MoveThings.MoveForward(itemToMove, centimeters / 100);
                MoveThings.TurnRight(itemToMove);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveLeftUserDefinedProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Left...";
            }
        }
    }

    class MoveForwardUserDefinedProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Forward", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                GameObject itemToMove = Target.GetItemToMove();
                MoveThings.MoveForward(itemToMove, centimeters / 100);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveForwardUserDefinedProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Forward...";
            }
        }
    }

    class MoveForwardProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.MoveForward(itemToMove, 1f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveForwardProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Forward 1 Tile";
            }
        }
    }

    class MoveBackUserDefinedProxy  : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Backwards", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                GameObject itemToMove = Target.GetItemToMove();
                MoveThings.MoveBackward(itemToMove, centimeters / 100);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveBackUserDefinedProxy >
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Backwards...";
            }
        }
    }

    class MoveBackProxy  : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.MoveBackward(itemToMove, 1f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveBackProxy >
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Backwards 1 Tile";
            }
        }
    }

    class MoveUpUserDefinedProxy  : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Up", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                GameObject itemToMove = Target.GetItemToMove();
                MoveThings.MoveUp(itemToMove, centimeters / 100);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveUpUserDefinedProxy >
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Up...";
            }
        }
    }


    class MoveUpProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.MoveUp(itemToMove, 1f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveUpProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Up 1 Tile";
            }
        }
    }

    class MoveDownUserDefinedProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Down", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                GameObject itemToMove = Target.GetItemToMove();
                MoveThings.MoveDown(itemToMove, centimeters / 100);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveDownUserDefinedProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Down...";
            }
        }
    }

    class MoveDownProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.MoveDown(itemToMove, 1f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, MoveDownProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Down 1 Tile";
            }
        }
    }

    class TurnLeftProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.TurnLeft(itemToMove);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, TurnLeftProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Turn Left";
            }
        }
    }

    class TurnAtAngleProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float angle = MoveThings.AskUserInput("Turn To Global Angle", "Angle (1 - 360)");
            if (angle > 0F)
            {
                GameObject itemToMove = Target.GetItemToMove();
                MoveThings.Turn(itemToMove, angle);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, TurnAtAngleProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Turn...";
            }
        }
    }

    class TiltFaceUpProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.Tilt(itemToMove, 0.01f, 0.9999f, 0.01f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, TiltFaceUpProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Tilt Backwards 90 Degrees";
            }
        }
    }

    class TiltBackProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.Tilt(itemToMove, 0.4109836f, 0.8137475f, 0.4109836f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, TiltBackProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Tilt Backwards";
            }
        }
    }

    class TiltUserDefinedProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();

            string[] values = ThreeStringInputDialog.Show("Give Forward Vector",
                new String[] { "X: (-0.99 - 0.99)", "Y: (-0.99 - 0.99)", "Z: (-0.99 - 0.99)" },
                new String[] { itemToMove.ForwardVector.x.ToString(), itemToMove.ForwardVector.y.ToString(), itemToMove.ForwardVector.z.ToString() }, false);

            float x = 0.5F;
            float y = 0.5F;
            float z = 0.5F;
            if (!string.IsNullOrEmpty(values[0]))
            {
                if (!float.TryParse(values[0], out x))
                {
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(values[1]))
            {
                if (!float.TryParse(values[1], out y))
                {
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(values[2]))
            {
                if (!float.TryParse(values[2], out z))
                {
                    return false;
                }
            }
            
            MoveThings.Tilt(itemToMove, x, y, z);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, TiltUserDefinedProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Tilt...";
            }
        }
    }

    class TiltForwardProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.Tilt(itemToMove, -0.4109836f, -0.8137475f, -0.4109836f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, TiltForwardProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Tilt Forward";
            }
        }
    }

    class TiltFaceDownProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.Tilt(itemToMove, -0.01f, -0.9999f, -0.01f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, TiltFaceDownProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Tilt Forward 90 Degrees";
            }
        }
    }

    class TurnRightProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.TurnRight(itemToMove);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, TurnRightProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Turn Right";
            }
        }
    }

    class TurnAroundProxy : ImmediateInteraction<Sim, Decorator>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            GameObject itemToMove = Target.GetItemToMove();
            MoveThings.TurnAround(itemToMove);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, Decorator, TurnAroundProxy>
        {
            public override bool Test(Sim actor, Decorator itemToMove, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous && itemToMove.HasItemsToMove();
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, Decorator itemToMove, InteractionObjectPair iop)
            {
                return "Turn Around";
            }
        }
    }
}
