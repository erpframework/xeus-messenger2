using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.iq.disco;
using xeus2.Properties;
using xeus2.xeus.Data;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    internal class RecentItems : ObservableCollectionDisp<IJid>
    {
        private static readonly RecentItems _instance = new RecentItems();

        private List<Recent> _recents;
        readonly object _recentsLock = new object();

        public static RecentItems Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Add(MucRoom mucRoom)
        {
            Add(mucRoom.Service.Jid, RecentType.MUC);
        }

        public void Add(Contact contact)
        {
            Add(contact.Jid, RecentType.Chat);
        }

        void Add(Jid jid, RecentType recentType)
        {
            lock (_recentsLock)
            {
                Recent recent = Exists(jid);

                if (recent != null)
                {
                    _recents.Remove(recent);
                }
                else if (_recents.Count >= Settings.Default.UI_MaxRecentItems)
                {
                    _recents.RemoveAt(_recents.Count - 1);
                }

                if (recent == null)
                {
                    recent = new Recent(jid, recentType);
                }

                recent.DateTime = DateTime.Now;
                _recents.Insert(0, recent);
            }

            Build();            
        }

        Recent Exists(Jid jid)
        {
            foreach (Recent recent in _recents)
            {
                if (JidUtil.BareEquals(recent.Jid, jid))
                {
                    return recent;
                }
            }

            return null;
        }

        void Build()
        {

            lock (_syncObject)
            {
                Clear();

                foreach (Recent recent in _recents)
                {
                    switch (recent.RecentType)
                    {
                        case RecentType.Chat:
                            {
                                lock (Roster.Instance.Items._syncObject)
                                {
                                    Contact contact = Roster.Instance.FindContact(recent.Jid);

                                    if (contact != null)
                                    {
                                        MetaContact metaContact = Roster.Instance.FindMetaContact(contact.Jid);

                                        if (metaContact != null)
                                        {
                                            Add(metaContact);
                                        }
                                    }
                                }
                                break;
                            }
                        case RecentType.MUC:
                            {
                                Service service = new Service(new DiscoItem(), false);
                                service.DiscoItem.Jid = recent.Jid;

                                Add(service);

                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException();
                            }
                    }
                }
            }
        }

        public void LoadItems()
        {
            lock (_recentsLock)
            {
                _recents = Database.GetRecentItems(Settings.Default.UI_MaxRecentItems);
            }
            
            Build();
        }

        public void SaveItems()
        {
            int i = 0;

            foreach (Recent recent in _recents)
            {
                Database.SaveRecent(recent, i++);
            }
        }
    }
}