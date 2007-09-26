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
        public InfoPopup()
        {
            InitializeComponent();

            _info.SizeChanged += _info_SizeChanged;
        }

        void _info_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Resize();
        }

        internal void Add(Event lastEvent)
        {
            lock (_info.Events._syncObject)
            {
                _info.Events.Add(lastEvent);
            }
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

            HorizontalOffset = primaryScreen.WorkingArea.Right - 10.0;
            VerticalOffset = primaryScreen.WorkingArea.Bottom - 10.0;
        }
    }
}