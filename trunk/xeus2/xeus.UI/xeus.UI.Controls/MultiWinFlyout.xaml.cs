using System;
using System.Windows;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MultiWinFlyout.xaml
    /// </summary>
    public partial class MultiWinFlyout : Window
    {
        private readonly MultiWin _content;
        private readonly IMultiWinContainerProvider _multiWinContainerProvider;
        private readonly string _name;

        internal MultiWinFlyout(IMultiWinContainerProvider multiWinContainerProvider, MultiWin content, string name)
        {
            _multiWinContainerProvider = multiWinContainerProvider;
            _content = content;
            _name = name;

            Title = name;

            InitializeComponent();

            Loaded += MultiWinFlyout_Loaded;
        }

        private void MultiWinFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            _container.Child = _content;

            _content.DisplayControls = true;
            _content.OnMultiWinEvent += content_OnMultiWinEvent;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _content.OnMultiWinEvent -= content_OnMultiWinEvent;
            Loaded -= MultiWinFlyout_Loaded;
        }

        private void content_OnMultiWinEvent(MultiWin sender, MultiWin.MultiWinEvent multiWinEvent)
        {
            switch (multiWinEvent)
            {
                case MultiWin.MultiWinEvent.Close:
                    {
                        Close();
                        break;
                    }
                case MultiWin.MultiWinEvent.Hide:
                    {
                        WindowState = WindowState.Minimized;
                        break;
                    }
                case MultiWin.MultiWinEvent.Flyout:
                    {
                        _container.Child = null;
                        _multiWinContainerProvider.MultiTabControl.MultiWindows.Add(new MultiTabItem(_name, _content));
                        Close();
                        break;
                    }
            }
        }
    }
}