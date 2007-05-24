using agsXMPP ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class MucContact
	{
		private Presence _presence ;

		public MucContact( Presence presence )
		{
			_presence = presence ;
		}

		public Jid Jid
		{
			get
			{
				return _presence.From ;
			}
		}

		public string Group
		{
			get
			{
				return _presence.MucUser.Item.Role.ToString() ;
			}
		}

		public string Nick
		{
			get
			{
				if ( string.IsNullOrEmpty( _presence.MucUser.Item.Nickname ) )
				{
					return Jid.Resource ;
				}
				else
				{
					return _presence.MucUser.Item.Nickname ;
				}
			}
		}

		public override string ToString()
		{
			return Nick ;
		}
	}
}