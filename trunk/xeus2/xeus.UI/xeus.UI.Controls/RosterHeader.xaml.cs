using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for RosterHeader.xaml
    /// </summary>
    /// 
    public partial class RosterHeader : UserControl
    {
        public RosterHeader()
        {
            InitializeComponent();
        }

        public void SetSelfContact(SelfContact self)
        {
            _self.Content = self;
        }

        private void _self_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Commands.ContactCommands.DisplayVCard.CanExecute(_self.Content, null))
            {
                Commands.ContactCommands.DisplayVCard.Execute(_self.Content, null);
            }
        }
    }
}