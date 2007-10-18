namespace xeus2.xeus.Core
{
    internal class EventErrorFileTransfer : Event
    {
        public EventErrorFileTransfer(string message)
            : base(message, EventSeverity.Error)
        {
        }
    }
}
