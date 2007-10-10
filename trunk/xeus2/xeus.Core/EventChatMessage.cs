namespace xeus2.xeus.Core
{
    public class EventChatMessage : Event
    {
        private readonly Contact _contact;
        private readonly Message _chatMessage;
        private readonly bool _isChatWindowAvailable;

        public EventChatMessage(Contact contact, Message message, bool isChatWindowAvailable)
            : base(string.Format("New Message from {0}", contact.DisplayName), EventSeverity.Info)
        {
            _contact = contact;
            _chatMessage = message;
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
                return _chatMessage;
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