using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Threading;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.caps;
using agsXMPP.protocol.extensions.chatstates;
using agsXMPP.protocol.iq.avatar;
using agsXMPP.protocol.iq.disco;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.iq.vcard;
using agsXMPP.protocol.iq.version;
using xeus2.Properties;
using xeus2.xeus.Data;
using xeus2.xeus.Middle;
using xeus2.xeus.Utilities;
using Avatar=agsXMPP.protocol.x.Avatar;
using Uri=agsXMPP.Uri;
using Version=agsXMPP.protocol.iq.version.Version;

namespace xeus2.xeus.Core
{
    internal class Roster
    {
        #region Delegates

        public delegate void NeedRefreshHandler();

        #endregion

        private static readonly Roster _instance = new Roster();

        private readonly List<ContactChat> _chats = new List<ContactChat>();
        private readonly object _chatsLock = new object();
        private readonly ObservableCollectionDisp<MetaContact> _items = new ObservableCollectionDisp<MetaContact>();
        private readonly Dictionary<string, Contact> _realContacts = new Dictionary<string, Contact>();

        private readonly Timer _timerRefresh = new Timer();

        public Roster()
        {
            _timerRefresh.AutoReset = false;
            _timerRefresh.Interval = 1000.0;

            _timerRefresh.Elapsed += _timerRefresh_Elapsed;
        }

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

        public event NeedRefreshHandler NeedRefresh;

