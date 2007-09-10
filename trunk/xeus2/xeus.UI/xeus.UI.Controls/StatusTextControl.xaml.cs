using System.Windows;
using System.Windows.Controls;

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
        }

        private void _send_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _status_Click(object sender, RoutedEventArgs e)
        {
            _statusPopup.IsOpen = true;
        }
    }
}