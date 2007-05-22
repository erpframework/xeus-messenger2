using System ;
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

		private static Roster _instance = new Roster() ;

		public static Roster Instance
		{
			get
			{
				return _instance ;
			}
		}

		public void OnPresence( object sender, Presence presence )
		{
			App.InvokeSafe( DispatcherPriority.Background,
			                new PresenceCallback( OnPresence ), presence ) ;
		}

		private void OnPresence( Presence presence )
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
						                                                        presence.From, presence.Nickname ), null ) ) ;
					}
				}
				else
				{
					EventPresenceChanged eventPresenceChanged = new EventPresenceChanged( contact, contact.Presence, presence ) ;
					Events.Instance.OnEvent( this, eventPresenceChanged ) ;

					contact.Presence = presence ;
				}
			}
		}

		public void OnRosterItem( object sender, RosterItem item )
		{
			App.InvokeSafe( DispatcherPriority.Background,
			                new RosterItemCallback( OnRosterItem ), item ) ;
		}

		private void OnRosterItem( RosterItem item )
		{
			lock ( _syncObject )
			{
				Contact contact = FindContact( item.Jid ) ;

				if ( contact == null )
				{
					Add( new Contact( item ) ) ;
				}
				else if ( item.Subscription == SubscriptionType.remove)
				{
					Remove( contact ) ;
				}
			}
		}

		// unsafe - use lock in calling code
		private Contact FindContact( Jid jid )
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