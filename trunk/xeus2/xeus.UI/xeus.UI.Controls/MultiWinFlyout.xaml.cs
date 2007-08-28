using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MultiWinFlyout.xaml
    /// </summary>

    public partial class MultiWinFlyout : System.Windows.Window
    {
        private readonly IMultiWinContainerProvider _multiWinContainerProvider;
        private readonly MultiWin _content;
        private readonly string _name;

        internal MultiWinFlyout(IMultiWinContainerProvider multiWinContainerProvider, MultiWin content, string name)
        {
            _multiWinContainerProvider = multiWinContainerProvider;
            _content = content;
            _name = name;

            Title = name;

            InitializeComponent();

            Loaded += new RoutedEventHandler(MultiWinFlyout_Loaded);
        }

        void MultiWinFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            _container.Child = _content;

            _content.DisplayControls = true;
            _content.OnMultiWinEvent += new MultiWin.NotifyMultiWin(content_OnMultiWinEvent);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _content.OnMultiWinEvent -= new MultiWin.NotifyMultiWin(content_OnMultiWinEvent);
            Loaded -= new RoutedEventHandler(MultiWinFlyout_Loaded);
        }

        void content_OnMultiWinEvent(MultiWin sender, MultiWin.MultiWinEvent multiWinEvent)
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
                        _multiWinContainerProvider.MultiTabControl.MultiWindows.Add(new MultiTabItem(_name,_content));
                        Close();
                        break;
                    }
                
            }
        }
    }
}