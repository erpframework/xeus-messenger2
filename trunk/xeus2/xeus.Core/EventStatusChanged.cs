using System ;
using agsXMPP.protocol.client ;

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
				return string.Format( "Contact {0} changed presence from '{1}' to '{2}'",
				                      Contact.DisplayName, OldPresence.Show, NewPresence.Show ) ;
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