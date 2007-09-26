using System.Collections.Specialized;
using System.Timers;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : UserControl
    {
        private readonly ObservableCollectionDisp<Event> _events = new ObservableCollectionDisp<Event>();

        readonly Timer _display = new Timer();

        private Event _eventToDisplay = null;

        private delegate void RedisplayCallback();

        public Info()
        {
            InitializeComponent();

            _display.AutoReset = false;
            _display.Interval = 500.0;
            _display.Elapsed += _display_Elapsed;

            _events.CollectionChanged += _events_CollectionChanged;
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
            }
        }

        internal ObservableCollectionDisp<Event> Events
        {
            get
            {
                return _events;
            }
        }

        private void _events_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (_content.Content != null)
                        {
                            _eventToDisplay = e.NewItems[e.NewItems.Count - 1] as Event;

                            _display.Stop();
                            _display.Start();
                        }
                        else
                        {
                            _content.Content = e.NewItems[e.NewItems.Count - 1];
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
                            _eventToDisplay = _events[_events.Count - 1];

                            _display.Stop();
                            _display.Start();
                        }
                        else
                        {
                            _content.Content = _events[_events.Count - 1];
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        _content.Content = null;
                        break;
                    }
            }

            _buttons.Visibility = (_events.Count == 0) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void _next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void _prev_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}