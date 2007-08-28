using System.Windows;
using System.Windows.Controls;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MultiWin.xaml
    /// </summary>
    public partial class MultiWin : UserControl
    {
        public delegate void NotifyMultiWin(MultiWin sender, MultiWinEvent multiWinEvent);
        public event NotifyMultiWin OnMultiWinEvent;

        public enum MultiWinEvent
        {
            Close,
            Hide,
            Flyout
        }

        public MultiWin()
        {
            InitializeComponent();
        }

        public MultiWin(UIElement element) : this()
        {
            ContentElement = element;
        }

        public UIElement ContentElement
        {
            get
            {
                return _container.Child;
            }
            set
            {
                _container.Child = value;
            }
        }

        void OnClose(object sender, RoutedEventArgs args)
        {
            if (OnMultiWinEvent != null)
            {
                OnMultiWinEvent(this, MultiWinEvent.Close);
            }
        }

        void OnHide(object sender, RoutedEventArgs args)
        {
            if (OnMultiWinEvent != null)
            {
                OnMultiWinEvent(this, MultiWinEvent.Hide);
            }
        }

        void OnFlyOut(object sender, RoutedEventArgs args)
        {
            if (OnMultiWinEvent != null)
            {
                OnMultiWinEvent(this, MultiWinEvent.Flyout);
            }
        }
    }
}