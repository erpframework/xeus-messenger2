using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.iq.disco;
using xeus2.Properties;
using xeus2.xeus.Data;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    internal class RecentItems : ObservableCollectionDisp<Recent>
    {
        readonly static RecentItems _instance = new RecentItems();

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
            lock (_syncObject)
            {
                Remove(jid);

                if (Count >= Settings.Default.UI_MaxRecentItems)
                {
                    RemoveAt(Count - 1);
                }

                Recent recent = new Recent(jid, recentType);
                Insert(0, recent);
            }
        }

        void Remove(Jid jid)
        {
            lock (_syncObject)
            {
                List<Recent> toBeRemoved = new List<Recent>();

                foreach (Recent recent in this)
                {
                    if (JidUtil.BareEquals(recent.Jid, jid))
                    {
                        toBeRemoved.Add(recent);
                    }
                }

                foreach (Recent recent in toBeRemoved)
                {
                    Remove(recent);
                }
            }
        }

        public void LoadItems()
        {
            lock (_syncObject)
            {
                Clear();

                List<Recent> recents = Database.GetRecentItems(Settings.Default.UI_MaxRecentItems);

                foreach (Recent recent in recents)
                {
                    Add(recent);    
                }
            }
        }

        public void SaveItems()
        {
            int i = 0;

            foreach (Recent recent in this)
            {
                Database.SaveRecent(recent, i++);
            }
        }
    }
}