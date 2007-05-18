using System ;
using agsXMPP ;
using agsXMPP.net ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.extensions.commands ;
using agsXMPP.protocol.iq.disco ;
using agsXMPP.protocol.iq.register ;
using agsXMPP.protocol.iq.roster ;
using agsXMPP.protocol.iq.search ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;
using xeus2.Properties ;
using xeus2.xeus.Middle ;

namespace xeus2.xeus.Core
{
	internal class Account : NotifyInfoDispatcher
	{
		private static Account _instance = new Account() ;

		private XmppClientConnection _xmppConnection = new XmppClientConnection() ;
		private bool _isLogged = false ;

		public static Account Instance
		{
			get
			{
				return _instance ;
			}
		}

		public void Open()
		{
			if ( IsLogged )
			{
				throw new XeusException( "Connection is already open" ) ;
			}

			_xmppConnection.UseCompression = true ;
			_xmppConnection.Priority = Settings.Default.XmppPriority ;
			_xmppConnection.AutoResolveConnectServer = true ;

			_xmppConnection.ConnectServer = null ;
			_xmppConnection.Resource = Settings.Default.XmppResource ;
			_xmppConnection.SocketConnectionType = SocketConnectionType.Direct ;
			_xmppConnection.UseStartTLS = true ;

			_xmppConnection.AutoRoster = true ;
			_xmppConnection.AutoAgents = true ;

			_xmppConnection.Username = Settings.Default.XmppUserName ;
			_xmppConnection.Password = Settings.Default.XmppPassword ;
			_xmppConnection.Server = Settings.Default.XmppServer ;

			_xmppConnection.OnClose += new ObjectHandler( _xmppConnection_OnClose ) ;
			_xmppConnection.OnLogin += new ObjectHandler( _xmppConnection_OnLogin ) ;
			_xmppConnection.OnRosterItem += new XmppClientConnection.RosterHandler( _xmppConnection_OnRosterItem ) ;
			_xmppConnection.OnRosterEnd += new ObjectHandler( _xmppConnection_OnRosterEnd ) ;
			_xmppConnection.OnPresence += new PresenceHandler( _xmppConnection_OnPresence ) ;
			_xmppConnection.OnError += new ErrorHandler( _xmppConnection_OnError ) ;
			_xmppConnection.OnAuthError += new XmppElementHandler( _xmppConnection_OnAuthError ) ;

			_xmppConnection.OnIq += new IqHandler( _xmppConnection_OnIq );
			
			Settings.Default.Save() ;

			_xmppConnection.Open() ;
		}

		void _xmppConnection_OnIq( object sender, IQ iq )
		{
			if ( iq.Type == IqType.get )
			{
				if ( iq.Query is DiscoInfo )
				{
					// todo:
				}
			}
		}

		private void _xmppConnection_OnAuthError( object sender, Element e )
		{
			EventError eventError = new EventError( Resources.Event_AuthFailed, null ) ;
			Events.Instance.OnEvent( eventError ) ;
		}

		private void _xmppConnection_OnError( object sender, Exception ex )
		{
			EventError eventError = new EventError( ex.Message, null ) ;
			Events.Instance.OnEvent( eventError ) ;
		}

		private void _xmppConnection_OnPresence( object sender, Presence pres )
		{
			switch ( pres.Type )
			{
				case PresenceType.subscribe:
					{
						break ;
					}
				case PresenceType.subscribed:
					{
						break ;
					}
				case PresenceType.unsubscribe:
					{
						break ;
					}
				case PresenceType.unsubscribed:
					{
						break ;
					}
				default:
					{
						Roster.Instance.OnPresence( sender, pres ) ;
						break ;
					}
			}
		}

		private void _xmppConnection_OnRosterEnd( object sender )
		{
			SendMyPresence() ;
			DiscoveryRoot() ;
		}

		private void DiscoveryRoot()
		{
			Services.Instance.Clear() ;
			_itemsToDiscover = 0 ;

			NotifyPropertyChanged( "ItemsToDiscover" ) ;

			Discovery( null ) ;
		}

		private void SendMyPresence()
		{
			_xmppConnection.Show = Settings.Default.XmppMyPresence ;
			_xmppConnection.SendMyPresence() ;
		}

		private void _xmppConnection_OnRosterItem( object sender, RosterItem item )
		{
			Roster.Instance.OnRosterItem( sender, item ) ;
		}

		private void _xmppConnection_OnLogin( object sender )
		{
			IsLogged = true ;
		}

		private void _xmppConnection_OnClose( object sender )
		{
			IsLogged = false ;
		}

		public void Discovery( DiscoItem discoItem )
		{
			if ( discoItem != null )
			{
				lock ( Services.Instance._syncObject )
				{
					Service service = Services.Instance.FindService( discoItem ) ;

					if ( service != null )
					{
						if ( service.IsDiscovered )
						{
							return ;
						}

						service.IsDiscovered = true ;
					}
				}
			}

			DiscoManager discoManager = new DiscoManager( _xmppConnection ) ;

			Jid jid ;

			if ( discoItem == null )
			{
				jid = new Jid( _xmppConnection.Server ) ;
			}
			else
			{
				jid = discoItem.Jid ;
			}

			if ( discoItem != null && discoItem.Node != null )
			{
				discoManager.DisoverItems( jid, discoItem.Node, new IqCB( OnDiscoServerResult ), discoItem ) ;
			}
			else
			{
				discoManager.DisoverItems( jid, new IqCB( OnDiscoServerResult ), discoItem ) ;
			}
		}

		public int MyPriority
		{
			get
			{
				return _xmppConnection.Priority ;
			}
		}

		public Jid MyJid
		{
			get
			{
				return _xmppConnection.MyJID ;
			}
		}

