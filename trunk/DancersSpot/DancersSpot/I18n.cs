using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Utilities;

namespace Misukisu.Common
{
    class CommonTexts
    {
        public static string DEBUG_STARTING = "Misukisu.Common.Debug:Starting";
        public static string DEBUG_STARTING_TITLE = "Misukisu.Common.Debug:StartingTitle";
        public static string DEBUG_CANNOT_START_LOGGER= "Misukisu.Common.Debug:CannotStartLogger";
        public static string DEBUG_CANNOT_CREATE_LOG_FILE= "Misukisu.Common.Debug:CannotCreateLogFile";
        public static string DEBUG_STOP= "Misukisu.Common.Debug:Start";
        public static string DEBUG_START= "Misukisu.Common.Debug:Stop";
        public static string MESSAGE_CRITICAL_ERROR= "Misukisu.Common.Message:CriticalError";

    }

    class I18n
    {

        public static string Localize(string key, string defaultValue)
        {
            if (Localization.HasLocalizationString(key))
            {
                return Localization.LocalizeString(key, new string[0]);
            }
            else
            {
                return defaultValue;
            }

        }

        public static string Localize(string key, object[] args, string defaultValue)
        {
            if (Localization.HasLocalizationString(key))
            {
                return Localization.LocalizeString(key, args);
            }
            else
            {
                return defaultValue;
            }

        }
    }
}
