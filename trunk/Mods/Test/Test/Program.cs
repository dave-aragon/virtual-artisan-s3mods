using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sims3.SimIFace;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            SuperDef def = TestInnerClass.Singleton;
            Console.WriteLine(def.GetType().ToString());
            Quaternion q = Quaternion.MakeFromEulerAngles(0, 0.785398163F, 0);
            float angle = 1.57079637f;
            //Quaternion q = Quaternion.MakeFromEulerAngles(0, angle, 0);
            
            Vector3 v = new Vector3(0.5f, 0, 0.5f).Normalize();
            PrintAngle(v);

            Vector3 v1=q.ToMatrix().TransformVector(v);
            PrintAngle(v1);
            Vector3 v2 = q.ToMatrix().TransformVector(v1);
            PrintAngle(v2);
            Vector3 v3 = q.ToMatrix().TransformVector(v2);
            PrintAngle(v3);
            Vector3 v4 = q.ToMatrix().TransformVector(v3);
            PrintAngle(v4);
            Vector3 v5 = q.ToMatrix().TransformVector(v4);
            PrintAngle(v5);
            Vector3 v6 = q.ToMatrix().TransformVector(v2);
            PrintAngle(v6);

            //PrintAngle(1,0,0);
            //PrintAngle(0, 0, 1);
            //PrintAngle(0.5F, 0, 0.5F);
            //PrintAngle(-0.5F, 0, -0.5F);
            //PrintAngle(-1F, 0, 0.5F);
            //PrintAngle(+1F, 0, -0.5F);
            //Console.WriteLine("Angle conv 45=" + (Math.PI/4));
            Console.ReadLine();
        }

        private static void PrintAngle(float x, float y, float z)
        {
            Vector3 v1 = new Vector3(x, y, z);
            v1 = PrintAngle(v1);
        }

        private static Vector3 PrintAngle(Vector3 vect)
        {
            Quaternion q = Quaternion.MakeFromForwardVector(vect);
            Console.WriteLine("Angle " + vect.x + "-" + vect.y + "-" + vect.z + " is " + Quaternion.GetAngle(q) * (180 / Math.PI) + " ("+Quaternion.GetAngle(q)+" radians)");
            
            return vect;
        }
    }
}
