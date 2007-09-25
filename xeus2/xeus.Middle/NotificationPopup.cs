using System;
using System.Collections.Generic;
using System.Text;
using xeus2.xeus.Core;

namespace xeus2.xeus.Middle
{
    internal class NotificationPopup : INotification
    {
        private static readonly NotificationPopup _instance = new NotificationPopup();

        public static NotificationPopup Instance
        {
            get
            {
                return _instance;
            }
        }

        public void RefreshStatus()
        {
        }
    }
}
