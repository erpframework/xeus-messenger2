using agsXMPP.protocol.x.muc;

namespace xeus2.xeus.Core
{
        public enum TypicalEvent
        {
            Undefined,
            Joined,
            Left,
            Kicked,
            Banned,
            NickChange,
            Error,
            RoomCreated,
            RoomPrepared
        }
    
    internal class EventMucRoom : Event
    {
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

        public MucContact MucContact
        {
            get
            {
                return _mucContact;
            }
        }

        private MucContact _mucContact;

        public EventMucRoom(TypicalEvent typicalEvent, MucRoom mucRoom, User user, string message)
            : base(message, EventSeverity.Info)
        {
            _typicalEvent = typicalEvent;
            _mucRoom = mucRoom;
            _user = user;
        }

        public EventMucRoom(TypicalEvent typicalEvent, MucRoom mucRoom, MucContact mucContact, string message)
            : base(message, EventSeverity.Info)
        {
            _typicalEvent = typicalEvent;
            _mucRoom = mucRoom;
            _mucContact = mucContact;
        }

        public EventMucRoom(MucRoom mucRoom, User user, string message)
            : base(message, EventSeverity.Info)
        {
            _mucRoom = mucRoom;
            _user = user;
        }
    }
}