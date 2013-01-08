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

namespace Misukisu.Decorator
{

    class MoveRight : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.TurnRight(Target);
            MoveThings.MoveForward(this.Target, 1f);
            MoveThings.TurnAround(Target);
            MoveThings.TurnRight(Target);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveRight>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Right 1 Tile";
            }
        }
    }

    class MoveRightUserDefined : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Left", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                MoveThings.TurnRight(Target);
                MoveThings.MoveForward(this.Target, centimeters / 100);
                MoveThings.TurnAround(Target);
                MoveThings.TurnRight(Target);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveRightUserDefined>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Right...";
            }
        }
    }

    class MoveLeft : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.TurnAround(Target);
            MoveThings.TurnRight(Target);
            MoveThings.MoveForward(this.Target, 1f);
            MoveThings.TurnRight(Target);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveLeft>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Left 1 Tile";
            }
        }
    }

    class MoveLeftUserDefined : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Left", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                MoveThings.TurnAround(Target);
                MoveThings.TurnRight(Target);
                MoveThings.MoveForward(this.Target, centimeters / 100);
                MoveThings.TurnRight(Target);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveLeftUserDefined>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Left...";
            }
        }
    }

    class MoveForwardUserDefined : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Forward", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                MoveThings.MoveForward(this.Target, centimeters / 100);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveForwardUserDefined>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Forward...";
            }
        }
    }

    class MoveForward : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.MoveForward(this.Target, 1f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveForward>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Forward 1 Tile";
            }
        }
    }

    class MoveBackUserDefined : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Backwards", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                MoveThings.MoveBackward(this.Target, centimeters / 100);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveBackUserDefined>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Backwards...";
            }
        }
    }

    class MoveBack : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.MoveBackward(this.Target, 1f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveBack>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Backwards 1 Tile";
            }
        }
    }

    class MoveUpUserDefined : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Up", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                MoveThings.MoveUp(this.Target, centimeters / 100);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveUpUserDefined>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Up...";
            }
        }
    }


    class MoveUp : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.MoveUp(this.Target, 1f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveUp>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Up 1 Tile";
            }
        }
    }

    class MoveDownUserDefined : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveThings.AskUserInput("Move Down", "How many units? (100 = 1 Tile)");
            if (centimeters > 0F)
            {
                MoveThings.MoveDown(this.Target, centimeters / 100);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveDownUserDefined>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Down...";
            }
        }
    }

    class MoveDown : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.MoveDown(this.Target, 1f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, MoveDown>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Down 1 Tile";
            }
        }
    }

    class TurnLeft : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.TurnLeft(this.Target);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, TurnLeft>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Turn Left";
            }
        }
    }

    class TurnAtAngle : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float angle = MoveThings.AskUserInput("Turn To Global Angle", "Angle (1 - 360)");
            if (angle > 0F)
            {

                MoveThings.Turn(this.Target, angle);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, TurnAtAngle>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Turn...";
            }
        }
    }

    class TiltFaceUp : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.Tilt(this.Target, 0.01f, 0.9999f, 0.01f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, TiltFaceUp>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Tilt Backwards 90 Degrees";
            }
        }
    }

    class TiltBack : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.Tilt(this.Target, 0.4109836f, 0.8137475f, 0.4109836f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, TiltBack>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Tilt Backwards";
            }
        }
    }

    class TiltUserDefined : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            string[] values = ThreeStringInputDialog.Show("Give Forward Vector",
                new String[] { "X: (-0.99 - 0.99)", "Y: (-0.99 - 0.99)", "Z: (-0.99 - 0.99)" },
                new String[] { Target.ForwardVector.x.ToString(), Target.ForwardVector.y.ToString(), Target.ForwardVector.z.ToString() }, false);

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
            MoveThings.Tilt(this.Target, x, y, z);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, TiltUserDefined>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Tilt...";
            }
        }
    }

    class TiltForward : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.Tilt(this.Target, -0.4109836f, -0.8137475f, -0.4109836f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, TiltForward>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Tilt Forward";
            }
        }
    }

    class TiltFaceDown : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.Tilt(this.Target, -0.01f, -0.9999f, -0.01f);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, TiltFaceDown>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Tilt Forward 90 Degrees";
            }
        }
    }

    class TurnRight : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.TurnRight(this.Target);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, TurnRight>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Turn Right";
            }
        }
    }

    class TurnAround : ImmediateInteraction<Sim, GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveThings.TurnAround(this.Target);
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : ActorlessInteractionDefinition<Sim, GameObject, TurnAround>
        {
            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return  !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveThings.GetTiltMenuPath();
            }
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return "Turn Around";
            }
        }
    }
}
