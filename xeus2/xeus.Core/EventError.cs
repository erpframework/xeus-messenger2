using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class EventError : Event
	{
		private Error _error = null ;

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

		public override string ToString()
		{
			return base.ToString();
		}
	}
}
