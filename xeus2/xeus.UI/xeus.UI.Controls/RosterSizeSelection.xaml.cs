using System.Windows.Controls;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for RosterSizeSelection.xaml
    /// </summary>
    public partial class RosterSizeSelection : UserControl
    {
        public RosterSizeSelection()
        {
            InitializeComponent();
        }

        private void CloseParentPopup()
        {
            System.Windows.Controls.Primitives.Popup popup = Parent as System.Windows.Controls.Primitives.Popup;

            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CloseParentPopup();
        }
    }
}