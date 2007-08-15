using agsXMPP.protocol.x.muc;

namespace xeus2.xeus.Core
{
    internal class EventMucRoom : Event
    {
        public enum TypicalEvent
        {
            Undefined,
            Joined,
            Left,
            Kicked,
            Banned,
            NickChange
        }

        public MucRoom MucRoom
        {
            get
            {
                return _mucRoom;
            }
        }

        public TypicalEvent TypicalEventCode
        {
            get
            {
                return _typicalEvent;
            }
        }

        private readonly TypicalEvent _typicalEvent = TypicalEvent.Undefined;
        private readonly MucRoom _mucRoom;

        public User User
        {
            get
            {
                return _user;
            }
        }

        private readonly User _user;

        public EventMucRoom(TypicalEvent typicalEvent, MucRoom mucRoom, User user, string message)
            : base(message, EventSeverity.Info)
        {
            _typicalEvent = typicalEvent;
            _mucRoom = mucRoom;
            _user = user;
        }
    
        public EventMucRoom(MucRoom mucRoom, User user, string message)
            : base(message, EventSeverity.Info)
        {
            _mucRoom = mucRoom;
            _user = user;
        }
    }
}