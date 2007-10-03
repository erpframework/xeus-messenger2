using System;
using agsXMPP;
using agsXMPP.protocol.client ;
using System.Collections.Generic;

namespace xeus2.xeus.Core
{
    public class EventError : Event
	{
		private readonly Error _error = null ;

		public EventError( string message, Error error ) : base( message, EventSeverity.Error )
		{
			_error = error ;
		}

		public Error IqError
		{
			get
			{
				return _error ;
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
