using System.Windows;
using System.Windows.Controls;
using agsXMPP;
using agsXMPP.protocol.iq.disco;
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

        private void OnJoin(object sender, RoutedEventArgs args)
        {
            /*
            DiscoItem discoItem = new DiscoItem();
            discoItem.Jid = new Jid(_jid.Text);

            Service service = new Service(discoItem, false);
            MucInfo.Instance.MucLogin(service, null);
             */

            Account.Instance.JoinMuc(_jid.Text);
        }
    }
}