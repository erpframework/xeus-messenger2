using agsXMPP ;
using agsXMPP.protocol.client ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.Core
{
	internal class MucRoster : ObservableCollectionDisp< MucContact >
	{
		public MucContact Find( Jid jid )
		{
			foreach ( MucContact item in Items )
			{
				if ( JidUtil.Equals( jid, item.Jid ) )
				{
					return item ;
				}
			}

			return null ;
		}

		public void OnPresence( Presence presence, MucRoom mucRoom )
		{
			lock ( _syncObject )
			{
				MucContact contact = Find( presence.From ) ;

				if ( contact == null )
				{
                    Add(new MucContact(presence, mucRoom));
				}
				else
				{
					if ( presence.Type == PresenceType.unavailable )
					{
						Remove( contact ) ;
					}
					else
					{
						contact.Presence = presence ;
					}
				}
			}
		}
	}
}