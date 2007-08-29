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

        public MultiWin(FrameworkElement element) : this()
        {
            ContentElement = element;
        }

        public double ContentMinWidth
        {
            get
            {
                if (ContentElement == null)
                {
                    return 0.0;
                }
                else
                {
                    return ContentElement.MinWidth;
                }
            }
        }

        public FrameworkElement ContentElement
        {
            get
            {
                return _container.Child as FrameworkElement;
            }
            set
            {
                _container.Child = value;
            }
        }

        public bool DisplayControls
        {
            get
            {
                return (_controls.Visibility == Visibility.Visible);
            }

            set
            {
                _controls.Visibility = (value) ? Visibility.Visible : Visibility.Hidden;
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