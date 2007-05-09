using agsXMPP ;
using agsXMPP.protocol.Base ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class Contact : NotifyInfoDispatcher
	{
		private RosterItem _rosterItem = null ;
		private Presence _presence = new Presence() ;

		public Contact( RosterItem rosterItem )
		{
			_rosterItem = rosterItem ;
		}

		public string DisplayName
		{
			get
			{
				// todo:
				return "Disp. name" ;
			}
		}

		public Jid Jid
		{
			get
			{
				return _rosterItem.Jid ;
			}
		}

		public Presence Presence
		{
			get
			{
				return _presence ;
			}

			set
			{
				_presence = value ;

				NotifyPropertyChanged( "Presence" ) ;
			}
		}

		public override string ToString()
		{
			return string.Format( "{0} / {1}", Jid, Presence.Status ) ;
		}
	}
}