using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Forms;
using xeus2.xeus.Commands;
using xeus2.xeus.Middle;
using MouseEventArgs=System.Windows.Input.MouseEventArgs;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for InfoPopup.xaml
    /// </summary>
    public partial class InfoPopup
    {
        private double _originalOpacity = 1.0;

        public InfoPopup()
        {
            InitializeComponent();

            _originalOpacity = _info.Opacity;
            _info.SizeChanged += _info_SizeChanged;

            Notification.Notifications.CollectionChanged += Notifications_CollectionChanged;
        }

        private void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsOpen = (Notification.Notifications.Count > 0);
        }

        private void _info_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resize();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _info.Clear();
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

        private void Popup_MouseEnter(object sender, MouseEventArgs e)
        {
            Notification.DontExpire = true;
            _info.Opacity = 1.0;
        }

        private void Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            Notification.DontExpire = false;
            _info.Opacity = _originalOpacity;
        }
    }
}