		public bool IsLogged
		{
			get
			{
				return _isLogged ;
			}

			protected set
			{
				if ( _isLogged != value )
				{
					NotifyPropertyChanged( "IsLogged" ) ;
				}

				_isLogged = value ;
			}
		}

		public int ItemsToDiscover
		{
			get
			{
				return _itemsToDiscover ;
			}
		}

		private int _itemsToDiscover = 0 ;
		private object _itemsToDiscoverLock = new object() ;

		private void AddItemToDiscover()
		{
			lock ( _itemsToDiscoverLock )
			{
				_itemsToDiscover++ ;
			}

			NotifyPropertyChanged( "ItemsToDiscover" ) ;
		}

		private void RemoveItemToDiscover()
		{
			lock ( _itemsToDiscoverLock )
			{
				_itemsToDiscover-- ;
			}

			NotifyPropertyChanged( "ItemsToDiscover" ) ;
		}

		private void OnDiscoServerResult( object sender, IQ iq, object data )
		{
			if ( iq.Type == IqType.result )
			{
				Element query = iq.Query ;

				if ( query != null && query is DiscoItems )
				{
					DiscoItems items = query as DiscoItems ;
					DiscoItem[] itms = items.GetDiscoItems() ;

					DiscoManager dm = new DiscoManager( _xmppConnection ) ;

					foreach ( DiscoItem itm in itms )
					{
						if ( itm.Jid != null )
						{
							Services.Instance.OnServiceItem( sender, itm, data as DiscoItem ) ;

							dm.DisoverInformation( itm.Jid, new IqCB( OnDiscoInfoResult ), itm ) ;
							AddItemToDiscover() ;

							Discovery( itm ) ;
						}
					}
				}
			}
		}

		/*
		private void OnDiscoInfoCommands( object sender, IQ iq, object data )
		{
			if ( iq.Type == IqType.result && iq.Query is DiscoInfo )
			{
				DiscoInfo di = iq.Query as DiscoInfo ;

				DiscoItem discoItem = data as DiscoItem ;

			}
			else if ( iq.Type == IqType.error )
			{
				Services.Instance.OnServiceItemError( sender, iq ) ;
			}

			RemoveItemToDiscover() ;
		}*/

		private void OnDiscoInfoResult( object sender, IQ iq, object data )
		{
			if ( iq.Type == IqType.result && iq.Query is DiscoInfo )
			{
				DiscoInfo di = iq.Query as DiscoInfo ;

				DiscoItem discoItem = data as DiscoItem ;

				Services.Instance.OnServiceItemInfo( sender, discoItem, di ) ;
			}
			else if ( iq.Type == IqType.error )
			{
				Services.Instance.OnServiceItemError( sender, iq ) ;
			}

			RemoveItemToDiscover() ;
		}

		public void DoRegisterService( Service service, Data data )
		{
			RegisterIq registerIq = new RegisterIq( IqType.set, service.Jid ) ;
			registerIq.To = service.Jid ;
			registerIq.Query.AddChild( data ) ;

			_xmppConnection.IqGrabber.SendIq( registerIq, OnServiceRegistered, service ) ;
		}

		public void DoRegisterService( Service service, string userName, string password, string email )
		{
			RegisterIq registerIq = new RegisterIq( IqType.set, service.Jid ) ;
			registerIq.Query.Username = userName ;
			registerIq.Query.Password = password ;
			registerIq.Query.Email = email ;

			_xmppConnection.IqGrabber.SendIq( registerIq, OnServiceRegistered, service ) ;
		}

		private void OnServiceRegistered( object sender, IQ iq, object data )
		{
			Service service = data as Service ;

			if ( iq.Error != null )
			{
				EventError eventError = new EventError( string.Format( "'{0}' Service registration failed with '{1}'",
															service.Name, iq.Error.Condition ), iq.Error ) ;
				Events.Instance.OnEvent( eventError ) ;
			}
			else
			{
				EventInfo eventinfo = new EventInfo( string.Format( "'{0}' Service registration succeeded", service.Name ) ) ;
				Events.Instance.OnEvent( eventinfo ) ;
			}
		}

		public void GetService( Service service )
		{
			RegisterIq registerIq = new RegisterIq( IqType.get, service.Jid ) ;

			_xmppConnection.IqGrabber.SendIq( registerIq, OnRegisterServiceGet, service ) ;
		}

		public void GetServiceSearch( Service service )
		{
			SearchIq searchIq = new SearchIq( IqType.get, service.Jid ) ;

			_xmppConnection.IqGrabber.SendIq( searchIq, OnRegisterServiceGetSearch, service ) ;
		}

		private void OnRegisterServiceGetSearch( object sender, IQ iq, object data )
		{
			Search search = iq.Query as Search ;

			if ( search != null )
			{
				// Registration.Instance.DisplayInBandRegistration( register, ( Service )data ) ;
			}
			else
			{
				Service service = data as Service ;

				EventError eventError = new EventError( string.Format( "Getting search Info of Service '{0}'",
																			service.Name ), iq.Error ) ;
		
				Events.Instance.OnEvent( eventError ) ;
			}
		}


		private void OnRegisterServiceGet( object sender, IQ iq, object data )
		{
			Register register = iq.Query as Register ;

			if ( register != null )
			{
				Registration.Instance.DisplayInBandRegistration( register, ( Service )data ) ;
			}
			else
			{
				Service service = data as Service ;

				EventError eventError = new EventError( string.Format( "Getting registration Info of Service '{0}'",
																			service.Name ), iq.Error ) ;
		
				Events.Instance.OnEvent( eventError ) ;
			}
		}


		public void Close()
		{
			if ( IsLogged )
			{
				_xmppConnection.Close() ;
			}
		}
	}
}