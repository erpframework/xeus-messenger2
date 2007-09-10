using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for StatusesControl.xaml
    /// </summary>
    public partial class StatusesControl : UserControl
    {
        public StatusesControl()
        {
            InitializeComponent();
        }

        private void CloseParentPopup()
        {
            Popup popup = Parent as Popup;

            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseParentPopup();
        }
    }
}