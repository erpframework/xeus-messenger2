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
        private static readonly List<Event> _notifications = new List<Event>(256);

        public delegate void NegotiateAddNotificationCallback(Event myEvent, NegotiateNotification negotiateNotification);

        public delegate void RefreshCallback();

        public static event NegotiateAddNotificationCallback NegotiateAddNotification;

        static readonly Timer _expTimer = new Timer();

        static void _expTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<Event> toBeRemoved = new List<Event>();

            lock (_notificationLock)
            {
                foreach (Event notification in _notifications)
                {
                    if (notification.Expiration < DateTime.Now)
                    {
                        toBeRemoved.Add(notification);
                    }
                }

                foreach (Event @event in toBeRemoved)
                {
                    _notifications.Remove(@event);
                }
            }

            if (toBeRemoved.Count > 0)
            {
                RefreshStatus();
            }
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
                if (myEvent is EventChatMessage
                    || myEvent is EventError
                    || myEvent is EventErrorAuth
                    || myEvent is EventErrorConnection
                    || myEvent is EventErrorProtocol
                    || myEvent is EventException
                    || myEvent is EventPresenceChanged)
                {
                    bool notify = true;

                    EventPresenceChanged presenceChanged = myEvent as EventPresenceChanged;

                    if (presenceChanged != null)
                    {
                        if ((presenceChanged.OldPresence == null
                            || presenceChanged.OldPresence.Type == PresenceType.unavailable)
                            && presenceChanged.NewPresence.Type == PresenceType.available
                            && (presenceChanged.NewPresence.Show == ShowType.NONE
                                || presenceChanged.NewPresence.Show == ShowType.chat)
                            && !Settings.Default.UI_Notify_PresenceAvailable)
                        {
                            // online, free for chat
                            notify = false;
                        } 
                        else if (presenceChanged.OldPresence != null
                            && presenceChanged.OldPresence.Type == PresenceType.available
                            && presenceChanged.NewPresence.Type == PresenceType.unavailable
                            && !Settings.Default.UI_Notify_PresenceUnavailable)
                        {
                            // offline
                            notify = false;
                        }
                        else if (presenceChanged.OldPresence != null
                            && presenceChanged.NewPresence.Type != presenceChanged.OldPresence.Type
                            && presenceChanged.NewPresence.Type == PresenceType.available
                            && (presenceChanged.NewPresence.Show == ShowType.away
                                || presenceChanged.NewPresence.Show == ShowType.dnd
                                || presenceChanged.NewPresence.Show == ShowType.xa)
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
                            _notifications.Add(myEvent);
                            Added(myEvent);
                        }
                    }
                }
            }
        }

        private static void Added(Event @event)
        {
            NotificationTray.Instance.ItemAdded(@event);
            NotificationPopup.Instance.ItemAdded(@event);
            NotificationSound.Instance.ItemAdded(@event);

            RefreshStatus();
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

            if (toBeRemoved.Count > 0)
            {
                RefreshStatus();
            }
        }

        public static void Initialize()
        {
            Events.Instance.OnEventRaised += Instance_OnEventRaised;

            _expTimer.AutoReset = true;
            _expTimer.Interval = 250.0;
            _expTimer.Elapsed += _expTimer_Elapsed;

            _expTimer.Enabled = true;
        }
    }
}