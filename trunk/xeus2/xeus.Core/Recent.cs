using System;
using System.Collections.Generic;
using System.Data;
using agsXMPP;
using agsXMPP.protocol.iq.disco;

namespace xeus2.xeus.Core
{
    public enum RecentType
    {
        Chat,
        MUC
    }

    internal class Recent
    {
        private readonly DateTime _dateTime;
        private readonly Jid _jid;
        private readonly RecentType _recentType;

        private IJid _item = null;

        public Recent(Jid jid, RecentType type)
        {
            _jid = jid;
            _recentType = type;
            _dateTime = System.DateTime.Now;

            Build();
        }

        public Recent(IDataRecord reader)
        {
            _dateTime = DateTime.FromBinary((Int64) reader["DateTime"]);
            _recentType = (RecentType) Enum.Parse(typeof (RecentType), (string) reader["Type"]);
            _jid = new Jid((string) reader["Jid"]);

            Build();
        }

        void Build()
        {
            switch (RecentType)
            {
                case RecentType.Chat:
                    {
                        lock (Roster.Instance.Items._syncObject)
                        {
                            Contact contact = Roster.Instance.FindContact(Jid);

                            if (contact != null)
                            {
                                MetaContact metaContact = Roster.Instance.FindMetaContact(contact.Jid);

                                if (metaContact != null)
                                {
                                    _item = metaContact;
                                }
                            }
                        }
                        break;
                    }
                case RecentType.MUC:
                    {
                        Service service = new Service(new DiscoItem(), false);
                        service.DiscoItem.Jid = Jid;

                        _item = service;

                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public Dictionary<string, object> GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("Type", RecentType.ToString());
            data.Add("DateTime", DateTime.ToBinary());
            data.Add("Jid", _jid.Bare);

            return data;
        }

        public RecentType RecentType
        {
            get
            {
                return _recentType;
            }
        }

        public DateTime DateTime
        {
            get
            {
                return _dateTime;
            }
        }

        public Jid Jid
        {
            get
            {
                return _jid;
            }
        }

        public IJid Item
        {
            get
            {
                return _item;
            }
        }
    }
}