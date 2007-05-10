using agsXMPP ;
using agsXMPP.net ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.iq.disco ;
using agsXMPP.protocol.iq.roster ;
using agsXMPP.Xml.Dom ;
using xeus2.Properties ;

namespace xeus2.xeus.Core
{
	internal class Account
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
			if ( _isLogged )
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
			_xmppConnection.OnError += new ErrorHandler( _xmppConnection_OnError );
			_xmppConnection.OnAuthError += new XmppElementHandler( _xmppConnection_OnAuthError );

			// todo:
			// _xmppConnection.Capabilities.

			Settings.Default.Save() ;

			_xmppConnection.Open() ;
		}

		void _xmppConnection_OnAuthError( object sender, Element e )
		{
			EventError eventError = new EventError( "Authorization failed" );
			Events.Instance.OnEvent( eventError );
		}

		void _xmppConnection_OnError( object sender, System.Exception ex )
		{
			EventError eventError = new EventError( ex.Message );
			Events.Instance.OnEvent( eventError );
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
			Discovery() ;
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
			_isLogged = true ;
		}

		private void _xmppConnection_OnClose( object sender )
		{
			_isLogged = false ;
		}

		public void Discovery()
		{
			DiscoManager discoManager = new DiscoManager( _xmppConnection ) ;
			discoManager.DisoverItems( new Jid( _xmppConnection.Server ), new IqCB( OnDiscoServerResult ), null ) ;
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
							dm.DisoverInformation( itm.Jid, new IqCB( OnDiscoInfoResult ), itm ) ;
						}
					}
				}
			}
		}

		private void OnDiscoInfoResult( object sender, IQ iq, object data )
		{
			if ( iq.Type == IqType.result && iq.Query is DiscoInfo )
			{
				DiscoInfo di = iq.Query as DiscoInfo ;

				Services.Instance.OnServiceItem( sender, di ) ;
			}
		}

		public void Close()
		{
			if ( _isLogged )
			{
				_xmppConnection.Close() ;
			}
		}
	}
}