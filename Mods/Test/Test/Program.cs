using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            SuperDef def = TestInnerClass.Singleton;
            Console.WriteLine(def.GetType().ToString());
            Console.ReadLine();
        }
    }
}
