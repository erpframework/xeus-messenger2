using System;
using agsXMPP.protocol.client;
using xeus2.Properties;

namespace xeus2.xeus.Core
{
    public class EventPresenceChanged : Event
    {
        private readonly Contact _contact;
        private readonly Presence _newPresence;
        private readonly Presence _oldPresence;

        public EventPresenceChanged(Contact contact, Presence oldPresence, Presence newPresence)
            : base(String.Empty, EventSeverity.Info)
        {
            _contact = contact;
            _oldPresence = oldPresence;
            _newPresence = newPresence;

            Expiration = DateTime.Now.AddSeconds(Settings.Default.UI_Notify_Presence_Exp);
        }

        public override string Message
        {
            get
            {
                if (OldPresence == null)
                {
                    return string.Format(Resources.Event_PresenceChange,
                                         Contact.DisplayName, "No presence", NewPresence.Show);
                }
                else if (NewPresence == null)
                {
                    return string.Format(Resources.Event_PresenceChange,
                                         Contact.DisplayName, OldPresence.Show, "No presence");
                }
                else
                {
                    return string.Format(Resources.Event_PresenceChange,
                                         Contact.DisplayName, OldPresence.Show, NewPresence.Show);
                }
            }
        }

        public Contact Contact
        {
            get
            {
                return _contact;
            }
        }

        public Presence OldPresence
        {
            get
            {
                return _oldPresence;
            }
        }

        public Presence NewPresence
        {
            get
            {
                return _newPresence;
            }
        }

        public bool IsGoingOnline()
        {
            return ((OldPresence == null
                     || OldPresence.Type == PresenceType.unavailable)
                    && NewPresence.Type == PresenceType.available);
        }

        public bool IsGoingOffline()
        {
            return (OldPresence != null
                    && OldPresence.Type == PresenceType.available
                    && NewPresence.Type == PresenceType.unavailable);
        }

        public bool IsChangingShowType()
        {
            return (OldPresence != null
                    && OldPresence.Type == PresenceType.available
                    && NewPresence.Type == PresenceType.available
                    && NewPresence.Show != OldPresence.Show);
        }
    }
}