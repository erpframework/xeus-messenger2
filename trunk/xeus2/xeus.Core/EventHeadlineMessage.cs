using agsXMPP;

namespace xeus2.xeus.Core
{
    public class EventHeadlineMessage : Event
    {
        private readonly HeadlineMessage _headline;
        private readonly Jid _sender;

        public EventHeadlineMessage(Jid jid, HeadlineMessage headline)
            : base(string.Format("New Headline from {0}", jid), EventSeverity.Info)
        {
            _sender = jid;
            _headline = headline;
        }

        public string Sender
        {
            get
            {
                return (string.IsNullOrEmpty(_sender.User) ? _sender.ToString() : _sender.User);
            }
        }

        public HeadlineMessage Headline
        {
            get
            {
                return _headline;
            }
        }

        public override string Message
        {
            get
            {
                return _headline.Body;
            }
        }
    }
}