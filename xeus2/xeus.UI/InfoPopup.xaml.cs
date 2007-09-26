using System.Windows.Controls;
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
        readonly ObservableCollectionDisp<Event> _events = new ObservableCollectionDisp<Event>();
        readonly ListBox _listBox = new ListBox();

        public InfoPopup()
        {
            InitializeComponent();

            _listBox.ItemsSource = _events;
            Child = _listBox;
        }

        internal void Add(Event lastEvent)
        {
            _events.Add(lastEvent);
            Resize();
        }

        protected override void OnOpened(System.EventArgs e)
        {
            base.OnOpened(e);
            Resize();
        }

        void Resize()
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