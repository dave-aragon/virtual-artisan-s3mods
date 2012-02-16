using System;
using System.Collections.Generic;
using System.Text;
using Sims3.UI;

namespace Misukisu
{
    class Message
    {
        public static void Show(string msg)
        {
            StyledNotification.Format format = new StyledNotification.Format(msg, StyledNotification.NotificationStyle.kSystemMessage);
            StyledNotification.Show(format);
        }
    }
}
