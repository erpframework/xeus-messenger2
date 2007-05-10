using System;
using System.Collections.Generic;
using System.Text;
using System.Threading ;
using System.Windows.Threading ;
using agsXMPP ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.iq.roster ;
using xeus2.Properties ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.Core
{
	internal class Roster : ObservableCollectionDisp< Contact >
	{
		private delegate void RosterItemCallback( RosterItem item ) ;
		private delegate void PresenceCallback( Presence presence ) ;

		private static Roster _instance = new Roster();

		public static Roster Instance
		{
			get
			{
				return _instance ;
			}
		}

		public void OnPresence( object sender, Presence presence )
		{
			App.Current.Dispatcher.Invoke( DispatcherPriority.Background,
											new PresenceCallback( OnPresence ), presence ) ;
		}

		void OnPresence( Presence presence )
		{
			lock ( _syncObject )
			{
				Contact contact = FindContact( presence.From ) ;

				if ( contact == null )
				{
					if ( JidUtil.BareEquals( presence.From, Account.Instance.MyJid ) )
					{
						// it's me from another client
						Events.Instance.OnEvent( this, new EventInfo(
								String.Format( Resources.Event_AnotherClient,
												presence.From.Resource, presence.Priority,
												Account.Instance.MyJid.Bare, presence.Show ) ) ) ;

						if ( presence.Priority > Account.Instance.MyPriority )
						{
							Events.Instance.OnEvent( this, new EventInfo(
								String.Format( Resources.Event_AnotherClientHigher, presence.From.Resource ) ) ) ;
						}
						else
						{
							Events.Instance.OnEvent( this, new EventInfo(
								String.Format( Resources.Event_AnotherClientLower, presence.From.Resource ) ) ) ;
						}
					}
					else
					{
						Events.Instance.OnEvent( this,
							new EventError( String.Format( Resources.Event_UnknownPresence,
											presence.From, presence.Nickname ) ) ) ;
					}
				}
				else
				{
					EventPresenceChanged eventPresenceChanged = new EventPresenceChanged( contact, contact.Presence, presence ); 
					Events.Instance.OnEvent( this, eventPresenceChanged );

					contact.Presence = presence ;
				}
			}
		}

		public void OnRosterItem( object sender, RosterItem item )
		{
			App.Current.Dispatcher.Invoke( DispatcherPriority.Background,
											new RosterItemCallback( OnRosterItem ), item ) ;
		}

		void OnRosterItem( RosterItem item )
		{
			lock ( _syncObject )
			{
				Contact contact = FindContact( item.Jid ) ;

				if ( contact == null )
				{
					Add( new Contact( item ) );
				}
				else
				{
					// todo:
					throw new ApplicationException( "Can't find contact" ) ;
				}
			}
		}

		// unsafe - use lock in calling code
		Contact FindContact( Jid jid )
		{
			foreach ( Contact item in Items )
			{
				if ( JidUtil.BareEquals( item.Jid, jid ) )
				{
					return item ;
				}
			}

			return null ;
		}
	}
}
