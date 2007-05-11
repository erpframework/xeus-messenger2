using System ;
using System.Collections.Generic ;
using agsXMPP ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class MultiContact : IContact
	{
		private Dictionary< Jid, IContact > _contacts = new Dictionary< Jid, IContact >() ;
		private IContact _defaultContact = null ;

		public Jid Jid
		{
			get
			{
				return _defaultContact.Jid ;
			}
		}

		public Presence Presence
		{
			get
			{
				return _defaultContact.Presence ;
			}
		}

		public string DisplayName
		{
			get
			{
				return _defaultContact.DisplayName ;
			}
		}

		public string Group
		{
			get
			{
				return _defaultContact.Group ;
			}
		}

		public string StatusText
		{
			get
			{
				return _defaultContact.StatusText ;
			}
		}

		public string FullName
		{
			get
			{
				return _defaultContact.FullName ;
			}
		}

		public string NickName
		{
			get
			{
				return _defaultContact.NickName ;
			}
		}

		public Dictionary< Jid, IContact > Contacts
		{
			get
			{
				return _contacts ;
			}
		}

		public IContact DefaultContact
		{
			get
			{
				return _defaultContact ;
			}
		}
	}
}