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
        readonly Thickness _openMargin ;
        readonly Thickness _closedMargin = new Thickness(4,-40,4,4);
        
        public RosterHeader()
        {
            InitializeComponent();

            _openMargin = Margin;
        }

        public void SetSelfContact(SelfContact self)
        {
            _self.Content = self;
        }
    }
}