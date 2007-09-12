using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.avatar;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.iq.vcard;
using xeus.Data;
using xeus2.Properties;
using xeus2.xeus.Middle;
using xeus2.xeus.Utilities;
using Avatar=agsXMPP.protocol.x.Avatar;

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

        void OnContactPresence(Presence presence)
        {
            lock (_items._syncObject)
            {
                Contact contact = FindContact(presence.From);

                if (contact == null)
                {
                    if (JidUtil.BareEquals(presence.From, Account.Instance.Self.Jid))
                    {
                        // it's me from another client
                        Events.Instance.OnEvent(this, new EventInfo(
                                                          String.Format(Resources.Event_AnotherClient,
                                                                        presence.From.Resource, presence.Priority,
                                                                        Account.Instance.Self.Jid, presence.Show)));

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

                    RefreshIqAvatar(contact);
                }
            }            
        }

        void RefreshIqAvatar(Contact contact)
        {
            if (contact.Presence != null)
            {
                Avatar avatar = contact.Presence.SelectSingleElement(typeof (Avatar)) as Avatar;

                if (avatar != null)
                {
                    string hash;
                    Storage.GetIqAvatar(contact.Jid.Bare, out hash);

                    if (avatar.Hash.ToLowerInvariant() != contact.AvatarHash)
                    {
                        AvatarIq aiq = new AvatarIq(IqType.get, contact.FullJid);
                        Account.Instance.XmppConnection.IqGrabber.SendIq(aiq, new IqCB(IqAvatarResult), contact);
                    }
                }
            }
        }

        private void IqAvatarResult(object sender, IQ iq, object data)
        {
            Contact contact = (Contact)data;

            if (iq.Type == IqType.error || iq.Error != null)
            {
                if (iq.Error.Code != ErrorCode.NotFound)
                {
                    Events.Instance.OnEvent(this,
                                            new EventError(String.Format("Iq-Avatar receiving error from {0}", iq.From),
                                                           null));
                }
            }
            else if (iq.Type == IqType.result)
            {
                agsXMPP.protocol.iq.avatar.Avatar avatar = iq.Query as agsXMPP.protocol.iq.avatar.Avatar;

                if (avatar != null && avatar.Data != null)
                {
                    contact.SetIqAvatar(avatar.Data);

                    //save it
                    Storage.CacheIqAvatar(avatar.Data, contact.Jid.Bare);
                }
            }
        }


        public void AuthorizeContact(IContact contact, bool approve)
        {
            if (approve)
            {
                ApproveAuthorization(contact);
            }
            else
            {
                Account.Instance.GetPresenceManager().RefuseSubscriptionRequest(contact.Jid);

                EventInfo eventinfo =
                    new EventInfo(string.Format("'{0} ({1})' - authorization denied", contact.DisplayName, contact.Jid));
                Events.Instance.OnEvent(this, eventinfo);
            }
        }

        public void RequestAuthorization(IContact contact)
        {
            Account.Instance.GetPresenceManager().Subcribe(contact.Jid);

            EventInfo eventinfo =
                new EventInfo(string.Format("You've asked '{0} ({1})' for authorization", contact.DisplayName, contact.Jid));
            Events.Instance.OnEvent(this, eventinfo);
        }

        public void RemoveAuthorization(IContact contact)
        {
            Account.Instance.GetPresenceManager().RefuseSubscriptionRequest(contact.Jid);

            EventInfo eventinfo =
                new EventInfo(string.Format("You've asked '{0} ({1})' for authorization", contact.DisplayName, contact.Jid));
            Events.Instance.OnEvent(this, eventinfo);
        }

        public void ApproveAuthorization(IContact contact)
        {
            Account.Instance.GetPresenceManager().ApproveSubscriptionRequest(contact.Jid);

            EventInfo eventinfo =
                new EventInfo(string.Format("'{0} ({1})' is now authorized", contact.DisplayName, contact.Jid));
            Events.Instance.OnEvent(this, eventinfo);
        }

        void OnSubscribePresence(Presence presence)
        {
            Contact contact;

            lock (_realContacts)
            {
                contact = FindContact(presence.From);
            }

            switch (presence.Type)
            {
                case PresenceType.subscribe:
                    {
                        VcardIq viq = new VcardIq(IqType.get, presence.From);
                        Account.Instance.XmppConnection.IqGrabber.SendIq(viq, new IqCB(VcardResultAuth),
                                                                            (contact ?? (object)presence));

                        break;
                    }
                case PresenceType.subscribed:
                    {
                        if (contact == null)
                        {
                            EventInfo eventinfo =
                                new EventInfo(string.Format("'{0}' just authorized you", presence.From));
                            Events.Instance.OnEvent(this, eventinfo);
                        }
                        else
                        {
                            EventInfo eventinfo =
                                new EventInfo(string.Format("'{0} ({1})' just authorized you", contact.DisplayName, contact.Jid));
                            Events.Instance.OnEvent(this, eventinfo);

                            // try to get v-card instantly
                            VcardIq viq = new VcardIq(IqType.get, contact.Jid);
                            Account.Instance.XmppConnection.IqGrabber.SendIq(viq, new IqCB(VcardResult), contact);
                        }

                        break;
                    }
                case PresenceType.unsubscribe:
                    {
                        break;
                    }
                case PresenceType.unsubscribed:
                    {
                        if (contact == null)
                        {
                            EventInfo eventinfo =
                                new EventInfo(string.Format("'{0}' removed your authorization", presence.From));
                            Events.Instance.OnEvent(this, eventinfo);
                        }
                        else
                        {
                            EventInfo eventinfo =
                                new EventInfo(string.Format("'{0} ({1})' removed your authorization", contact.DisplayName, contact.Jid));
                            Events.Instance.OnEvent(this, eventinfo);
                        }

                        break;
                    }
            }
        }

        private void OnPresence(Presence presence)
        {
            if (presence.MucUser != null)
            {
                return;
            }

            if (presence.Error != null)
            {
                EventError eventError = new EventError(string.Format("Presence error from {0}", presence.From),
                                                        presence.Error);
                Events.Instance.OnEvent(this, eventError);
            }
            else
            {
                switch (presence.Type)
                {
                    case PresenceType.subscribe:
                    case PresenceType.subscribed:
                    case PresenceType.unsubscribe:
                    case PresenceType.unsubscribed:
                        {
                            OnSubscribePresence(presence);
                            break;
                        }
                    default:
                        {
                            OnContactPresence(presence);
                            break;
                        }
                }
            }
        }

        private void SetFreshVcard(Contact contact, Presence presence)
        {
            Vcard vcard = Storage.GetVcard(contact.Jid, Settings.Default.VCardExpirationDays);

            if (vcard != null)
            {
                contact.SetVcard(vcard);
            }
            else 
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

        private void VcardResultAuth(object sender, IQ iq, object data)
        {
            Contact contact;

            if (data is Presence)
            {
                contact = new Contact((Presence)data);
            }
            else
            {
                contact = (Contact) data;
            }

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

            Authorization.Instance.Ask(contact);
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

            Vcard vcard = Storage.GetVcard(contact.Jid, 99999);

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