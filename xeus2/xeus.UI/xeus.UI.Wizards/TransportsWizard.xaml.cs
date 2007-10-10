using System.Windows.Controls;
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
        }
    }
}