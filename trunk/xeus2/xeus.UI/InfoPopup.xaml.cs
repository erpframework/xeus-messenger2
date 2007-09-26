using System;
using System.Windows.Forms;
using xeus2.xeus.Core;
using xeus2.xeus.Middle;
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

            Notification.Notifications.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Notifications_CollectionChanged);
        }

        void Notifications_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsOpen = (Notification.Notifications.Count > 0);
        }

        void _info_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
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

            HorizontalOffset = primaryScreen.WorkingArea.Right - 10.0;
            VerticalOffset = primaryScreen.WorkingArea.Bottom - 10.0;
        }
    }
}