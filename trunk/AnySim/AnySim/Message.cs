using System;
using System.Collections.Generic;
using System.Text;
using Sims3.UI;
using Sims3.Gameplay;
using Sims3.SimIFace;
using Sims3.Gameplay.Abstracts;

namespace Misukisu.Common
{
    class Message
    {
        public static readonly string NewLine = System.Environment.NewLine;
        public static readonly Message Sender = new Message();
        private Debugger mDebugger = null;

        private Message()
            : base()
        {
        }

        public void Show(GameObject owner, string msg)
        {
            StyledNotification.Format format = new StyledNotification.Format(msg, 
                ObjectGuid.InvalidObjectGuid, owner.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
            StyledNotification.Show(format, "tns_icon_bulb");
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

            if (ex != null)
            {
                msg.Append(Message.NewLine);
                msg.Append(ex.Message);
            }
            if (isCritical)
            {
                msg.Append(Message.NewLine);

                msg.Append(I18n.Localize(CommonTexts.MESSAGE_CRITICAL_ERROR,
                    "Please exit the game without saving, this error cannot be recovered from"));
            }
            string fullError = msg.ToString();
            DebugError(this, fullError, ex);
            SimpleMessageDialog.Show("Virtual Artisan - " + projectName, fullError, ModalDialog.PauseMode.PauseSimulator);
        }

        public void setDebugger(Debugger debugger)
        {
            this.mDebugger = debugger;
        }

        public void DebugError(object sender, string msg, Exception ex)
        {
            if (mDebugger != null)
            {
                mDebugger.DebugError(sender, msg, ex);
            }
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
