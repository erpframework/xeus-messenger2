namespace xeus2.xeus.Core
{
    internal class EventInfoFileTransfer : Event
    {
        public EventInfoFileTransfer(string message)
            : base(message, EventSeverity.Info)
        {
        }
    }
}
