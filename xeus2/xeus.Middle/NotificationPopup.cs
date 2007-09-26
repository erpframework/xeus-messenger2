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
                _infoPopup.IsOpen = true;
            }
            else
            {
                _infoPopup.IsOpen = false;
            }
        }

        public void ItemAdded(Event @event)
        {
            _infoPopup.Add(@event);
        }

        #endregion

        public void Initialize(InfoPopup infoPopup)
        {
            _infoPopup = infoPopup;
        }
    }
}