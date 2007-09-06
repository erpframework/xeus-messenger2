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

            _border.PreviewMouseDown += RosterHeader_PreviewMouseDoubleClick;

            _openMargin = Margin;
        }

        void RosterHeader_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Margin = (Margin == _openMargin) ? _closedMargin : _openMargin;
        }

        public void SetSelfContact(SelfContact self)
        {
            _self.Content = self;
        }
    }
}