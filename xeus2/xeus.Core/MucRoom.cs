using System ;
using System.Windows.Threading ;
using agsXMPP ;
using agsXMPP.Collections ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class MucRoom
	{
		private MucRoster _mucRoster = new MucRoster() ;
		private MucMessages _mucMessages = new MucMessages() ;

		private Service _service ;
		private XmppClientConnection _xmppClientConnection = null ;

		public MucRoom( Service service, XmppClientConnection xmppClientConnection )
		{
			_service = service ;
			_xmppClientConnection = xmppClientConnection ;

			_xmppClientConnection.MesagageGrabber.Add( service.Jid, new BareJidComparer(), new MessageCB( MessageCallback ), null ) ;
			_xmppClientConnection.PresenceGrabber.Add( service.Jid, new BareJidComparer(), new PresenceCB( PresenceCallback ),
			                                           null ) ;
		}

		public MucRoster MucRoster
		{
			get
			{
				return _mucRoster ;
			}
		}

		public MucMessages MucMessages
		{
			get
			{
				return _mucMessages ;
			}
		}

		public Service Service
		{
			get
			{
				return _service ;
			}
		}

		private void MessageCallback( object sender, Message msg, object data )
		{
			if ( App.CheckAccessSafe() )
			{
				if ( msg.Error != null )
				{
					EventError eventError = new EventError( string.Format( "Message error in MUC from {0}", msg.From ),
					                                        msg.Error ) ;
					Events.Instance.OnEvent( eventError ) ;
				}
				else
				{
					MucContact contact = null ;

					lock ( MucRoster._syncObject )
					{
						contact = MucRoster.Find( msg.From ) ;
					}

					MucMessages.OnMessage( msg, contact ) ;
				}
			}
			else
			{
				App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Background,
				                                    new MessageCB( MessageCallback ), sender, msg, data ) ;
			}
		}

		private void PresenceCallback( object sender, Presence presence, object data )
		{
			if ( App.CheckAccessSafe() )
			{
				if ( presence.Error != null )
				{
					EventError eventError = new EventError( string.Format( "Presence error in MUC from {0}", presence.From ),
					                                        presence.Error ) ;
					Events.Instance.OnEvent( eventError ) ;
				}
				else
				{
					MucRoster.OnPresence( presence ) ;
				}
			}
			else
			{
				App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Background,
				                                    new PresenceCB( PresenceCallback ), sender, presence, data ) ;
			}
		}

		public void SendMessage( string text )
		{
			Message message = new Message() ;

			message.Type = MessageType.groupchat ;
			message.To = _service.Jid ;
			message.Body = text ;

			_xmppClientConnection.Send( message ) ;
		}

		public void LeaveRoom()
		{
			Presence presence = new Presence() ;
			presence.To = _service.Jid ;
			presence.Type = PresenceType.unavailable ;

			_xmppClientConnection.Send( presence ) ;

			_xmppClientConnection.MesagageGrabber.Remove( _service.Jid ) ;
			_xmppClientConnection.PresenceGrabber.Remove( _service.Jid ) ;
		}
	}
}