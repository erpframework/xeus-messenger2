using System;
using System.Collections.Generic;
using System.Timers;
using agsXMPP.protocol.client;
using xeus2.Properties;
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
        private static readonly ObservableCollectionDisp<Event> _notifications = new ObservableCollectionDisp<Event>();

        public delegate void NegotiateAddNotificationCallback(Event myEvent, NegotiateNotification negotiateNotification);

        public delegate void RefreshCallback();

        public static event NegotiateAddNotificationCallback NegotiateAddNotification;

        static readonly Timer _expTimer = new Timer();

        private static bool _dontExpire = false;

        static void _expTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_dontExpire)
            {
                List<Event> toBeRemoved = new List<Event>();

                DateTime now = DateTime.Now;

                lock (_notificationLock)
                {
                        foreach (Event notification in Notifications)
                        {
                            if (notification.Expiration < now)
                            {
                                toBeRemoved.Add(notification);
                            }

                            if (notification.Expiration != DateTime.MaxValue
                                && notification.Expiration > now)
                            {
                                // too new items
                                break;
                            }
                        }

                    foreach (Event @event in toBeRemoved)
                    {
                        Notifications.Remove(@event);
                    }
                }

                if (toBeRemoved.Count > 0)
                {
                    RefreshStatus();
                }
            }

            _expTimer.Start();
        }

        public static ObservableCollectionDisp<Event> Notifications
        {
            get
            {
                return _notifications;
            }
        }

        public static bool DontExpire
        {
            get
            {
                return _dontExpire;
            }
            set
            {
                _dontExpire = value;
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
                if (myEvent is EventChatMessage
                    || myEvent is EventError
                    || myEvent is EventErrorAuth
                    || myEvent is EventErrorConnection
                    || myEvent is EventErrorProtocol
                    || myEvent is EventException
                    || myEvent is EventPresenceChanged
                    || myEvent is EventHeadlineMessage)
                {
                    bool notify = true;

                    EventPresenceChanged presenceChanged = myEvent as EventPresenceChanged;

                    if (presenceChanged != null)
                    {
                        if (presenceChanged.IsGoingOnline()
                            && !Settings.Default.UI_Notify_PresenceAvailable)
                        {
                            // online, free for chat
                            notify = false;
                        } 
                        else if (presenceChanged.IsGoingOffline()
                                && !Settings.Default.UI_Notify_PresenceUnavailable)
                        {
                            // offline
                            notify = false;
                        }
                        else if (presenceChanged.IsChangingShowType()
                                && !Settings.Default.UI_Notify_PresenceOther)
                        {
                            // online, away
                            notify = false;
                        }
                    }

                    if (notify)
                    {
                        lock (_notificationLock)
                        {
                            Notifications.Add(myEvent);
                        }
                    }
                }
            }
        }

        public static T GetFirstEvent<T>() where T:Event
        {
            lock (_notificationLock)
            {
                foreach (Event notification in Notifications)
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
            App.InvokeSafe(App._dispatcherPriority, new RefreshCallback(RefreshStatusInternal));
        }

        static void RefreshStatusInternal()
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
                foreach (Event notification in Notifications)
                {
                    EventChatMessage messageEvent = notification as EventChatMessage;

                    if (messageEvent != null && JidUtil.BareEquals(messageEvent.Contact.Jid, contact.Jid))
                    {
                        toBeRemoved.Add(messageEvent);
                    }
                }

                foreach (EventChatMessage eventChatMessage in toBeRemoved)
                {
                    Notifications.Remove(eventChatMessage);
                }
            }

            if (toBeRemoved.Count > 0)
            {
                RefreshStatus();
            }
        }

        public static void DismissNotificationType(Type type)
        {
            List<Event> toBeRemoved = new List<Event>();

            lock (_notificationLock)
            {
                foreach (Event notification in Notifications)
                {
                    if (notification.GetType() == type)
                    {
                        toBeRemoved.Add(notification);
                    }
                }

                foreach (Event @event in toBeRemoved)
                {
                    Notifications.Remove(@event);
                }
            }

            if (toBeRemoved.Count > 0)
            {
                RefreshStatus();
            }
        }

        public static void Initialize()
        {
            Events.Instance.OnEventRaised += Instance_OnEventRaised;

            _expTimer.AutoReset = false;
            _expTimer.Interval = 1000.0;
            _expTimer.Elapsed += _expTimer_Elapsed;

            _expTimer.Start();
        }
    }
}