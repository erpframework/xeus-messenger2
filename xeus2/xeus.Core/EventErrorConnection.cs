using System;

namespace xeus2.xeus.Core
{
    internal class EventErrorConnection : Event
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