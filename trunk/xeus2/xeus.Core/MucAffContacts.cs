using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using agsXMPP.protocol.x.muc.iq.admin;
using Item=agsXMPP.protocol.x.muc.iq.admin.Item;

namespace xeus2.xeus.Core
{
    internal class MucAffContacts : NotifyInfoDispatcher
    {
        private Affiliation _affiliation = Affiliation.none;
        private MucManager _manager;
        private MucRoom _mucRoom;

        private ObservableCollectionDisp<MucAffContact> _affContacts = new ObservableCollectionDisp<MucAffContact>();

        public delegate void EventChangeCallback(object sender, MucAffContact mucAffContact);

        public event EventChangeCallback OnChange;

        public Affiliation Affiliation
        {
            get
            {
                return _affiliation;
            }
        }

        public ObservableCollectionDisp<MucAffContact> AffContacts
        {
            get
            {
                return _affContacts;
            }
        }

        internal void SetupAffiliations(MucRoom mucRoom, Affiliation affiliation)
        {
            lock (_affContacts._syncObject)
            {
                _affContacts.Clear();
            }

            _affiliation = affiliation;
            _mucRoom = mucRoom;

            NotifyPropertyChanged("Affiliation");

            _manager = Account.Instance.GetMucManager();

            _manager.RequestList(_affiliation, mucRoom.Service.Jid, new IqCB(OnRequestResult), null);
        }

        private void OnRequestResult(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                Services.Instance.OnServiceItemError(sender, iq);
            }
            else if (iq.Type == IqType.result)
            {
                Admin admin = iq.Query as Admin;

                if (admin != null)
                {
                    List<MucAffContact> contacts = new List<MucAffContact>();

                    Item[] items = admin.GetItems();
                    foreach (Item item in items)
                    {
                        contacts.Add(new MucAffContact(item, this));
                    }

                    lock (_affContacts._syncObject)
                    {
                        _affContacts.Add(contacts);
                    }

                    if (OnChange != null)
                    {
                        OnChange(this, null);
                    }
                }
            }
        }

        public void AddNew(string text)
        {
            switch (Affiliation)
            {
                case Affiliation.owner:
                    {
                        _manager.GrantOwnershipPrivileges(_mucRoom.Service.Jid, new Jid(text),
                                                          new IqCB(OnAddResult),
                                                          new MucAffContact(new Jid(text), Affiliation.owner, this));
                        break;
                    }
                case Affiliation.admin:
                    {
                        _manager.GrantAdminPrivileges(_mucRoom.Service.Jid, new Jid(text),
                                                      new IqCB(OnAddResult),
                                                      new MucAffContact(new Jid(text), Affiliation.admin, this));
                        break;
                    }
                case Affiliation.member:
                    {
                        _manager.GrantMembership(_mucRoom.Service.Jid, new Jid(text), String.Empty,
                                                 new IqCB(OnAddResult),
                                                 new MucAffContact(new Jid(text), Affiliation.member, this));
                        break;
                    }
                case Affiliation.outcast:
                    {
                        _manager.BanUser(_mucRoom.Service.Jid, new Jid(text), String.Empty,
                                         new IqCB(OnAddResult),
                                         new MucAffContact(new Jid(text), Affiliation.outcast, this));
                        break;
                    }
            }
        }

        public void RemoveFromGroup(MucAffContact mucAffContact)
        {
            Item item = new Item(Affiliation.none, new Jid(mucAffContact.Jid));
            _manager.ModifyList(_mucRoom.Service.Jid, new Item[] {item},
                                new IqCB(OnRemoveResult), mucAffContact);
        }

        private void OnRemoveResult(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                Services.Instance.OnServiceItemError(sender, iq);
            }
            else if (iq.Type == IqType.result)
            {
                if (OnChange != null)
                {
                    OnChange(this, data as MucAffContact);
                }
            }
        }

        private void OnAddResult(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                Services.Instance.OnServiceItemError(sender, iq);
            }
            else if (iq.Type == IqType.result)
            {
                if (OnChange != null)
                {
                    OnChange(this, data as MucAffContact);

                    lock (_affContacts._syncObject)
                    {
                        _affContacts.Add(data as MucAffContact);
                    }
                }
            }
        }

        public void Remove(MucAffContact contact)
        {
            lock (_affContacts._syncObject)
            {
                foreach (MucAffContact mucAffContact in _affContacts)
                {
                    if (mucAffContact.Jid == contact.Jid)
                    {
                        _affContacts.Remove(mucAffContact);

                        break;
                    }
                }
            }
        }
    }
}