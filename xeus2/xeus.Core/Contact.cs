using System ;
using agsXMPP ;
using agsXMPP.protocol.Base ;
using agsXMPP.protocol.client ;
using agsXMPP.Xml.Dom ;
using xeus2.Properties ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.Core
{
	public class Contact : NotifyInfoDispatcher, IContact
	{
		private RosterItem _rosterItem = null ;
		private Presence _presence = new Presence() ;
	    private string _customName;

	    public Contact( RosterItem rosterItem )
		{
			_rosterItem = rosterItem ;
		}

		public string DisplayName
		{
			get
			{
                if (!string.IsNullOrEmpty(CustomName))
                {
                    return CustomName;
                }
                else if (!string.IsNullOrEmpty(FullName))
                {
                    return FullName;
                }
                else if (!string.IsNullOrEmpty(NickName))
                {
                    return NickName;
                }

				return Jid.ToString() ;
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
				NotifyPropertyChanged( "StatusText" ) ;
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

		public string StatusText
		{
			get
			{
				return Presence.Show.ToString() ;
			}
		}

		public string FullName
		{
			get
			{
				// todo:
				return "dummy" ;
			}
		}

		public string NickName
		{
			get
			{
				if ( _presence.Nickname == null )
				{
					return String.Empty ;
				}

				return _presence.Nickname.ToString() ;
			}
		}

	    public string CustomName
	    {
	        get
	        {
	            return _customName;
	        }
	    }

	    public override string ToString()
		{
			return string.Format( "{0} / {1}", Jid, Presence.Status ) ;
		}
	}
}