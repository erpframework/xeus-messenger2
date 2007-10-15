using System.Windows;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for Groups.xaml
    /// </summary>
    public partial class Groups : UserControl
    {
        public Groups()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Group group = ((Button) sender).DataContext as Group;
            IContact contact = (IContact) DataContext;

            if (group == null)
            {
                Roster.Instance.SetContactGropup(contact, _groupName.Text);
            }
            else
            {
                Roster.Instance.SetContactGropup(contact, group.Name);
            }

            Window.GetWindow(this).Close();
        }
    }
}