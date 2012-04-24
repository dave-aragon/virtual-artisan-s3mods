using System;
using System.Collections.Generic;
using System.Text;
using Sims3.UI;
using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;

namespace Misukisu.PosePlayerAddon
{
    public class MoveThings
    {

        public static string[] GetMenuPath()
        {
            return new String[] { "Move..." };
        }

        public static string[] GetTiltMenuPath()
        {
            return new String[] { "Tilt And Turn..." };
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

        internal static void MoveUp(GameObject sim, float meters)
        {
            sim.SetPosition(
                new Vector3(
                    sim.Position.x,
                    sim.Position.y + meters,
                    sim.Position.z));
        }

        internal static void MoveDown(GameObject sim, float meters)
        {
            sim.SetPosition(
                new Vector3(
                    sim.Position.x,
                    sim.Position.y - meters,
                    sim.Position.z));
        }

        internal static void MoveBackward(GameObject sim, float meters)
        {
            sim.SetPosition(
                 new Vector3(
                     sim.Position.x - (sim.ForwardVector.x * meters),
                     sim.Position.y,
                     sim.Position.z - (sim.ForwardVector.z * meters)));
        }

        internal static void MoveForward(GameObject sim, float meters)
        {
            sim.SetPosition(
                new Vector3(
                    sim.Position.x + (sim.ForwardVector.x * meters),
                    sim.Position.y,
                    sim.Position.z + (sim.ForwardVector.z * meters)));
        }
        public static float ANGLE_45 = (float)Math.PI / 4;
        public static float ANGLE_90 = 1.57079637f;
        public static float ANGLE_315 = (float)-Math.PI / 4;

        internal static void TurnLeft(GameObject sim)
        {
            Vector3 newForward = Quaternion.MakeFromEulerAngles(0f, ANGLE_315, 0f).ToMatrix().TransformVector(sim.ForwardVector);
            sim.SetForward(newForward);

            //Quaternion q = Quaternion.MakeFromEulerAngles(0, ANGLE_45, 0);
            //Vector3 newForward = (q * sim.ForwardVector).Vector;
            //sim.SetForward(newForward);
        }
        internal static void TurnRight(GameObject sim)
        {
            Vector3 newForward = Quaternion.MakeFromEulerAngles(0f, ANGLE_90, 0f).ToMatrix().TransformVector(sim.ForwardVector);
            sim.SetForward(newForward);
        }
        internal static void TurnAround(GameObject sim)
        {
            Vector3 newForward = new Vector3(-sim.ForwardVector.x, sim.ForwardVector.y, -sim.ForwardVector.z);
            sim.SetForward(newForward);
        }

        internal static void Turn(GameObject sim, float angle)
        {
            sim.SetRotation((float)(angle * Math.PI / 180));
        }

        internal static void Tilt(GameObject sim, float x, float y, float z)
        {
            Vector3 newForward = new Vector3(x, y, z).Normalize();
            sim.SetForward(newForward);
        }
    }
}
