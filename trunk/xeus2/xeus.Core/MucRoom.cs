using System.Windows.Threading ;
using agsXMPP ;
using agsXMPP.Collections ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class MucRoom
	{
		private MucRoster _mucRoster = new MucRoster() ;
		private MucMessages _mucMessages = new MucMessages();

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

		private void MessageCallback( object sender, Message msg, object data )
		{
			if ( App.CheckAccessSafe() )
			{
				MucContact contact = null ;

				lock ( MucRoster._syncObject )
				{
					contact = MucRoster.Find( msg.From ) ;
				}

				MucMessages.OnMessage( msg, contact );
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
				MucRoster.OnPresence( presence ) ;
			}
			else
			{
				App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Background,
				                                    new PresenceCB( PresenceCallback ), sender, presence, data ) ;
			}
		}
	}
}