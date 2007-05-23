using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP ;

namespace xeus2.xeus.Core
{
	internal class MucRoom
	{
		MucRoster _mucRoster = new MucRoster();
		private Service _service ;
		private XmppClientConnection _xmppClientConnection = null ;

		public MucRoom( Service service, XmppClientConnection xmppClientConnection )
		{
			_service = service ;
			_xmppClientConnection = xmppClientConnection ;
		}
	}
}
