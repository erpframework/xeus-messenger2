using System;
using System.Windows;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MultiWinFlyout.xaml
    /// </summary>
    public partial class MultiWinFlyout : BaseWindow
    {
        private readonly MultiWin _content;
        private readonly IMultiWinContainerProvider _multiWinContainerProvider;
        private readonly string _name;
        private IFlyoutContainer _flyoutContainer = null;

        internal MultiWinFlyout(IMultiWinContainerProvider multiWinContainerProvider, MultiWin content, string name, string key)
            : base(key)
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

            _flyoutContainer = ((MultiWin)(_container.Child)).ContentElement as IFlyoutContainer;

            if (_flyoutContainer != null)
            {
                _flyoutContainer.CloseMe += _flyoutContainer_CloseMe;
            }

            _content.DisplayControls = true;
            _content.OnMultiWinEvent += content_OnMultiWinEvent;
        }

        void _flyoutContainer_CloseMe()
        {
            Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_closingCompletely)
            {
                if (_flyoutContainer != null)
                {
                    _flyoutContainer.Closing();
                    _flyoutContainer.CloseMe -= _flyoutContainer_CloseMe;
                    _flyoutContainer = null;
                }
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _content.OnMultiWinEvent -= content_OnMultiWinEvent;

            base.OnClosed(e);
        }

        private bool _closingCompletely = true;

        private void content_OnMultiWinEvent(MultiWin sender, MultiWin.MultiWinEvent multiWinEvent)
        {
            switch (multiWinEvent)
            {
                case MultiWin.MultiWinEvent.Close:
                    {
                        _closingCompletely = true;
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
                        _closingCompletely = false;
                        _container.Child = null;

                        Close();
                        
                        _multiWinContainerProvider.MultiTabControl.MultiWindows.Add(new MultiTabItem(_name, _content));
                        break;
                    }
            }
        }
    }
}