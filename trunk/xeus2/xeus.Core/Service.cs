using System ;
using System.Collections.Generic ;
using agsXMPP ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.iq.disco ;
using xeus2.Properties ;
using Uri=agsXMPP.Uri;

namespace xeus2.xeus.Core
{
	internal enum MucFeature
	{
		muc_passwordprotected,
		muc_hidden,
		muc_temporary,
		muc_open,
		muc_unmoderated,
		muc_nonanonymous
	}

	internal class Service : NotifyInfoDispatcher
	{
		private DiscoInfo _discoInfo = null ;
		private DiscoItem _discoItem = null ;
		private bool _isDiscovered = false ;
		private IQ _errorIq = null ;

		private bool _isToplevel = false ;

		private Services _services = new Services() ;
		private ObservableCollectionDisp< Service > _commands = new ObservableCollectionDisp< Service >() ;

		public Service( DiscoItem discoItem, bool isToplevel )
		{
			_discoItem = discoItem ;
			_isToplevel = isToplevel ;
		}

		public string Key
		{
			get
			{
				return GetKey( _discoItem ) ;
			}
		}

		public static string GetKey( DiscoItem discoItem )
		{
			return string.Format( "{0}/{1}", discoItem.Jid, discoItem.Node ) ;
		}

		public Jid Jid
		{
			get
			{
				return DiscoItem.Jid ;
			}
		}

		public List< string > Categories
		{
			get
			{
				List< string > categories = new List< string >() ;

				if ( _discoInfo != null )
				{
					foreach ( DiscoIdentity identity in _discoInfo.GetIdentities() )
					{
						categories.Add( identity.Category ) ;
					}
				}

				return categories ;
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

				DiscoIdentity[] discoIdentities = DiscoInfo.GetIdentities() ;

				if ( !String.IsNullOrEmpty( DiscoItem.Name ) )
				{
					return DiscoItem.Name ;
				}
				else if ( discoIdentities.Length > 0 && !string.IsNullOrEmpty( discoIdentities[ 0 ].Name ) )
				{
					return discoIdentities[ 0 ].Name ;
				}
				else if ( !String.IsNullOrEmpty( Node ) )
				{
					return Node ;
				}

				return DiscoItem.Jid.ToString() ;
			}
		}

		public string Group
		{
			get
			{
				if ( DiscoInfo != null )
				{
					DiscoIdentity[] discoIdentities = DiscoInfo.GetIdentities() ;

					if ( discoIdentities.Length > 0 )
					{
						return discoIdentities[ 0 ].Category ;
					}
				}

				return Resources.Constant_General ;
			}
		}

		public Services Services
		{
			get
			{
				return _services ;
			}
		}

		public virtual DiscoInfo DiscoInfo
		{
			get
			{
				return _discoInfo ;
			}

			set
			{
				_discoInfo = value ;

				NotifyPropertyChanged( "DiscoInfo" ) ;
				NotifyPropertyChanged( "IsCommand" ) ;
				NotifyPropertyChanged( "Name" ) ;
				NotifyPropertyChanged( "Group" ) ;
				NotifyPropertyChanged( "IsRegistrable" ) ;
				NotifyPropertyChanged( "IsSearchable" ) ;
				NotifyPropertyChanged( "IsChatRoom" ) ;
				NotifyPropertyChanged( "IsMucPasswordProtected" ) ;
				NotifyPropertyChanged( "IsMucHidden" ) ;
				NotifyPropertyChanged( "IsMucTemporary" ) ;
				NotifyPropertyChanged( "IsMucOpen" ) ;
				NotifyPropertyChanged( "IsMucUnmoderated" ) ;
				NotifyPropertyChanged( "IsMucNonAnonymous" ) ;
				NotifyPropertyChanged( "Categories" ) ;
				NotifyPropertyChanged( "Type" ) ;
			}
		}

		public string Type
		{
			get
			{
				if ( _discoInfo != null
				     && _discoInfo.GetIdentities().Length > 0 )
				{
					return _discoInfo.GetIdentities()[ 0 ].Type ;
				}
				else
				{
					return null ;
				}
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
				bool notify = ( _isDiscovered != value ) ;

				_isDiscovered = value ;

				if ( notify )
				{
					NotifyPropertyChanged( "IsDiscovered" ) ;
				}
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

		public bool IsCommand
		{
			get
			{
				return ( _discoInfo != null && _discoInfo.HasFeature( Uri.COMMANDS ) ) ;
			}
		}

		public bool IsChatRoom
		{
			get
			{
				if ( _discoInfo == null )
				{
					return false ;
				}

				return _discoInfo.HasFeature( Uri.MUC ) ;
			}
		}


		public bool IsRegistrable
		{
			get
			{
				if ( _discoInfo == null || IsCommand )
				{
					return false ;
				}

				return _discoInfo.HasFeature( Uri.IQ_REGISTER ) ;
			}
		}

		public bool IsSearchable
		{
			get
			{
				if ( _discoInfo == null || IsCommand )
				{
					return false ;
				}

				return _discoInfo.HasFeature( Uri.IQ_SEARCH ) ;
			}
		}


		public bool IsMucPasswordProtected
		{
			get
			{
				return ( DiscoInfo != null && DiscoInfo.HasFeature( MucFeature.muc_passwordprotected.ToString() ) ) ;
			}
		}

		public bool IsMucHidden
		{
			get
			{
				return ( DiscoInfo != null && DiscoInfo.HasFeature( MucFeature.muc_hidden.ToString() ) ) ;
			}
		}

		public bool IsMucTemporary
		{
			get
			{
				return ( DiscoInfo != null && DiscoInfo.HasFeature( MucFeature.muc_temporary.ToString() ) ) ;
			}
		}

		public bool IsMucOpen
		{
			get
			{
				return ( DiscoInfo != null && DiscoInfo.HasFeature( MucFeature.muc_open.ToString() ) ) ;
			}
		}

		public bool IsMucUnmoderated
		{
			get
			{
				return ( DiscoInfo != null && DiscoInfo.HasFeature( MucFeature.muc_unmoderated.ToString() ) ) ;
			}
		}

		public bool IsMucNonAnonymous
		{
			get
			{
				return ( DiscoInfo != null && DiscoInfo.HasFeature( MucFeature.muc_nonanonymous.ToString() ) ) ;
			}
		}

		public bool IsToplevel
		{
			get
			{
				return _isToplevel ;
			}
		}

		public ObservableCollectionDisp< Service > Commands
		{
			get
			{
				return _commands ;
			}
		}

		public override string ToString()
		{
			return Name ;
		}
	}
}