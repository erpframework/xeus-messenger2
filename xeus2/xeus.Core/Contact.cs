using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP ;
using agsXMPP.protocol.Base ;

namespace xeus2.xeus.Core
{
	internal class Contact : NotifyInfoDispatcher
	{
		private RosterItem _rosterItem = null ;

		public Contact( RosterItem rosterItem )
		{
			_rosterItem = rosterItem ;
		}

		public Jid Jid
		{
			get
			{
				return _rosterItem.Jid ;
			}
		}

		public override string ToString()
		{
			return Jid.ToString() ;
		}
	}
}
