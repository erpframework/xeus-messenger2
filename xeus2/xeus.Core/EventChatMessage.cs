namespace xeus2.xeus.Core
{
    internal class EventChatMessage : Event
    {
        private readonly Contact _contact;
        private readonly Message _message;
        private readonly bool _isChatWindowAvailable;

        public EventChatMessage(Contact contact, Message message, bool isChatWindowAvailable)
            : base(string.Format("New Message from {0}", contact.DisplayName), EventSeverity.Info)
        {
            _contact = contact;
            _message = message;
            _isChatWindowAvailable = isChatWindowAvailable;
        }

        public Contact Contact
        {
            get
            {
                return _contact;
            }
        }

        public Message ChatMessage
        {
            get
            {
                return _message;
            }
        }

        public bool IsChatWindowAvailable
        {
            get
            {
                return _isChatWindowAvailable;
            }
        }
    }
}