using System ;
using System.Collections.Generic ;
using System.Timers ;
using agsXMPP ;
using agsXMPP.net ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.extensions.commands ;
using agsXMPP.protocol.iq.disco ;
using agsXMPP.protocol.iq.register ;
using agsXMPP.protocol.iq.roster ;
using agsXMPP.protocol.iq.search ;
using agsXMPP.protocol.x.data ;
using agsXMPP.protocol.x.muc ;
using agsXMPP.Xml.Dom ;
using xeus2.Properties ;
using xeus2.xeus.Middle ;
using Search=xeus2.xeus.Middle.Search;
using Uri=agsXMPP.Uri;

namespace xeus2.xeus.Core
{
	internal class DiscoverySessionData
	{
		private string _sessionKey ;
		private object _data ;

		public DiscoverySessionData( object data )
		{
			_data = data ;
			_sessionKey = Services.Instance.SessionKey ;
		}

		public string SessionKey
		{
			get
			{
				return _sessionKey ;
			}
		}

		public object Data
		{
			get
			{
				return _data ;
			}
		}
	}

	internal class Account : NotifyInfoDispatcher
	{
		private Timer _discoTime = new Timer( 50 ) ;

		private static Account _instance = new Account() ;

		private XmppClientConnection _xmppConnection = new XmppClientConnection() ;
		private DiscoManager _discoManager ;

		private MucManager _mucManager = null ;
		private bool _isLogged = false ;

		public static Account Instance
		{
			get
			{
				return _instance ;
			}
		}

		protected void Cleanup()
		{
			Services.Instance.Clear();
			Roster.Instance.Clear();
		}

		public void Login()
		{
			Close() ;
			Cleanup() ;
			Open() ;
		}

		public void Open()
		{
			_discoManager = new DiscoManager( _xmppConnection ) ;

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

			_xmppConnection.OnIq += new IqHandler( _xmppConnection_OnIq ) ;

			Settings.Default.Save() ;

			_mucManager = new MucManager( _xmppConnection ) ;

			_xmppConnection.Open() ;

			_discoTime.Elapsed += new ElapsedEventHandler( _discoTime_Elapsed ) ;
		}

		private Queue< DiscoItem > _pendingDisco = new Queue< DiscoItem >() ;
		private Queue< DiscoItem > _pendingDiscoInfo = new Queue< DiscoItem >() ;

		private int _servicesCount ;
		private int _servicesDoneCount ;

		private void _discoTime_Elapsed( object sender, ElapsedEventArgs e )
		{
			_discoTime.Stop() ;

			if ( _pendingDiscoInfo.Count > 0 )
			{
				DiscoItem discoItem = _pendingDiscoInfo.Dequeue() ;

				ServicesCount++ ;

				if ( string.IsNullOrEmpty( discoItem.Node ) )
				{
					_discoManager.DisoverInformation( discoItem.Jid, new IqCB( OnDiscoInfoResult ),
					                                  new DiscoverySessionData( discoItem ) ) ;
				}
				else
				{
					_discoManager.DisoverInformation( discoItem.Jid, discoItem.Node, new IqCB( OnDiscoInfoResult ),
					                                  new DiscoverySessionData( discoItem ) ) ;
				}
			}
			else if ( _pendingDisco.Count > 0 )
			{
				DiscoItem discoItem = _pendingDisco.Dequeue() ;

				ServicesDoneCount++ ;

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
					_discoManager.DisoverItems( jid, discoItem.Node, new IqCB( OnDiscoServerResult ),
					                            new DiscoverySessionData( discoItem ) ) ;
				}
				else
				{
					_discoManager.DisoverItems( jid, new IqCB( OnDiscoServerResult ), new DiscoverySessionData( discoItem ) ) ;
				}
			}

			_discoTime.Start();
		}

		public void StopDiscovery()
		{
			_discoTime.Stop();

			_pendingDisco.Clear() ;
			_pendingDiscoInfo.Clear() ;

			Services.Instance.StopSession() ;
		}

		private void _xmppConnection_OnIq( object sender, IQ iq )
		{
		}

		private void _xmppConnection_OnAuthError( object sender, Element e )
		{
			EventError eventError = new EventError( Resources.Event_AuthFailed, null ) ;
			Events.Instance.OnEvent( this, eventError ) ;
		}

		private void _xmppConnection_OnError( object sender, Exception ex )
		{
			EventError eventError = new EventError( ex.Message, null ) ;
			Events.Instance.OnEvent( this, eventError ) ;
		}

