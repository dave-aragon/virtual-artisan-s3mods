using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{

    public class SuperDef { }

    public class TestInnerClass 
    {
        public static readonly SuperDef Singleton = new Definition();

        

        private sealed class Definition:SuperDef 
        {

            
        }
    }
}
