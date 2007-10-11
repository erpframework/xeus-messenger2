using System;
using System.Collections.Generic;
using agsXMPP.protocol.client;
using xeus2.Properties;

namespace xeus2.xeus.Core
{
    public class EventError : Event
    {
        private readonly Error _error = null;

        public EventError(string message, Error error) : base(message, EventSeverity.Error)
        {
            _error = error;

            Expiration = DateTime.Now.AddSeconds(Settings.Default.UI_Notify_Error_Exp);
        }

        public Error IqError
        {
            get
            {
                return _error;
            }
        }

        public override string Message
        {
            get
            {
                if (_error != null)
                {
                    return string.Format("{0}\nCode: '{1}'", base.Message, _error.Code);
                }
                else
                {
                    return base.Message;
                }
            }
        }

        public Dictionary<string, object> GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (_error != null)
            {
                data.Add("Iq", _error.ToString());
            }

            data.Add("DateTime", Time.ToBinary());
            data.Add("Message", base.Message);

            return data;
        }
    }
}