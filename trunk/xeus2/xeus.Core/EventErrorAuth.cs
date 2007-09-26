namespace xeus2.xeus.Core
{
    internal class EventErrorAuth : Event
    {
        public EventErrorAuth(string message)
            : base(message, EventSeverity.Error)
        {
        }
    }
}