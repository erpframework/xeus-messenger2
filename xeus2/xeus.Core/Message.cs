using agsXMPP;

namespace xeus2.xeus.Core
{
    internal class Message : MessageBase
    {
        private readonly string _body = string.Empty;
        private readonly Jid _from;
        private readonly Jid _to;

        internal Message(agsXMPP.protocol.client.Message message)
        {
            _from = message.From;
            _to = message.To;
            _body = message.Body;
        }

        public Jid From
        {
            get
            {
                return _from;
            }
        }

        public Jid To
        {
            get
            {
                return _to;
            }
        }

        public string Body
        {
            get
            {
                return _body;
            }
        }
    }
}