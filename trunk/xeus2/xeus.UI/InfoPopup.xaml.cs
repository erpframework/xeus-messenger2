using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for InfoPopup.xaml
    /// </summary>
    public partial class InfoPopup
    {
        public InfoPopup()
        {
            InitializeComponent();
        }

        internal void Add(Event lastEvent)
        {
            _list.Items.Add(lastEvent);

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

            HorizontalOffset = primaryScreen.WorkingArea.Right;
            VerticalOffset = primaryScreen.WorkingArea.Bottom;

            IsOpen = true;

            VerticalOffset -= _list.ActualHeight;
        }
    }
}