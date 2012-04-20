using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.UI;

namespace Misukisu.PosePlayerAddon
{
    public class MoveSim
    {

        public static string[] GetMenuPath()
        {
            return new String[] { "Move..." };
        }

        internal static float AskUserInput(string title, string labelText)
        {
            string text = StringInputDialog.Show(title, labelText, "", 256, StringInputDialog.Validation.Number);
            if (text == null || text == "")
            {
                return 0F;
            }
            float result = 0f;
            if (!float.TryParse(text, out result))
            {
                return 0F;
            }
            return result;
        }

        internal static void MoveUp(Sim sim, float meters)
        {
            sim.SetPosition(
                new Vector3(
                    sim.Position.x,
                    sim.Position.y + meters,
                    sim.Position.z));
        }

        internal static void MoveDown(Sim sim, float meters)
        {
            sim.SetPosition(
                new Vector3(
                    sim.Position.x,
                    sim.Position.y - meters,
                    sim.Position.z));
        }

        internal static void MoveBackward(Sim sim, float meters)
        {
            sim.SetPosition(
                 new Vector3(
                     sim.Position.x - (sim.ForwardVector.x * meters),
                     sim.Position.y,
                     sim.Position.z - (sim.ForwardVector.z * meters)));
        }

        internal static void MoveForward(Sim sim, float meters)
        {
            sim.SetPosition(
                new Vector3(
                    sim.Position.x + (sim.ForwardVector.x * meters),
                    sim.Position.y,
                    sim.Position.z + (sim.ForwardVector.z * meters)));
        }
        internal static void Turn(Sim sim, float angle)
        {
            Quaternion q = Quaternion.MakeFromEulerAngles(0, angle, 0);
            Vector3 newForward = (q * sim.ForwardVector).Vector;
            sim.SetForward(newForward);
        }
        internal static void TurnRight(Sim sim)
        {
            Vector3 newForward = Quaternion.MakeFromEulerAngles(0f, 1.57079637f, 0f).ToMatrix().TransformVector(sim.ForwardVector);
            sim.SetForward(newForward);
        }
        internal static void TurnAround(Sim sim)
        {
            Vector3 newForward = new Vector3(-sim.ForwardVector.x, sim.ForwardVector.y, -sim.ForwardVector.z);
            sim.SetForward(newForward);
        }

        //internal static void Turn(Sim sim, float angle)
        //{
        //    Debugger log = new Debugger(sim);
        //    Quaternion q = Quaternion.MakeFromForwardVector(sim.ForwardVector);
        //    float oldAngle = Quaternion.GetAngle(q);
        //    log.Debug(sim, "Forward is X: " + sim.ForwardVector.x + " Y: " + sim.ForwardVector.y + " Z: " + sim.ForwardVector.z);
        //    log.Debug(sim, "Old angle was " + oldAngle + " radians - " + (oldAngle * (180 / Math.PI))+ " degrees");
        //    float newAngle = (oldAngle + angle) % 6.28318548f;
        //    log.Debug(sim, "New angle was " + newAngle + " radians - " + (newAngle * (180 / Math.PI)) + " degrees");
        //    log.EndDebugLog();
        //    sim.SetRotation(newAngle);
        //}
    }

    class MoveRight : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveSim.TurnRight(Target);
            MoveSim.MoveForward(this.Target, 1f);
            MoveSim.TurnAround(Target);
            MoveSim.TurnRight(Target);
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, MoveRight>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Right 1 Meter";
            }
        }
    }

    class MoveLeft : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveSim.TurnAround(Target);
            MoveSim.TurnRight(Target);
            MoveSim.MoveForward(this.Target, 1f);
            MoveSim.TurnRight(Target);
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, MoveLeft>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Left 1 Meter";
            }
        }
    }

    class MoveForwardUserDefined : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveSim.AskUserInput("Move Sim Forward", "How many centimeters?");
            if (centimeters > 0F)
            {
                MoveSim.MoveForward(this.Target, centimeters/100);
            }
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, MoveForwardUserDefined>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Forward...";
            }
        }
    }

    class MoveForward : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveSim.MoveForward(this.Target, 1f);
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, MoveForward>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Forward 1 Meter";
            }
        }
    }

    class MoveBack : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveSim.MoveBackward(this.Target, 1f);
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, MoveBack>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Back 1 Meter";
            }
        }
    }

    class MoveUpUserDefined : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            float centimeters = MoveSim.AskUserInput("Move Sim Up","How many centimeters?");
            if (centimeters > 0F)
            {
                MoveSim.MoveUp(this.Target, centimeters/100);
            }
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, MoveUpUserDefined>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Up...";
            }
        }
    }
   

    class MoveUp : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveSim.MoveUp(this.Target, 1f);
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, MoveUp>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Up 1 Meter";
            }
        }
    }

    class MoveDown : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveSim.MoveDown(this.Target, 1f);
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, MoveDown>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Down 1 Meter";
            }
        }
    }

    class TurnLeft : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveSim.Turn(this.Target, (float)Math.PI / 4);
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, TurnLeft>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Turn Left";
            }
        }
    }

    class TurnRight : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveSim.TurnRight(this.Target);
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, TurnRight>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Turn Right";
            }
        }
    }

    class TurnAround : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            MoveSim.TurnAround(this.Target);
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Sim, TurnAround>
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref Sims3.SimIFace.GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return PoseManager.IsPosing(target) && !isAutonomous;
            }
            public override string[] GetPath(bool isFemale)
            {
                return MoveSim.GetMenuPath();
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return "Turn Around";
            }
        }
    }
}
