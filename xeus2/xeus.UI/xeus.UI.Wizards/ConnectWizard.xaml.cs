using System.Windows.Controls;
using xeus2.Properties;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Wizards
{
    /// <summary>
    /// Interaction logic for ConnectWizard.xaml
    /// </summary>
    public partial class ConnectWizard : UserControl
    {
        public ConnectWizard()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Settings.Default.XmppPassword = _password.Password;

            if ((bool)_newAccount.IsChecked)
            {
                if (_password.Password != _confirmedPassword.Password)
                {
                    Middle.Alert.Instance.AlertOpen("Your passwords don't match.", null);
                    
                    return;
                }

                Settings.Default.XmppPassword = _password.Password;
                Account.Instance.Create();
            }
            else
            {
                Settings.Default.XmppPassword = _password.Password;
                Account.Instance.Login();
            }
        }

        private void _newAccount_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            HandleNewState();
        }

        private void _newAccount_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            HandleNewState();
        }

        void HandleNewState()
        {
            if ((bool)_newAccount.IsChecked)
            {
                _ok.Content = "Create and connect";
            }
            else
            {
                _ok.Content = "Connect";
            }
        }
    }
}