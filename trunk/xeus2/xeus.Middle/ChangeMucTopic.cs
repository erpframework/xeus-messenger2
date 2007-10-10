using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class ChangeMucTopic
    {
        private static readonly ChangeMucTopic _instance = new ChangeMucTopic();

        public static ChangeMucTopic Instance
        {
            get
            {
                return _instance;
            }
        }

        public void DisplayTopic(MucRoom mucRoom)
        {
            RoomTopic topic = new RoomTopic(mucRoom);

            topic.Activate();
            if ((bool) topic.ShowDialog())
            {
                mucRoom.ChangeMucTopic(topic.Topic);
            }
        }
    }
}