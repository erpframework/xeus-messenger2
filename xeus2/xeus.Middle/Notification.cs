using System.Collections.Generic;
using xeus2.xeus.Core;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Middle
{
    internal class NegotiateNotification
    {
        private bool _raise = true;

        public bool Raise
        {
            get
            {
                return _raise;
            }
            set
            {
                _raise = value;
            }
        }
    }

    internal static class Notification
    {
        private static readonly object _notificationLock = new object();
        private static readonly List<Event> _notifications = new List<Event>(256);

        public delegate void NegotiateAddNotificationCallback(Event myEvent, NegotiateNotification negotiateNotification);

        public static event NegotiateAddNotificationCallback NegotiateAddNotification;

        static Notification()
        {
            Events.Instance.OnEventRaised += Instance_OnEventRaised;
        }

        public static List<Event> Notifications
        {
            get
            {
                return _notifications;
            }
        }

        private static void Instance_OnEventRaised(object sender, Event myEvent)
        {
            NegotiateNotification notification = new NegotiateNotification();

            if (NegotiateAddNotification != null)
            {
                NegotiateAddNotification(myEvent, notification);
            }

            if (notification.Raise)
            {
                if (myEvent is EventChatMessage)
                {
                    lock (_notificationLock)
                    {
                        _notifications.Add(myEvent);

                        RefreshStatus();
                    }
                }
            }
        }

        public static T GetFirstEvent<T>() where T:Event
        {
            lock (_notificationLock)
            {
                foreach (Event notification in _notifications)
                {
                    if (notification is T)
                    {
                        return notification as T;
                    }
                }
            }

            return null;
        }

        static void RefreshStatus()
        {
            NotificationTray.Instance.RefreshStatus();
            NotificationPopup.Instance.RefreshStatus();
            NotificationSound.Instance.RefreshStatus();           
        }

        public static void DismissChatMessageNotification(IContact contact)
        {
            List<EventChatMessage> toBeRemoved = new List<EventChatMessage>();

            lock (_notificationLock)
            {
                foreach (Event notification in _notifications)
                {
                    EventChatMessage messageEvent = notification as EventChatMessage;

                    if (messageEvent != null && JidUtil.BareEquals(messageEvent.Contact.Jid, contact.Jid))
                    {
                        toBeRemoved.Add(messageEvent);
                    }
                }

                foreach (EventChatMessage eventChatMessage in toBeRemoved)
                {
                    _notifications.Remove(eventChatMessage);
                }
            }

            RefreshStatus();
        }
    }
}