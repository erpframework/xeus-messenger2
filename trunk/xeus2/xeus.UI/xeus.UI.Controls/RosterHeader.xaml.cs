using System.Windows;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for RosterHeader.xaml
    /// </summary>
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
    }
}