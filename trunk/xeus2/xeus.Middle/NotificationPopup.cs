using System;
using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class NotificationPopup : INotification
    {
        private static readonly NotificationPopup _instance = new NotificationPopup();
        private InfoPopup _infoPopup = null;

        public static NotificationPopup Instance
        {
            get
            {
                return _instance;
            }
        }

        #region INotification Members

        public void RefreshStatus()
        {
            Event lastEvent = Notification.GetFirstEvent<Event>();

            if (lastEvent != null)
            {
                _infoPopup.Add(lastEvent);
            }
        }

        #endregion

        public void Initialize(InfoPopup infoPopup)
        {
            _infoPopup = infoPopup;
        }
    }
}