using System;
using System.Collections.Generic;
using System.Text;

namespace xeus2.xeus.Core
{
    internal class EventFatal : Event
    {
        public EventFatal(string message)
            : base(message, EventSeverity.Fatal)
        {
        }
    }
}
