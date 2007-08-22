using System.Windows;
using System.Windows.Controls;
using agsXMPP;
using agsXMPP.protocol.x.muc;
using xeus2.xeus.Core;
using xeus2.xeus.Middle;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for CreateMucRoom.xaml
    /// </summary>
    public partial class CreateMucRoom : UserControl
    {
        public CreateMucRoom()
        {
            InitializeComponent();
        }

        private void OnReserved(object sender, RoutedEventArgs args)
        {
        }

        private void OnJoin(object sender, RoutedEventArgs args)
        {
            MucInfo.Instance.MucLogin(new MucMark(_jid.Text));
        }
    }
}