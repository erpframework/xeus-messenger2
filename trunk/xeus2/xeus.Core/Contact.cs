using agsXMPP ;
using agsXMPP.protocol.Base ;
using agsXMPP.protocol.client ;
using agsXMPP.Xml.Dom ;
using xeus2.Properties ;

namespace xeus2.xeus.Core
{
	internal class Contact : NotifyInfoDispatcher, IContact
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

		public string Group
		{
			get
			{
				foreach ( Element element in _rosterItem.GetGroups() )
				{
					return element.Value ;
				}

				return Resources.Constant_General ;
			}
		}

		public override string ToString()
		{
			return string.Format( "{0} / {1}", Jid, Presence.Status ) ;
		}
	}
}