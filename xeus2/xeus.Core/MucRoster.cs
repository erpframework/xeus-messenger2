using System.Text;
using System.Windows.Threading;
using agsXMPP ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.x.muc;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.Core
{
	internal class MucRoster : ObservableCollectionDisp< MucContact >
	{
		public MucContact Find( Jid jid )
		{
			foreach ( MucContact item in Items )
			{
				if ( JidUtil.Equals( jid, item.MucJid ) )
				{
					return item ;
				}
			}

			return null ;
		}

		public MucContact OnPresence( Presence presence, MucRoom mucRoom )
		{
			lock ( _syncObject )
			{
                StringBuilder message = new StringBuilder();
                EventMucRoom eventMucRoom;
                
                MucContact contact = Find(presence.From);

			    bool join = true;

				if ( contact == null )
				{
				    contact = new MucContact(presence, mucRoom);
                    Add(contact);
				}
				else
				{
					if ( presence.Type == PresenceType.unavailable )
					{
						Remove( contact ) ;
					    join = false;
					}
					else
					{
					    string group = contact.Group;

						contact.Presence = presence ;

                        if (group != contact.Group)
                        {
                            Remove(contact);
                            Add(contact);
                        }
					}
				}

                if (presence.Type != contact.Presence.Type)
                {
                    message.Append(contact.Nick);

                    if (join)
                    {
                        message.AppendFormat(" is {0}", contact.Role);

                        if (contact.Affiliation != Affiliation.none)
                        {
                            message.AppendFormat(" and {0}", contact.Affiliation);
                        }

                        eventMucRoom =
                            new EventMucRoom(TypicalEvent.Joined, mucRoom, contact.Presence.MucUser, message.ToString());
                    }
                    else
                    {
                        message.Append(" has left the room");
                        eventMucRoom =
                            new EventMucRoom(TypicalEvent.Left, mucRoom, contact.Presence.MucUser, message.ToString());
                    }

                    Events.Instance.OnEvent(this, eventMucRoom, DispatcherPriority.ApplicationIdle);
                }

			    return contact;
			}
		}
	}
}