using agsXMPP.protocol.iq.disco ;
using xeus2.Properties ;

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
				DiscoIdentity[] discoIdentities = _discoInfo.GetIdentities() ;

				if ( discoIdentities.Length > 0 )
				{
					return discoIdentities[ 0 ].Name ;
				}
				else
				{
					return Resources.Constant_UnknownService ;
				}
			}
		}

		public string Group
		{
			get
			{
				DiscoIdentity[] discoIdentities = _discoInfo.GetIdentities() ;

				if ( discoIdentities.Length > 0 )
				{
					return discoIdentities[ 0 ].Category ;
				}
				else
				{
					return Resources.Constant_General ;
				}
			}
		}

		public override string ToString()
		{
			return Name ;
		}
	}
}