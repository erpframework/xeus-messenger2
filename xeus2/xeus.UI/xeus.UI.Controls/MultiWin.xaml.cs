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

        private readonly string _key = null;
        private IFlyoutContainer _flyoutContainer = null;

        public enum MultiWinEvent
        {
            Close,
            Hide,
            Flyout,
            Show
        }

        public MultiWin()
        {
            InitializeComponent();

            Loaded += MultiWin_Loaded;
            Unloaded += MultiWin_Unloaded;
        }

        void MultiWin_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MultiWin_Loaded;
            Unloaded -= MultiWin_Unloaded;

            if (_flyoutContainer != null)
            {
                _flyoutContainer.CloseMe -= _flyoutContainer_CloseMe;
            }            
        }

        void MultiWin_Loaded(object sender, RoutedEventArgs e)
        {
            _flyoutContainer = ContentElement as IFlyoutContainer;

            if (_flyoutContainer != null)
            {
                _flyoutContainer.CloseMe += _flyoutContainer_CloseMe;
            }
        }

        public MultiWin(FrameworkElement element, string keyBase, string key) : this()
        {
            _key = WindowManager.MakeKey(keyBase, key);

            WindowManager.Approve(Key);

            ContentElement = element;
        }

        void _flyoutContainer_CloseMe()
        {
            OnClose(this, null);
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

        public string Key
        {
            get
            {
                return _key;
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

        public void ShowContainer()
        {
            if (OnMultiWinEvent != null)
            {
                OnMultiWinEvent(this, MultiWinEvent.Show);
            }
        }

        public virtual void Closing()
        {
        }
    }
}