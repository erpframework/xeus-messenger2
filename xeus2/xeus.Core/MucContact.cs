using agsXMPP ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class MucContact : NotifyInfoDispatcher
	{
		private Presence _presence ;

		public MucContact( Presence presence )
		{
			Presence = presence ;
		}

		public Jid Jid
		{
			get
			{
				return Presence.From ;
			}
		}

		public string Group
		{
			get
			{
				return Presence.MucUser.Item.Role.ToString() ;
			}
		}

		public string Nick
		{
			get
			{
				if ( string.IsNullOrEmpty( Presence.MucUser.Item.Nickname ) )
				{
					return Jid.Resource ;
				}
				else
				{
					return Presence.MucUser.Item.Nickname ;
				}
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

				NotifyPropertyChanged( "Nick" );
				NotifyPropertyChanged( "Group" );
			}
		}

		public override string ToString()
		{
			return Nick ;
		}
	}
}