        public void OnPresence(object sender, Presence presence)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new PresenceCallback(OnPresence), presence);
        }

        public void OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new MessageCallback(OnMessage), msg);
        }

        private void OnMessage(agsXMPP.protocol.client.Message msg)
        {
            if (msg.Type == MessageType.chat)
            {
                Message message = new Message(msg);

                if (msg.Body != null)
                {
                    Database.SaveMessage(message);
                }

                DistributeMessage(message, msg.Chatstate);
            }
        }

        private void _timerRefresh_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new RefreshCallback(TimerRefresh));
        }

        private void TimerRefresh()
        {
            if (NeedRefresh != null)
            {
                NeedRefresh();
            }
        }

        private void OnContactPresence(Presence presence, Capabilities capabilities)
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
                    if (contact.Presence == null
                        || contact.Presence.Show != presence.Show)
                    {
                        // only if showtype changes
                        EventPresenceChanged eventPresenceChanged =
                            new EventPresenceChanged(contact, contact.Presence, presence);

                        Events.Instance.OnEvent(this, eventPresenceChanged);
                    }

                    contact.Presence = presence;

                    if (capabilities != null)
                    {
                        contact.Caps = capabilities;
                    }

                    if (!contact.HasDiscoRecieved)
                    {
                        AskForDisco(contact);
                    }
                }
            }
        }

        private void RefreshIqAvatar(Contact contact)
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
            Contact contact = (Contact) data;

            if (iq.Type == IqType.error || iq.Error != null)
            {
                if (iq.Error.Code != ErrorCode.NotFound
                    && iq.Error.Code != ErrorCode.NotImplemented)
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
                new EventInfo(
                    string.Format("You've asked '{0} ({1})' for authorization", contact.DisplayName, contact.Jid));
            Events.Instance.OnEvent(this, eventinfo);
        }

        public void RemoveAuthorization(IContact contact)
        {
            Account.Instance.GetPresenceManager().RefuseSubscriptionRequest(contact.Jid);

            EventInfo eventinfo =
                new EventInfo(
                    string.Format("You've asked '{0} ({1})' for authorization", contact.DisplayName, contact.Jid));
            Events.Instance.OnEvent(this, eventinfo);
        }

        public void ApproveAuthorization(IContact contact)
        {
            Account.Instance.GetPresenceManager().ApproveSubscriptionRequest(contact.Jid);

            EventInfo eventinfo =
                new EventInfo(string.Format("'{0} ({1})' is now authorized", contact.DisplayName, contact.Jid));
            Events.Instance.OnEvent(this, eventinfo);
        }

        private void OnSubscribePresence(Presence presence)
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
                                                                         (contact ?? (object) presence));

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
                                new EventInfo(
                                    string.Format("'{0} ({1})' just authorized you", contact.DisplayName, contact.Jid));
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
                                new EventInfo(
                                    string.Format("'{0} ({1})' removed your authorization", contact.DisplayName,
                                                  contact.Jid));
                            Events.Instance.OnEvent(this, eventinfo);
                        }

                        break;
                    }
            }
        }

        private void AskForDisco(Contact contact)
        {
            Account.Instance.DiscoMan.DisoverInformation(contact.FullJid,
                new IqCB(OnDiscoInfoResult), contact);
        }

        private void OnDiscoInfoResult(object sender, IQ iq, object data)
        {
            Contact contact = (Contact)data;

            if (iq.Error != null)
            {
                Services.Instance.OnServiceItemError(sender, iq);
            }
            else if (iq.Type == IqType.result && iq.Query is DiscoInfo)
            {
                contact.Disco = iq.Query as DiscoInfo;
            }


            if (contact.Caps != null)
            {
                DiscoInfo discoInfo = CapsCache.Instance.Get(contact.Caps);

                if (discoInfo != null)
                {
                    contact.ExtendedDisco = discoInfo;
                }
                else
                {
                    Account.Instance.DiscoMan.DisoverInformation(contact.FullJid,
                                                                 string.Format("{0}#{1}",
                                                                 contact.Caps.Node, contact.Caps.Version),
                                                                 OnDiscoInfoCapsResult, contact);
                }
            }

            // ask for iq avatar only if the client has not v-card support
            if (contact.Caps == null || !contact.HasFeature(Uri.VCARD)
                || contact.Image == null || Storage.IsDefaultAvatar(contact.Image))
            {
                RefreshIqAvatar(contact);
            }

            if (!contact.HasVCardRecieved)
            {
                SetFreshVcard(contact, Settings.Default.VCardExpirationDays);
            }
        }

        private static void OnDiscoInfoCapsResult(object sender, IQ iq, object data)
        {
            Contact contact = (Contact)data;

            if (iq.Error != null)
            {
                Services.Instance.OnServiceItemError(sender, iq);
            }
            else if (iq.Type == IqType.result && iq.Query is DiscoInfo)
            {
                contact.ExtendedDisco = iq.Query as DiscoInfo;

                CapsCache.Instance.AddToCache(contact.Caps, contact.ExtendedDisco);
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
                Capabilities capabilities = presence.SelectSingleElement(typeof(Capabilities)) as Capabilities;

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
                            OnContactPresence(presence, capabilities);
                            break;
                        }
                }
            }
        }

        public void AskVersion(IContact contact)
        {
            VersionIq viq = new VersionIq(IqType.get, contact.FullJid);
            Account.Instance.XmppConnection.IqGrabber.SendIq(viq, new IqCB(VersionIqResult), contact);
        }

        private void VersionIqResult(object sender, IQ iq, object data)
        {
            IContact contact = (IContact)data;

            if (iq.Type == IqType.error || iq.Error != null)
            {
                // no version info
            }
            else if (iq.Type == IqType.result
                && iq.Query is Version)
            {
                contact.SetVersion((Version)iq.Query);
            }
        }

        public void SetFreshVcard(IContact contact, int maxOldness)
        {
            Contact cont = contact as Contact;
            MetaContact metaContact = contact as MetaContact;

            if (metaContact != null)
            {
                lock (metaContact.SubContacts._syncObject)
                {
                    cont = metaContact.FindContact(contact.Jid);
                }
            }

            if (cont == null)
            {
                return;
            }

            Vcard vcard = Storage.GetVcard(contact.Jid, maxOldness);

            if (vcard != null)
            {
                cont.SetVcard(vcard);
            }
            else // if (cont.HasFeature(Uri.VCARD))
            {
                VcardIq viq = new VcardIq(IqType.get, contact.Jid);
                Account.Instance.XmppConnection.IqGrabber.SendIq(viq, new IqCB(VcardResult), cont);
            }
        }

        private void VcardResult(object sender, IQ iq, object data)
        {
            Contact contact = (Contact) data;

            if (iq.Type == IqType.error || iq.Error != null)
            {
                if (iq.Error.Code == ErrorCode.NotFound
                    || iq.Error.Code == ErrorCode.NotImplemented
                    || iq.Error.Code == ErrorCode.Unauthorized)
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
            }
        }

        private void VcardResultAuth(object sender, IQ iq, object data)
        {
            Contact contact;

            if (data is Presence)
            {
                contact = new Contact((Presence) data);
            }
            else
            {
                contact = (Contact) data;
            }

            if (iq.Type == IqType.error || iq.Error != null)
            {
                if (iq.Error.Code == ErrorCode.NotFound
                    || iq.Error.Code == ErrorCode.NotImplemented
                    || iq.Error.Code == ErrorCode.Unauthorized)
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
        internal Contact FindContact(Jid jid)
        {
            Contact contact;

            _realContacts.TryGetValue(jid.Bare.ToLowerInvariant(), out contact);

            return contact;
        }

        private void AddRosterItem(RosterItem item)
        {
            Contact contact;

            lock (_items._syncObject)
            {
                // for now
                contact = Database.GetContact(item);

                MetaContact metaContact = null;

                if (contact != null)
                {
                    metaContact = Database.GetMetaContact(contact.MetaId);
                }

                if (metaContact == null)
                {
                    metaContact = new MetaContact();
                    Database.SaveMetaContact(metaContact);

                    contact = new Contact(item, metaContact.Id);
                    Database.SaveContact(contact);
                }

                metaContact.AddContact(contact);

                _realContacts.Add(item.Jid.Bare.ToLowerInvariant(), contact);

                _items.Add(metaContact);
            }

            Vcard vcard = Storage.GetVcard(contact.Jid, 99999);

            if (vcard != null)
            {
                contact.SetVcard(vcard);
                contact.HasVCardRecieved = false;
            }
        }

        public void NotifyNeedRefresh()
        {
            _timerRefresh.Stop();
            _timerRefresh.Start();
        }

        private void RemoveRosterItem(Contact contact)
        {
            lock (_items._syncObject)
            {
                MetaContact metaContact = FindMetaContact(contact.Jid);

                if (metaContact != null)
                {
                    metaContact.SubContacts.Remove(contact);

                    _realContacts.Remove(contact.Jid.Bare.ToLowerInvariant());

                    if (metaContact.SubContacts.Count == 0)
                    {
                        _items.Remove(metaContact);
                    }
                }
            }
        }

        internal MetaContact FindMetaContact(Jid jid)
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

        private void DistributeMessage(Message message, Chatstate chatstate)
        {
            lock (_items._syncObject)
            {
                Contact contact = FindContact(message.From);

                if (contact == null)
                {
                    throw new NotImplementedException("Message from contact not in roster");
                }
                else
                {
                    List<ContactChat> contactChats = GetContactChats(contact);

                    foreach (ContactChat chat in contactChats)
                    {
                        if (message.Body != null)
                        {
                            chat.Messages.Add(message);
                        }

                        chat.ChatState = chatstate;
                    }

                    if (message.Body != null)
                    {
                        RecentItems.Instance.Add(contact);
                        Events.Instance.OnEvent(this, new EventChatMessage(contact, message, (contactChats.Count > 0)));
                    }
                }
            }
        }

        public ContactChat CreateChat(IContact contact)
        {
            ContactChat contactChat = new ContactChat(contact, Account.Instance.XmppConnection);

            foreach (Message message in Database.GetMessages(contact, Settings.Default.UI_MaxHistoryMessages))
            {
                contactChat.Messages.Add(message);
            }

            lock (_chatsLock)
            {
                _chats.Add(contactChat);
            }

            return contactChat;
        }

        private List<ContactChat> GetContactChats(IContact contact)
        {
            List<ContactChat> contactChats = new List<ContactChat>();

            lock (_chatsLock)
            {
                foreach (ContactChat chat in _chats)
                {
                    if (contact == chat.Contact)
                    {
                        contactChats.Add(chat);
                    }
                    else
                    {
                        // find if the contact participates in metacontact chat
                        if (contact is Contact
                            && chat.Contact is MetaContact)
                        {
                            MetaContact metaContact = (MetaContact) chat.Contact;

                            Contact subContact;

                            lock (metaContact.SubContacts._syncObject)
                            {
                                subContact = metaContact.FindContact(contact.Jid);
                            }

                            if (subContact != null)
                            {
                                contactChats.Add(chat);
                            }
                        }
                    }
                }
            }

            return contactChats;
        }

        #region Nested type: MessageCallback

        private delegate void MessageCallback(agsXMPP.protocol.client.Message msg);

        #endregion

        #region Nested type: PresenceCallback

        private delegate void PresenceCallback(Presence presence);

        #endregion

        #region Nested type: RefreshCallback

        private delegate void RefreshCallback();

        #endregion

        #region Nested type: RosterItemCallback

        private delegate void RosterItemCallback(RosterItem item);

        #endregion
    }
}