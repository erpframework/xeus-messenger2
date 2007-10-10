using xeus2.xeus.Core;

namespace xeus2.xeus.Middle
{
    internal class KickReason
    {
        private static KickReason _instance = new KickReason();

        public static KickReason Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Kick(MucContact mucContact)
        {
            UI.SingleValueBox kickReason = new UI.SingleValueBox("Kick reason", "Kick user");
            kickReason.DataContext = mucContact;

            kickReason.Activate();
            if ((bool) kickReason.ShowDialog())
            {
                mucContact.MucRoom.Kick(mucContact, kickReason.Text);
            }
        }
    }
}