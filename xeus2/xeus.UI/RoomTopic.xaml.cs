using System.Windows;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for RoomTopic.xaml
    /// </summary>
    public partial class RoomTopic : BaseWindow
    {
        public const string _keyBase = "MUCRoomTopic";

        internal RoomTopic(MucRoom mucRoom) : base(_keyBase, mucRoom.Service.Jid.Bare)
        {
            InitializeComponent();

            DataContext = mucRoom;
        }

        public string Topic
        {
            get
            {
                return _topic.Text;
            }
        }

        protected void OnChange(object sender, RoutedEventArgs eventArgs)
        {
            DialogResult = true;
            Close();
        }
    }
}