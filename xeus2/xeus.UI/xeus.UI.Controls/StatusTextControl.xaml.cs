using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for StatusTextControl.xaml
    /// </summary>
    public partial class StatusTextControl : UserControl
    {
        public StatusTextControl()
        {
            InitializeComponent();

            _content.Content = Account.Instance.Self;
        }

        private Popup _statusPopup;

        private void _send_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _status_Click(object sender, RoutedEventArgs e)
        {
            _statusPopup.IsOpen = true;
        }

        private void _statusPopup_Initialized(object sender, System.EventArgs e)
        {
            _statusPopup = (Popup)sender;
        }
    }
}