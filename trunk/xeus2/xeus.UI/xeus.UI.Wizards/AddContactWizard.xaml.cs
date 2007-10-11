using System.Windows.Controls;
using agsXMPP;
using xeus2.xeus.Commands;

namespace xeus2.xeus.UI.xeus.UI.Wizards
{
    /// <summary>
    /// Interaction logic for AddContactWizard.xaml
    /// </summary>
    public partial class AddContactWizard : UserControl
    {
        public AddContactWizard()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Jid jid = new Jid(_userJid.Text.Trim());

            if (ContactCommands.AddContact.CanExecute(jid, null))
            {
                ContactCommands.AddContact.Execute(jid, null);
            }
        }
    }
}