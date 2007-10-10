using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class ChangeMucContactNick
    {
        private static readonly ChangeMucContactNick _instance = new ChangeMucContactNick();

        public static ChangeMucContactNick Instance
        {
            get
            {
                return _instance;
            }
        }

        public void DisplayNick(MucRoom mucRoom)
        {
            ChangeNickInRoom room = new ChangeNickInRoom(mucRoom);

            room.Activate();
            if ((bool) room.ShowDialog())
            {
                mucRoom.ChangeNickname(room.Nick);
            }
        }
    }
}