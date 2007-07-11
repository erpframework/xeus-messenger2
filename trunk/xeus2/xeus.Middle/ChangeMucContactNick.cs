using System;
using System.Collections.Generic;
using System.Text;
using xeus2.xeus.Core;

namespace xeus2.xeus.Middle
{
    internal class ChangeMucContactNick
    {
        private static ChangeMucContactNick _instance = new ChangeMucContactNick();

        public static ChangeMucContactNick Instance
        {
            get
            {
                return _instance;
            }
        }

        public void DisplayNick(MucRoom mucRoom)
        {
            UI.ChangeNickInRoom room = new UI.ChangeNickInRoom();
            room.DataContext = mucRoom;

            if ((bool)room.ShowDialog())
            {
                mucRoom.ChangeNickname(room.Nick);
            }
        }
    }
}
