using System;
using System.Collections.Specialized;
using System.Timers;
using System.Windows.Controls;
using xeus2.xeus.Core;
using xeus2.xeus.Middle;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : UserControl
    {
        readonly Timer _display = new Timer();

        private Event _eventToDisplay = null;

        private delegate void RedisplayCallback();

        public Info()
        {
            InitializeComponent();

            _display.AutoReset = false;
            _display.Interval = 500.0;
            _display.Elapsed += _display_Elapsed;

            Notification.Notifications.CollectionChanged += Notifications_CollectionChanged;
        }

        public void Clear()
        {
            _content.Content = null;
            _eventToDisplay = null;
        }

        void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (_content.Content != null)
                        {
                            _eventToDisplay = Notification.GetFirstEvent<Event>();

                            _display.Stop();
                            _display.Start();
                        }
                        else
                        {
                            _content.Content = Notification.GetFirstEvent<Event>();
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (Event item in e.OldItems)
                        {
                            if (item == _content.Content)
                            {
                                _content.Content = null;
                                break;
                            }
                        }

                        if (_content.Content != null)
                        {
                            _eventToDisplay = Notification.GetFirstEvent<Event>();

                            _display.Stop();
                            _display.Start();
                        }
                        else
                        {
                            _content.Content = Notification.GetFirstEvent<Event>();
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        _content.Content = null;
                        break;
                    }
            }

            RefreshNavigation();
        }

        void RefreshNavigation()
        {
            lock (Notification.Notifications._syncObject)
            {
                _buttons.Visibility = (Notification.Notifications.Count > 1)
                                       ? System.Windows.Visibility.Visible
                                       : System.Windows.Visibility.Collapsed;

                int index = Notification.Notifications.IndexOf(_content.Content as Event);

                _count.Text = string.Format("{0} of {1}", index + 1, Notification.Notifications.Count);

                _next.Visibility = (index + 1 < Notification.Notifications.Count)
                                       ? System.Windows.Visibility.Visible
                                       : System.Windows.Visibility.Hidden;

                _prev.Visibility = (index - 1 >= 0)
                                       ? System.Windows.Visibility.Visible
                                       : System.Windows.Visibility.Hidden;
            }
        }

        void _display_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.InvokeSafe(App._dispatcherPriority, new RedisplayCallback(Redisplay));
        }

        private void Redisplay()
        {
            if (_eventToDisplay != null)
            {
                _content.Content = _eventToDisplay;
                _eventToDisplay = null;

                RefreshNavigation();
            }
        }

        private void _next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            lock (Notification.Notifications._syncObject)
            {
                int index = Notification.Notifications.IndexOf(_content.Content as Event);

                if (index >= 0
                    && (index + 1) < Notification.Notifications.Count)
                {
                    _content.Content = Notification.Notifications[index + 1];
                    RefreshNavigation();
                }
            }
        }

        private void _prev_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            lock (Notification.Notifications._syncObject)
            {
                int index = Notification.Notifications.IndexOf(_content.Content as Event);

                if (index > 0
                    && (index - 1) < Notification.Notifications.Count)
                {
                    _content.Content = Notification.Notifications[index - 1];
                    RefreshNavigation();
                }
            }
        }

        private void _content_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EventChatMessage eventChatMessage = _content.Content as EventChatMessage;

            if (eventChatMessage != null)
            {
                Notification.DismissChatMessageNotification(eventChatMessage.Contact);
                Middle.Chat.Instance.DisplayChat(eventChatMessage.Contact);
            }
            else
            {
                Notification.DismissNotificationType(_content.Content);
            }
        }
    }
}