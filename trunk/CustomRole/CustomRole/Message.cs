using System;
using System.Collections.Generic;
using System.Text;
using Sims3.UI;
using System.Diagnostics;

namespace Misukisu.Common
{
    class Message
    {
        public static void Show(string msg)
        {
            StyledNotification.Format format = new StyledNotification.Format(msg, StyledNotification.NotificationStyle.kSystemMessage);
            StyledNotification.Show(format);
        }

        public static void StackTrace()
        {
            string trace = new StackTrace().ToString();
            Show(trace);
        }
    }
}
