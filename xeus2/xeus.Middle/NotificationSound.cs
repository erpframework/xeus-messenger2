using xeus2.xeus.Core;

namespace xeus2.xeus.Middle
{
    internal class NotificationSound : INotification
    {
        private static readonly NotificationSound _instance = new NotificationSound();

        public static NotificationSound Instance
        {
            get
            {
                return _instance;
            }
        }

        #region INotification Members

        public void RefreshStatus()
        {
        }

        #endregion
    }
}