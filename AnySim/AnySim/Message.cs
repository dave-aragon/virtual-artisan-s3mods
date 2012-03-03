using System;
using System.Collections.Generic;
using System.Text;
using Sims3.UI;
using Sims3.Gameplay;

namespace Misukisu.Common
{
    class Message 
    {
        public static readonly string NewLine = Environment.NewLine;
        public static readonly Message Sender = new Message();
        private Debugger mDebugger = null;

        private Message()
            : base()
        {
        }

        public void Show(string msg)
        {
            StyledNotification.Format format = new StyledNotification.Format(msg, StyledNotification.NotificationStyle.kSystemMessage);
            StyledNotification.Show(format);
        }

        public void ShowError(string projectName, string error, bool isCritical, Exception ex)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append(error);
            if (isCritical)
            {
                msg.Append("\nPlease exit the game without saving, this error cannot be recovered from");
            }
            if (ex != null)
            {
                msg.Append("\nInternal info:\n");
                msg.Append(ex.StackTrace);
            }
           
            string fullError = msg.ToString();
            Debug(this, fullError);
            if (fullError.Length > 1000)
            {
                fullError = fullError.Substring(0, 600);
            }

            SimpleMessageDialog.Show("Virtual Artisan - " + projectName, fullError, ModalDialog.PauseMode.PauseSimulator);
        }

        public void setDebugger(Debugger debugger)
        {
            this.mDebugger = debugger;
        }

        public void Debug(object sender, string msg)
        {
            if (mDebugger != null)
            {
                mDebugger.Debug(sender, msg);
            }
        }

        public bool IsDebugging()
        {
            if (mDebugger == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        internal Debugger getDebugger()
        {
            return mDebugger;
        }
    }
}
