using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for InfoPopup.xaml
    /// </summary>
    public partial class InfoPopup
    {
        public InfoPopup()
        {
            InitializeComponent();
        }

        internal void Display(Event eventInfo)
        {
            //IsOpen = false;

            Content = eventInfo.Message;

            //Point position = App.Current.MainWindow.PointToScreen(Mouse.GetPosition(App.Current.MainWindow));

           // Placement = PlacementMode.MousePoint;

            IsOpen = true;
        }
    }
}