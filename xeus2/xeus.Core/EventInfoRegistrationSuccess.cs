namespace xeus2.xeus.Core
{
    internal class EventInfoRegistrationSuccess : Event
    {
        public EventInfoRegistrationSuccess(string message)
            : base(message, EventSeverity.Info)
        {
        }
    }
}
