using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.iq.vcard;
using agsXMPP.protocol.x;
using xeus.Data;
using xeus2.Properties;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    internal class Roster
    {
        private static readonly Roster _instance = new Roster();
        private readonly ObservableCollectionDisp<MetaContact> _items = new ObservableCollectionDisp<MetaContact>();

        private readonly Dictionary<string, Contact> _realContacts = new Dictionary<string, Contact>();

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
                                                                        Account.Instance.MyJid, presence.Show)));

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

                    if (!contact.HasVCardRecieved)
                    {
                        SetFreshVcard(contact, presence);
                    }
                }
            }
        }

        private void SetFreshVcard(Contact contact, Presence presence)
        {
            Vcard vcard = Storage.GetVcard(contact.Jid.Bare, Settings.Default.VCardExpirationDays);

            bool askVCard = true;

            if (vcard != null)
            {
                contact.SetVcard(vcard);

                Avatar avatar = presence.SelectSingleElement(typeof (Avatar)) as Avatar;

                if (avatar != null && vcard.Photo != null)
                {
                    askVCard = (avatar.Hash.ToLowerInvariant() != Storage.GetPhotoHashCode(vcard.Photo));
                }
                else
                {
                    askVCard = false;
                }
            }

            if (askVCard)
            {
                VcardIq viq = new VcardIq(IqType.get, contact.Jid);
                Account.Instance.XmppConnection.IqGrabber.SendIq(viq, new IqCB(VcardResult), contact);
            }
        }

        private void VcardResult(object sender, IQ iq, object data)
        {
            Contact contact = (Contact) data;

            if (iq.Type == IqType.error || iq.Error != null)
            {
                if (iq.Error.Code == ErrorCode.NotFound)
                {
                    contact.SetVcard(null);
                }
                else
                {
                    Events.Instance.OnEvent(this,
                                            new EventError(String.Format("V-Card receiving error from {0}", iq.From),
                                                           null));
                }
            }
            else if (iq.Type == IqType.result)
            {
                contact.SetVcard(iq.Vcard);

                //save it
                if (iq.Vcard != null)
                {
                    Storage.CacheVCard(iq.Vcard, contact.Jid.Bare);
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

        private void AddRosterItem(RosterItem item)
        {
            Contact contact;

            lock (_items._syncObject)
            {
                // for now
                contact = new Contact(item);

                _realContacts.Add(item.Jid.ToString(), contact);

                _items.Add(new MetaContact(contact));
            }

            Vcard vcard = Storage.GetVcard(contact.Jid.Bare, 99999);

            if (vcard != null)
            {
                contact.SetVcard(vcard);
            }
        }

        private void RemoveRosterItem(Contact contact)
        {
            lock (_items._syncObject)
            {
                MetaContact metaContact = FindMetaContact(contact.Jid);

                if (metaContact != null)
                {
                    metaContact.SubContacts.Remove(contact);

                    _realContacts.Remove(contact.Jid.ToString());

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

        #region Nested type: PresenceCallback

        private delegate void PresenceCallback(Presence presence);

        #endregion

        #region Nested type: RosterItemCallback

        private delegate void RosterItemCallback(RosterItem item);

        #endregion
    }
}