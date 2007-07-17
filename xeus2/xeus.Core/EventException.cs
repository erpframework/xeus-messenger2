using System;
using System.Collections.Generic;
using System.Text;

namespace xeus2.xeus.Core
{
    internal class EventException : Event
    {
		private Exception _exception = null ;

        public EventException(string message, Exception exception)
            : base(message, EventSeverity.Exception)
		{
		    _exception = exception;
		}
    }
}
