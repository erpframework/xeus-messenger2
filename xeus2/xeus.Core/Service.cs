using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP ;
using agsXMPP.protocol.iq.disco ;

namespace xeus2.xeus.Core
{
	internal class Service
	{
		private DiscoInfo _discoInfo = null ;

		public Service( DiscoInfo discoInfo )
		{
			_discoInfo = discoInfo ;
		}

		public string Name
		{
			get
			{
				DiscoIdentity [] discoIdentities = _discoInfo.GetIdentities() ;

				if ( discoIdentities.Length > 0 )
				{
					return discoIdentities[ 0 ].Name ;
				}
				else
				{
					return "[Unknown service]" ;
				}
			}
		}

		public override string ToString()
		{
			return Name ;
		}
	}
}
