using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.roster;
using xeus2.Properties;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    internal class Roster
    {
        private ObservableCollectionDisp<MetaContact> _items = new ObservableCollectionDisp<MetaContact>();

        private delegate void RosterItemCallback(RosterItem item);

        private delegate void PresenceCallback(Presence presence);

        private static Roster _instance = new Roster();

        private Dictionary<string, Contact> _realContacts = new Dictionary<string, Contact>();

        public static Roster Instance
        {
            get
            {
                return _instance;
            }
        }

        public ObservableCollectionDisp<MetaContact> Items
        {
            get
            {
                return _items;
            }
        }

        public void OnPresence(object sender, Presence presence)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new PresenceCallback(OnPresence), presence);
        }

        private void OnPresence(Presence presence)
        {
            lock (_items._syncObject)
            {
                Contact contact = FindContact(presence.From);

                if (contact == null)
                {
                    if (JidUtil.BareEquals(presence.From, Account.Instance.MyJid))
                    {
                        // it's me from another client
                        Events.Instance.OnEvent(this, new EventInfo(
                                                          String.Format(Resources.Event_AnotherClient,
                                                                        presence.From.Resource, presence.Priority,
                                                                        Account.Instance.MyJid.Bare, presence.Show)));

                        if (presence.Priority > Account.Instance.MyPriority)
                        {
                            Events.Instance.OnEvent(this, new EventInfo(
                                                              String.Format(Resources.Event_AnotherClientHigher,
                                                                            presence.From.Resource)));
                        }
                        else
                        {
                            Events.Instance.OnEvent(this, new EventInfo(
                                                              String.Format(Resources.Event_AnotherClientLower,
                                                                            presence.From.Resource)));
                        }
                    }
                    else
                    {
                        Events.Instance.OnEvent(this,
                                                new EventError(String.Format(Resources.Event_UnknownPresence,
                                                                             presence.From, presence.Nickname), null));
                    }
                }
                else
                {
                    EventPresenceChanged eventPresenceChanged =
                        new EventPresenceChanged(contact, contact.Presence, presence);
                    Events.Instance.OnEvent(this, eventPresenceChanged);

                    contact.Presence = presence;
                }
            }
        }

        public void OnRosterItem(object sender, RosterItem item)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new RosterItemCallback(OnRosterItem), item);
        }

        private void OnRosterItem(RosterItem item)
        {
            lock (_items._syncObject)
            {
                Contact contact = FindContact(item.Jid);

                if (contact == null)
                {
                    AddRosterItem(item);

                    Account.Instance.RequestVCard(item);
                }
                else if (item.Subscription == SubscriptionType.remove)
                {
                    RemoveRosterItem(contact);
                }
            }
        }

        // unsafe - use lock in calling code
        private Contact FindContact(Jid jid)
        {
            Contact contact;

            _realContacts.TryGetValue(jid.Bare, out contact);

            return contact;
        }

        void AddRosterItem(RosterItem item)
        {
            lock (_items._syncObject)
            {
                // for now
                _items.Add(new MetaContact(new Contact(item)));
            }
        }

        void RemoveRosterItem(Contact contact)
        {
            lock (_items._syncObject)
            {
                MetaContact metaContact = FindMetaContact(contact.Jid);

                if (metaContact != null)
                {
                    metaContact.SubContacts.Remove(contact);

                    if (metaContact.SubContacts.Count == 0)
                    {
                        _items.Remove(metaContact);
                    }
                }
            }
        }

        private MetaContact FindMetaContact(Jid jid)
        {
            foreach (MetaContact metaContact in _items)
            {
                lock (metaContact.SubContacts._syncObject)
                {
                    if (metaContact.FindContact(jid) != null)
                    {
                        return metaContact;
                    }
                }
            }

            return null;
        }
    }
}