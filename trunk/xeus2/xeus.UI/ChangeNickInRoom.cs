using System.Windows;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for RoomTopic.xaml
    /// </summary>
    public partial class ChangeNickInRoom : BaseWindow
    {
        public const string _keyBase = "ChangeNickInMUCRoom";

        internal ChangeNickInRoom(MucRoom mucRoom) : base(_keyBase, mucRoom.Service.Jid.Bare)
        {
            InitializeComponent();

            DataContext = mucRoom;
        }

        public string Nick
        {
            get
            {
                return _nick.Text;
            }
        }

        protected void OnChange(object sender, RoutedEventArgs eventArgs)
        {
            DialogResult = true;
            Close();
        }
    }
}