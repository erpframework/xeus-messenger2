using System;
using System.Collections.Generic;
using System.Text;

namespace xeus2.xeus.Core
{
	internal class EventError : Event
	{
		public EventError( string message ) : base( message, EventSeverity.Error )
		{
		}
	}
}
