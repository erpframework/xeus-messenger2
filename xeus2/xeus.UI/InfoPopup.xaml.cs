using System;
using System.Windows.Forms;
using xeus2.xeus.Core;
using ListBox=System.Windows.Controls.ListBox;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for InfoPopup.xaml
    /// </summary>
    public partial class InfoPopup
    {
        private readonly ObservableCollectionDisp<Event> _events = new ObservableCollectionDisp<Event>();
        private readonly ListBox _listBox = new ListBox();

        public InfoPopup()
        {
            InitializeComponent();

            _listBox.ItemsSource = _events;
            Child = _listBox;
        }

        internal void Add(Event lastEvent)
        {
            lock (_events._syncObject)
            {
                if (_events.Count > 3)
                {
                    _events.RemoveAt(0);
                }
                
                _events.Add(lastEvent);
            }

            Resize();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            Resize();
        }

        private void Resize()
        {
            Screen[] screens = Screen.AllScreens;

            Screen primaryScreen = screens[0];

            foreach (Screen screen in screens)
            {
                if (screen.Primary)
                {
                    primaryScreen = screen;
                    break;
                }
            }

            Height = 100;
            Width = 200;

            HorizontalOffset = primaryScreen.WorkingArea.Right - _listBox.ActualWidth;
            VerticalOffset = primaryScreen.WorkingArea.Bottom - _listBox.ActualHeight;
        }
    }
}