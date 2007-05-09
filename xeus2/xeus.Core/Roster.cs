using System;
using System.Collections.Generic;
using System.Text;
using System.Threading ;
using System.Windows.Threading ;
using agsXMPP ;
using agsXMPP.protocol.iq.roster ;

namespace xeus2.xeus.Core
{
	internal class Roster : ObservableCollectionDisp< Contact >
	{
		private delegate void RosterItemCallback( RosterItem item ) ;

		private static Roster _instance = new Roster();

		public static Roster Instance
		{
			get
			{
				return _instance ;
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
				}
			}
		}

		// unsafe - use lock in calling code
		Contact FindContact( Jid jid )
		{
			foreach ( Contact item in Items )
			{
				if ( item.Jid == jid )
				{
					return item ;
				}
			}

			return null ;
		}
	}
}
