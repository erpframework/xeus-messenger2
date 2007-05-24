using agsXMPP ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.x.muc ;
using xeus2.Properties ;

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

		public Role Role
		{
			get
			{
				return Presence.MucUser.Item.Role ;
			}
		}

		public string MucStatusCode
		{
			get
			{
				if ( _presence.MucUser.Status != null )
				{
					return  _presence.MucUser.Status.Code.ToString() ;
				}
				else
				{
					return null ;
				}
			}
		}

		public string MucStatusCodeText
		{
			get
			{
				if ( !string.IsNullOrEmpty( _presence.Status ) )
				{
					return MucStatusCodeTexts.GetCodeText( _presence.MucUser.Status ) ;
				}
				else
				{
					return null ;
				}
			}
		}

		public string StatusText
		{
			get
			{
				return Presence.Status ;
			}
		}

		public ShowType Show
		{
			get
			{
				return Presence.Show ;
			}
		}

		public Affiliation Affiliation
		{
			get
			{
				return Presence.MucUser.Item.Affiliation ;
			}
		}

		public string Group
		{
			get
			{
				switch ( Role )
				{
					case Role.visitor:
						{
							return Resources.Role_Visitor ;
						}
					case Role.participant:
						{
							return Resources.Role_Participant ;
						}
					case Role.moderator:
						{
							return Resources.Role_Moderator ;
						}
					default:
						{
							return Resources.Role_None ;
						}
				}
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

				NotifyPropertyChanged( "Nick" ) ;
				NotifyPropertyChanged( "Group" ) ;
				NotifyPropertyChanged( "Role" ) ;
				NotifyPropertyChanged( "Affiliation" ) ;
				NotifyPropertyChanged( "StatusText" ) ;
				NotifyPropertyChanged( "Show" ) ;
			}
		}

		public override string ToString()
		{
			return Nick ;
		}
	}
}