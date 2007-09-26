using System;
using System.Collections.Generic;
using System.Data;
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

        internal Message(IDataRecord reader)
        {
            _from = new Jid((string)reader["From"]);
            _to = new Jid((string)reader["To"]);
            _body = (string)reader["Body"];
            _dateTime = DateTime.FromBinary((Int64)reader["DateTime"]);
        }

        public Dictionary<string, object> GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("From", From.Bare);
            data.Add("To", To.Bare);
            data.Add("DateTime", DateTime.ToBinary());
            data.Add("Body", Body);
            data.Add("Type", "chat");

            return data;
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