		private void _xmppConnection_OnPresence( object sender, Presence pres )
		{
			if ( pres.MucUser != null )
			{
				return ;
			}

			if ( pres.Error != null )
			{
				EventError eventError = new EventError( string.Format( "Presence error from {0}", pres.From ),
				                                        pres.Error ) ;
				Events.Instance.OnEvent( this, eventError ) ;
			}
			else
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
		}

		private void _xmppConnection_OnRosterEnd( object sender )
		{
			SendMyPresence() ;
		}

		public void Discovery( string serverJid )
		{
			StopDiscovery();

			Services.Instance.Clear() ;

			ServicesDoneCount = 0 ;
			ServicesCount = 0 ;

			Jid jid ;

			if ( string.IsNullOrEmpty( serverJid ) )
			{
				jid = new Jid( _xmppConnection.Server ) ;
			}
			else
			{
				jid = new Jid( serverJid ) ;
			}

			Discovery( jid ) ;
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

		private void Discovery( Jid jid )
		{
			_discoManager.DisoverItems( jid, new IqCB( OnDiscoServerResult ), new DiscoverySessionData( null ) ) ;
		}

		public void Discovery( DiscoItem discoItem )
		{
			if ( Services.Instance.SessionKey == string.Empty )
			{
				return ;
			}

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

			_pendingDisco.Enqueue( discoItem ) ;
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

		public int ServicesCount
		{
			get
			{
				lock ( _serviceCountLock )
				{
					return _servicesCount ;
				}
			}
			set
			{
				lock ( _serviceCountLock )
				{
					_servicesCount = value ;
				}

				NotifyPropertyChanged( "ServicesCount" ) ;
			}
		}

		object _serviceCountLock = new object();

		public int ServicesDoneCount
		{
			get
			{
				lock ( _serviceCountLock )
				{
					return _servicesDoneCount ;
				}
			}
			set
			{
				lock ( _serviceCountLock )
				{
					_servicesDoneCount = value ;
				}

				NotifyPropertyChanged( "ServicesDoneCount" ) ;
			}
		}

		private bool CheckSessionKey( object data )
		{
			DiscoverySessionData sessionData = data as DiscoverySessionData ;

			return ( Services.Instance.SessionKey != string.Empty
			         && sessionData.SessionKey == Services.Instance.SessionKey ) ;
		}

		private void OnDiscoServerResult( object sender, IQ iq, object data )
		{
			_discoTime.Start();

			if ( !CheckSessionKey( data ) )
			{
				return ;
			}

			if ( iq.Error != null )
			{
				EventError eventError = new EventError( string.Format( Resources.Error_DiscoFailed, iq.From ), iq.Error ) ;
				Events.Instance.OnEvent( this, eventError ) ;
			}
			else if ( iq.Type == IqType.result )
			{
				Element query = iq.Query ;

				if ( query != null && query is DiscoItems )
				{
					DiscoItems items = query as DiscoItems ;
					DiscoItem[] itms = items.GetDiscoItems() ;

					foreach ( DiscoItem itm in itms )
					{
						if ( itm.Jid != null )
						{
							if ( !CheckSessionKey( data ) )
							{
								return ;
							}

							DiscoverySessionData sessionData = data as DiscoverySessionData ;

							Services.Instance.OnServiceItem( sender, itm, sessionData.Data as DiscoItem ) ;
						}
					}
				}
			}
		}

		public void DiscoInfo( DiscoItem item )
		{
			_pendingDiscoInfo.Enqueue( item ) ;
		}

		private void OnDiscoInfoResult( object sender, IQ iq, object data )
		{
			if ( !CheckSessionKey( data ) )
			{
				return ;
			}

			if ( iq.Error != null )
			{
				Services.Instance.OnServiceItemError( sender, iq ) ;
			}
			else if ( iq.Type == IqType.result && iq.Query is DiscoInfo )
			{
				DiscoInfo di = iq.Query as DiscoInfo ;

				DiscoverySessionData sessionData = data as DiscoverySessionData ;

				DiscoItem discoItem = sessionData.Data as DiscoItem ;

				Services.Instance.OnServiceItemInfo( sender, discoItem, di ) ;

				if ( di.HasFeature( Uri.COMMANDS ) && di.Node == null )
				{
					_discoManager.DisoverItems( discoItem.Jid,
					                            Uri.COMMANDS,
					                            new IqCB( OnCommandsServerResult ),
					                            new DiscoverySessionData( discoItem ) ) ;
				}
			}
		}

		private void OnCommandsServerResult( object sender, IQ iq, object data )
		{
			if ( !CheckSessionKey( data ) )
			{
				return ;
			}

			if ( iq.Error != null )
			{
				EventError eventError = new EventError( string.Format( Resources.Error_CommandResultFailed, iq.From ), iq.Error ) ;
				Events.Instance.OnEvent( this, eventError ) ;
			}
			else if ( iq.Type == IqType.result )
			{
				DiscoverySessionData discoverySessionData = data as DiscoverySessionData ;
				Services.Instance.OnCommandsItemInfo( this, discoverySessionData.Data as DiscoItem, iq ) ;
			}
		}

		public void DoSearchService( Service service, Data data )
		{
			SearchIq searchIq = new SearchIq( IqType.set, service.Jid ) ;
			searchIq.To = service.Jid ;
			searchIq.Query.AddChild( data ) ;

			_xmppConnection.IqGrabber.SendIq( searchIq, OnServiceSearched, service ) ;
		}

		public void DoSearchService( Service service, string first, string last, string nick, string email )
		{
			SearchIq searchIq = new SearchIq( IqType.set, service.Jid ) ;
			searchIq.Query.Firstname = first ;
			searchIq.Query.Lastname = last ;
			searchIq.Query.Nickname = nick ;
			searchIq.Query.Email = email ;

			_xmppConnection.IqGrabber.SendIq( searchIq, OnServiceSearched, service ) ;
		}

		private void OnServiceSearched( object sender, IQ iq, object data )
		{
			Service service = data as Service ;

			agsXMPP.protocol.iq.search.Search search = iq.Query as agsXMPP.protocol.iq.search.Search ;

			if ( iq.Error != null )
			{
				EventError eventError = new EventError( string.Format( Resources.Event_SearchFailed,
				                                                       service.Name, iq.Error.Condition ), iq.Error ) ;
				Events.Instance.OnEvent( this, eventError ) ;
			}
			else if ( iq.Type == IqType.result && search != null )
			{
				Search.Instance.DisplaySearchResult( search, ( Service ) data ) ;

				EventInfo eventinfo = new EventInfo( string.Format( Resources.Even_SearchSucceeded, service.Name ) ) ;
				Events.Instance.OnEvent( this, eventinfo ) ;
			}
		}

		public void DoRegisterService( Service service, Data data )
		{
			RegisterIq registerIq = new RegisterIq( IqType.set, service.Jid ) ;
			registerIq.To = service.Jid ;
			registerIq.Query.AddChild( data ) ;

			_xmppConnection.IqGrabber.SendIq( registerIq, OnServiceRegistered, service ) ;
		}

		public void DoRegisterService( Service service, Dictionary< string, string > values )
		{
			RegisterIq registerIq = new RegisterIq( IqType.set, service.Jid ) ;

			foreach ( KeyValuePair< string, string > value in values )
			{
				registerIq.Query.AddTag( value.Key, value.Value ) ;
			}

			_xmppConnection.IqGrabber.SendIq( registerIq, OnServiceRegistered, service ) ;
		}

		private void OnServiceRegistered( object sender, IQ iq, object data )
		{
			Service service = data as Service ;

			if ( iq.Error != null )
			{
				EventError eventError = new EventError( string.Format( Resources.Event_RegistrationFailed,
				                                                       service.Name, iq.Error.Condition ), iq.Error ) ;
				Events.Instance.OnEvent( this, eventError ) ;
			}
			else if ( iq.Type == IqType.result )
			{
				EventInfo eventinfo = new EventInfo( string.Format( Resources.Event_RegistrationSucceeded, service.Name ) ) ;
				Events.Instance.OnEvent( this, eventinfo ) ;
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
			agsXMPP.protocol.iq.search.Search search = iq.Query as agsXMPP.protocol.iq.search.Search ;

			if ( iq.Error != null )
			{
				Service service = data as Service ;

				EventError eventError = new EventError( string.Format( Resources.Event_SearchInfoFailed,
				                                                       service.Name ), iq.Error ) ;

				Events.Instance.OnEvent( this, eventError ) ;
			}
			else if ( iq.Type == IqType.result && search != null )
			{
				Search.Instance.DisplaySearch( search, ( Service ) data ) ;
			}
		}

		private void OnRegisterServiceGet( object sender, IQ iq, object data )
		{
			Register register = iq.Query as Register ;

			if ( iq.Error != null )
			{
				Service service = data as Service ;

				EventError eventError = new EventError( string.Format( Resources.Event_RegInfoFailed,
				                                                       service.Name ), iq.Error ) ;

				Events.Instance.OnEvent( this, eventError ) ;
			}
			else if ( iq.Type == IqType.result && register != null )
			{
				Registration.Instance.DisplayInBandRegistration( register, ( Service ) data ) ;
			}
		}

		public void ServiceCommand( Service service )
		{
			IQ commandIq = new IQ( IqType.set ) ;

			commandIq.GenerateId() ;
			commandIq.To = service.Jid ;

			Command command = new Command( service.Node ) ;
			command.Action = Action.execute ;

			commandIq.AddChild( command ) ;

			_xmppConnection.IqGrabber.SendIq( commandIq, OnCommandResult, service ) ;
		}

		private void OnCommandResult( object sender, IQ iq, object data )
		{
			if ( iq.Error != null )
			{
				Service service = data as Service ;

				EventError eventError = new EventError( string.Format( Resources.Event_CommandExecFailed,
				                                                       service.Name ), iq.Error ) ;

				Events.Instance.OnEvent( this, eventError ) ;
			}
			else if ( iq.Type == IqType.result )
			{
				foreach ( Node node in iq.ChildNodes )
				{
					Command command = node as Command ;

					if ( command != null )
					{
						Service service = data as Service ;

						if ( service == null )
						{
							service = ( data as ServiceCommandExecution ).Service ;
						}

						CommandExecutor.Instance.DisplayQuestionaire( command, service ) ;
						break ;
					}
				}
			}
		}

		public void Close()
		{
			if ( IsLogged )
			{
				_xmppConnection.Close() ;
			}
		}

		protected void ExecuteServiceCommand( ServiceCommandExecution command, Action action )
		{
			IQ commandIq = new IQ( IqType.set ) ;

			commandIq.GenerateId() ;
			commandIq.To = command.Service.Jid ;

			Command commandExec = new Command( command.Command.Node ) ;
			commandExec.Action = action ;
			commandExec.SessionId = command.Command.SessionId ;

			commandIq.AddChild( commandExec ) ;

			_xmppConnection.IqGrabber.SendIq( commandIq, OnCommandResult, command ) ;
		}

		public void ServiceCommandComplete( ServiceCommandExecution command )
		{
			ExecuteServiceCommand( command, Action.complete ) ;
		}

		public void ServiceCommandNext( ServiceCommandExecution command )
		{
			ExecuteServiceCommand( command, Action.next ) ;
		}

		public void ServiceCommandPrevious( ServiceCommandExecution command )
		{
			ExecuteServiceCommand( command, Action.prev ) ;
		}

		public void ServiceCommandCancel( ServiceCommandExecution command )
		{
			ExecuteServiceCommand( command, Action.cancel ) ;
		}

		public void RequestVCard( RosterItem item )
		{
		}

		public void JoinMuc( string jidBare )
		{
			Service service ;

			Jid jid = new Jid( jidBare ) ;

			lock ( Services.Instance._syncObject )
			{
				service = Services.Instance.FindService( jid ) ;
			}
			
			if ( service == null )
			{
				// not on this server
				service = new Service( new DiscoItem(), false ) ;
				service.DiscoItem.Jid = new Jid( jidBare ) ;
			}

			DiscoverReservedRoomNickname( service ) ;
		}

		public void JoinMuc( Service service )
		{
			DiscoverReservedRoomNickname( service ) ;
		}
		
		protected void DiscoverReservedRoomNickname( Service service )
		{
			IQ iq = new IQ( IqType.get, MyJid, service.Jid ) ;

			iq.GenerateId() ;
			DiscoInfo di = new DiscoInfo() ;
			di.Node = "x-roomuser-item" ;
			iq.Query = di ;

			_xmppConnection.IqGrabber.SendIq( iq, new IqCB( OnRoomNicknameResult ), service ) ;
		}

		private void OnRoomNicknameResult( object sender, IQ iq, object data )
		{
			string nick = null ;

			if ( iq.Error != null )
			{
				EventError eventError = new EventError( string.Format( Resources.Error_RoomNickFailed,
				                                                       iq.From ), iq.Error ) ;

				Events.Instance.OnEvent( this, eventError ) ;
			}
			else if ( iq.Type == IqType.result && iq.Query is DiscoInfo )
			{
				DiscoInfo di = iq.Query as DiscoInfo ;

				if ( di != null && di.Node == "x-roomuser-item"
				     && di.GetIdentities() != null
				     && di.GetIdentities().Length > 0 )
				{
					nick = di.GetIdentities()[ 0 ].Name ;
				}
			}

			JoinMuc( data as Service, nick ) ;
		}

		protected void JoinMuc( Service service, string nick )
		{
			MucInfo.Instance.MucLogin( service, nick ) ;
		}

		public MucRoom JoinMuc( Service service, string nick, string password )
		{
			if ( service.IsMucPasswordProtected )
			{
				_mucManager.JoinRoom( service.Jid, nick, password ) ;
			}
			else
			{
				_mucManager.JoinRoom( service.Jid, nick ) ;
			}

			return new MucRoom( service, _xmppConnection ) ;
		}
	}
}