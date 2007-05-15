using System ;
using agsXMPP ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.iq.disco ;
using xeus2.Properties ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.Core
{
	internal class Service : NotifyInfoDispatcher
	{
		private DiscoInfo _discoInfo = null ;
		private DiscoItem _discoItem = null ;
		private Services _services = new Services();
		private bool _isDiscovered = false ;
		private IQ _errorIq = null ;

		public Service( DiscoItem discoItem )
		{
			_discoItem = discoItem ;
		}

		public Service( DiscoInfo discoInfo )
		{
			DiscoInfo = discoInfo ;
		}

		public Jid Jid
		{
			get
			{
				return DiscoItem.Jid ;
			}
		}

		public string Name
		{
			get
			{
				if ( DiscoInfo == null )
				{
					if ( ErrorIq == null )
					{
						return DiscoItem.Jid.ToString() ;
					}
					else
					{
						return string.Format( Resources.Error_CodeMsg, ErrorIq.Error.Code, ErrorIq.Error.LastNode.Value ) ;
					}
				}

				string name ;

				DiscoIdentity[] discoIdentities = DiscoInfo.GetIdentities() ;

				if ( DiscoItem.Name != null && DiscoItem.Name != String.Empty )
				{
					name = DiscoItem.Name ;
				}
				else if ( discoIdentities.Length > 0 )
				{
					name = discoIdentities[ 0 ].Name ;
				}
				else
				{
					name = Resources.Constant_UnknownService ;
				}

				if ( !String.IsNullOrEmpty( Node ) )
				{
					return Node ;
				}

				return name ;
			}
		}

		public string Group
		{
			get
			{
				DiscoIdentity[] discoIdentities = DiscoInfo.GetIdentities() ;

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

		public Services Services
		{
			get
			{
				return _services ;
			}
		}

		public DiscoInfo DiscoInfo
		{
			get
			{
				return _discoInfo ;
			}
			set
			{
				_discoInfo = value ;

				NotifyPropertyChanged( "Name" ) ;
			}
		}

		public bool IsDiscovered
		{
			get
			{
				return _isDiscovered ;
			}
			set
			{
				_isDiscovered = value ;
			}
		}

		public IQ ErrorIq
		{
			get
			{
				return _errorIq ;
			}
			set
			{
				_errorIq = value ;
			}
		}

		public string Node
		{
			get
			{
				return DiscoItem.Node ;
			}
		}

		public DiscoItem DiscoItem
		{
			get
			{
				return _discoItem ;
			}
		}

		public override string ToString()
		{
			return Name ;
		}
	}
}