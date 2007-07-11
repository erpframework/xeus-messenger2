using System;
using System.Collections.Generic;
using System.Text;
using xeus2.xeus.Core;

namespace xeus2.xeus.Middle
{
    internal class ChangeMucTopic
    {
        private static ChangeMucTopic _instance = new ChangeMucTopic();

        public static ChangeMucTopic Instance
        {
            get
            {
                return _instance;
            }
        }

        public void DisplayTopic(MucRoom mucRoom)
        {
            UI.RoomTopic topic = new UI.RoomTopic();
            topic.DataContext = mucRoom;

            if ((bool)topic.ShowDialog())
            {
                mucRoom.ChangeMucTopic(topic.Topic);
            }
        }
    }
}
