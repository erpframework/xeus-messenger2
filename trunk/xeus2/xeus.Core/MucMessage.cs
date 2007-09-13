using System;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    internal class MucMessage : MessageBase
    {
        private readonly agsXMPP.protocol.client.Message _message;
        private readonly MucContact _sender;

        public MucMessage(agsXMPP.protocol.client.Message message, MucContact sender)
        {
            _message = message;
            _sender = sender;

            if (_message.XDelay != null)
            {
                _dateTime = _message.XDelay.Stamp;
            }
        }

        public string Body
        {
            get
            {
                return _message.Body;
            }
        }

        public string Subject
        {
            get
            {
                return _message.Subject;
            }
        }

        public MucContact Contact
        {
            get
            {
                return _sender;
            }
        }

        public string Sender
        {
            get
            {
                return _message.From.Resource;
            }
        }

        public override string ToString()
        {
            return string.Format("({2}) {0}: {1}", Sender, Body, DateTime);
        }
    }
}