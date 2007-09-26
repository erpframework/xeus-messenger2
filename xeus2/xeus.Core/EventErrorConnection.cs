using System;

namespace xeus2.xeus.Core
{
    public class EventErrorConnection : Event
    {
        private readonly Exception _exception;

        public EventErrorConnection(string message, Exception ex)
            : base(message, EventSeverity.Error)
        {
            _exception = ex;
        }

        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }
    }
}