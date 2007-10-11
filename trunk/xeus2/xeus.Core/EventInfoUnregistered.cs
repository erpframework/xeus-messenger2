namespace xeus2.xeus.Core
{
    internal class EventInfoUnregistered : Event
    {
        public EventInfoUnregistered(string message)
            : base(message, EventSeverity.Info)
        {
        }
    }
}
