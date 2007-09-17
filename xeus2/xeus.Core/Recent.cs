using System;
using System.Collections.Generic;
using System.Data;
using agsXMPP;

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
        private DateTime _DateTime;

        public Recent(Jid jid, RecentType type)
        {
            _jid = jid;
            _recentType = type;
            _dateTime = System.DateTime.Now;
        }

        public Recent(IDataRecord reader)
        {
            _dateTime = DateTime.FromBinary((Int64) reader["DateTime"]);
            _recentType = (RecentType) Enum.Parse(typeof (RecentType), (string) reader["Type"]);
            _jid = new Jid((string) reader["Jid"]);
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
            set
            {
                _DateTime = value;
            }
        }

        public Jid Jid
        {
            get
            {
                return _jid;
            }
        }
    }
}