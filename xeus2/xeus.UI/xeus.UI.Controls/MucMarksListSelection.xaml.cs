using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using xeus2.xeus.Commands;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MucMarksListSelection.xaml
    /// </summary>
    public partial class MucMarksListSelection : UserControl
    {
        public MucMarksListSelection()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            if (e.AddedItems.Count > 0)
            {
                ServiceCommands.JoinMuc.Execute(e.AddedItems[0], null);

                _list.SelectedItem = null;
                
                CloseParentPopup();
            }
        }

        private void CloseParentPopup()
        {
            Popup popup = Parent as Popup;

            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }
    }
}