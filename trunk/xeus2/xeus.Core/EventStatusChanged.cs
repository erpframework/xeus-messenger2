using System ;
using System.Globalization ;
using System.Threading ;
using agsXMPP.protocol.client ;
using xeus2.Properties ;

namespace xeus2.xeus.Core
{
	internal class EventPresenceChanged : Event
	{
		private readonly Contact _contact ;
		private readonly Presence _oldPresence ;
		private readonly Presence _newPresence ;

		public EventPresenceChanged( Contact contact, Presence oldPresence, Presence newPresence )
			: base( String.Empty, EventSeverity.Info )
		{
			_contact = contact ;
			_oldPresence = oldPresence ;
			_newPresence = newPresence ;
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
				return _contact ;
			}
		}

		public Presence OldPresence
		{
			get
			{
				return _oldPresence ;
			}
		}

		public Presence NewPresence
		{
			get
			{
				return _newPresence ;
			}
		}
	}
}