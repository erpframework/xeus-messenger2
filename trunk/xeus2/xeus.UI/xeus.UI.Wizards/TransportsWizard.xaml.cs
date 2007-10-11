using System.Windows.Controls;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Wizards
{
    /// <summary>
    /// Interaction logic for TransportsWizard.xaml
    /// </summary>
    public partial class TransportsWizard : UserControl
    {
        public TransportsWizard()
        {
            DataContext = Services.Instance;

            InitializeComponent();

            if (Services.Instance.Transports.Count == 0
                && ServiceCommands.DiscoveryServices.CanExecute(null, null))
            {
                ServiceCommands.DiscoveryServices.Execute(null, null);
            }
        }
    }
}