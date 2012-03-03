using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace AccessModifierHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            Program prog=new Program();
            prog.DoYourMagic();

            Console.WriteLine("All finished, you can close this window");
            Console.ReadLine();
        }

        public void DoYourMagic()
        {
            string[] array = Directory.GetFiles(@".", "*.il");
            foreach (string filename in array)
            {
                ChangeAccessToPublic(filename);
            }
        }


        // This part of the code by Twallan
        private void ChangeAccessToPublic(string filename)
        {
            List<Tuple<string, string>> regEx = new List<Tuple<string, string>>();

            regEx.Add(new Tuple<string, string>("(\\s)assembly(\\s)", "$1public$2"));

            regEx.Add(new Tuple<string, string>("(\\s)family(\\s)", "$1public$2"));

            regEx.Add(new Tuple<string, string>("(\\s)private(\\s)", "$1public$2"));

            regEx.Add(new Tuple<string, string>("(\\s)sealed(\\s)", "$1"));

            regEx.Add(new Tuple<string, string>("(\\s)initonly(\\s)", "$1"));

            regEx.Add(new Tuple<string, string>("\\.field (|static )assembly", ".field $1 public"));

            regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname instance void(\\s*\\n\\s*add_)", ".method private hidebysig specialname instance void$1"));

            regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname instance void(\\s*\\n\\s*remove_)", ".method private hidebysig specialname instance void$1"));

            regEx.Add(new Tuple<string, string>("\\.method public hidebysig newslot specialname virtual(| final)(\\s*\\n\\s*instance void  add_)", ".method private hidebysig newslot specialname virtual $1$2"));

            regEx.Add(new Tuple<string, string>("\\.method public hidebysig newslot specialname virtual(| final)(\\s*\\n\\s*instance void  remove_)", ".method private hidebysig newslot specialname virtual $1$2"));

            regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname static(\\s*\\n\\s*void  add_)", ".method private hidebysig specialname static$1"));

            regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname static(\\s*\\n\\s*void  remove_)", ".method private hidebysig specialname static$1"));



            string contents = null;



            try

            {
                Console.WriteLine("Changing methods and fields public for " + filename);
                using (StreamReader file = new StreamReader(filename))

                {

                    contents = file.ReadToEnd();

                }



                foreach (Tuple<string, string> value in regEx)

                {

                    Regex regex = new Regex(value.Item1);



                    contents = regex.Replace(contents, value.Item2);

                }



                using (StreamWriter file = new StreamWriter(filename))

                {

                    file.Write(contents.ToCharArray());

                }

            }

            finally

            {

               

            }

        }
    }
}
