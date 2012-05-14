using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sims3.SimIFace;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using Microsoft.Win32;


namespace Test
{

    public struct Matrix33
    {
        public Vector3 row1;
        public Vector3 row2;
        public Vector3 row3;

        public static Matrix33 forYRotation(double radians)
        {
            Matrix33 matrix = new Matrix33();
            // ROW 1
            matrix.row1.x = (float)Math.Cos(radians);
            matrix.row1.y = 0f;
            matrix.row1.z = (float)Math.Sin(radians);
            // ROW 2
            matrix.row2.x = 0f;
            matrix.row2.y = 1f;
            matrix.row2.z = 0f;
            // ROW 3
            matrix.row3.x = -((float)Math.Sin(radians));
            matrix.row3.y = 0f;
            matrix.row3.z = (float)Math.Cos(radians);
            return matrix;
        }

        public Vector3 TransformVector(Vector3 original)
        {
            Vector3 result = new Vector3();
            Vector3 row1 = this.row1;
            result.x = (row1.x * original.z + row1.y * original.y + row1.z * original.x);
            Vector3 row2 = this.row2;
            result.y = (row2.x * original.z + row2.y * original.y + row2.z * original.x);
            Vector3 row3 = this.row3;
            result.z = (row3.x * original.z + row3.y * original.y + row3.z * original.x);
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Process[] plist = System.Diagnostics.Process.GetProcessesByName("TS3W");
            Console.WriteLine("Processes found: " + plist.Length);
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Sims\The Sims 3");
            string basepath = key.GetValue("Install Dir") as string;
            basepath = Path.Combine(basepath, "Game", "Bin", "TS3W.exe");
            Console.WriteLine(basepath);
            Console.ReadLine();
        }

        //C:\Program Files (x86)\Electronic Arts\The Sims 3\Game\Bin\TS3W.exe

   

        

        

        static void TestAngles(string[] args)
        {
            //RotateXZ();

            Vector3 forw = new Vector3(0f, 1f, 0f).Normalize();
            PrintAngle(forw,0);
            for (int i = 1; i < 10; i++)
            {
                Vector3 newForward = Quaternion.MakeFromEulerAngles(0f, (float)Math.PI / 4, 0f).ToMatrix().TransformVector(forw);
                PrintAngle(newForward,i);
                forw = newForward;
            }



            Console.WriteLine("*************");
            OwnFunction();

            Console.ReadLine();
            return;

            SuperDef def = TestInnerClass.Singleton;
            Console.WriteLine(def.GetType().ToString());
            Quaternion q = Quaternion.MakeFromEulerAngles(0, 0.785398163F, 0);
            float angle = 1.57079637f;
            //Quaternion q = Quaternion.MakeFromEulerAngles(0, angle, 0);

            Vector3 v = new Vector3(0.5f, 0, 0.5f).Normalize();
            PrintAngle(v,0);

            Vector3 v1 = q.ToMatrix().TransformVector(v);
            PrintAngle(v1,0);
            Vector3 v2 = q.ToMatrix().TransformVector(v1);
            PrintAngle(v2,0);
            Vector3 v3 = q.ToMatrix().TransformVector(v2);
            PrintAngle(v3,0);
            Vector3 v4 = q.ToMatrix().TransformVector(v3);
            PrintAngle(v4,0);
            Vector3 v5 = q.ToMatrix().TransformVector(v4);
            PrintAngle(v5,0);
            Vector3 v6 = q.ToMatrix().TransformVector(v2);
            PrintAngle(v6,0);

            //PrintAngle(1,0,0);
            //PrintAngle(0, 0, 1);
            //PrintAngle(0.5F, 0, 0.5F);
            //PrintAngle(-0.5F, 0, -0.5F);
            //PrintAngle(-1F, 0, 0.5F);
            //PrintAngle(+1F, 0, -0.5F);
            //Console.WriteLine("Angle conv 45=" + (Math.PI/4));
            Console.ReadLine();
        }

        private static void OwnFunction()
        {
            Vector3 forw = new Vector3(0.5f, 0f, 0.5f).Normalize();
            PrintAngle(forw,0);
            for (int i = 1; i < 10; i++)
            {
                Vector3 newForward = Matrix33.forYRotation(Math.PI / 4).TransformVector(forw);
                PrintAngle(newForward,i);
                forw = newForward;
            }
            
        }

        private static Vector3 RotateXZ()
        {
            Vector3 forw = new Vector3(0.5f, 0f, 0.5f).Normalize();
            PrintAngle(forw,0);
            for (int i = 1; i < 10; i++)
            {
                Vector3 newForward = Quaternion.MakeFromEulerAngles(1.57079637f, 0f, 1.57079637f).ToMatrix().TransformVector(forw);
                PrintAngle(newForward,i);
                forw = newForward;
            }
            return forw;
        }

        private static void PrintAngle(float x, float y, float z)
        {
            Vector3 v1 = new Vector3(x, y, z);
            v1 = PrintAngle(v1,0);
        }

        private static Vector3 PrintAngle(Vector3 vect, int i)
        {
            Quaternion q = Quaternion.MakeFromForwardVector(vect);
            Console.WriteLine(i+": " + vect.x + "\t - " + vect.y + " - \t" + vect.z + " is \t" );
            //Console.WriteLine("Angle is "+Quaternion.GetAngle(q) * (180 / Math.PI) + " (" + Quaternion.GetAngle(q) + " radians)");

            return vect;
        }
    }
}
