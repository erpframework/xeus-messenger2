using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for VCardControl.xaml
    /// </summary>
    public partial class VCardControl : UserControl
    {
        public VCardControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.ToString());
            }

            catch
            {
            }
        }
    }
}