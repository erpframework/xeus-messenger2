using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for StatusTextControl.xaml
    /// </summary>
    public partial class StatusTextControl : UserControl
    {
        private Popup _statusPopup;

        public StatusTextControl()
        {
            InitializeComponent();

            _content.Content = Account.Instance.Self;
        }

        private void _send_Click(object sender, RoutedEventArgs e)
        {
            SendStatusText();
        }

        private void _status_Click(object sender, RoutedEventArgs e)
        {
            _statusPopup.IsOpen = true;
        }

        private void _statusPopup_Initialized(object sender, EventArgs e)
        {
            _statusPopup = (Popup) sender;
        }

        private void _text_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendStatusText();
            }
        }

        private void SendStatusText()
        {
            Account.Instance.SendMyPresence();
        }
    }
}