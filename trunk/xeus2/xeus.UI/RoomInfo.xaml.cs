using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for RoomInfo.xaml
    /// </summary>
    public partial class RoomInfo : BaseWindow
    {
        public const string _keyBase = "MUCRoomInfo";

        internal RoomInfo(Service service) : base(_keyBase, service.Jid.Bare)
        {
            InitializeComponent();

            DataContext = service;

            _detail.Setup(service);
        }
    }
}