using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Sims3.SimIFace;
using ScriptCore;


namespace Misukisu.Stubble
{
    class LocalizerWithCollector : IStringTable
    {
            public static List<string> stringCollection = new List<string>();
            private IStringTable gStringTable;

            public LocalizerWithCollector()
            {
                new LocalizedStringService();
                gStringTable = (AppDomain.CurrentDomain.GetData("LocalizedStringService") as IStringTable);
            }

            public string GetLocalizedString(ulong key)
            {
                return gStringTable.GetLocalizedString(key);
            }

            
            public string GetLocalizedString(string key)
            {
                if (key.Contains("/") && !stringCollection.Contains(key))
                {
                    stringCollection.Add(key);
                }
                return gStringTable.GetLocalizedString(key);
            }
           
            public string GetLocale()
            {
                return gStringTable.GetLocale();
            }
            public string GetLocalizedLogoSuffix()
            {
                return gStringTable.GetLocalizedLogoSuffix();
            }

            internal List<string> GetCollectedStrings()
            {
                return stringCollection;
            }
    }
}
