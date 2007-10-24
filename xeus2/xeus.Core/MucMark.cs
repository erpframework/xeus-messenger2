using System;
using agsXMPP;
using agsXMPP.protocol.extensions.bookmarks;
using agsXMPP.protocol.iq.disco;

namespace xeus2.xeus.Core
{
    internal class MucMark : IJid
    {
        private string _nick;
        private readonly Jid _jid;
        private string _password;
        private readonly string _name;
        private Service _service;
        private readonly DateTime _time;

        private bool _autoJoin = false;

        public MucMark(Service service)
        {
            _jid = service.Jid;
            _name = service.Name;

            _time = DateTime.Now;
        }

        public MucMark(Jid jid)
        {
            _jid = jid;

            _time = DateTime.Now;
        }

        public MucMark(Conference conference)
        {
            _nick = conference.Nickname;

            if (!string.IsNullOrEmpty(conference.Password))
            {
                _password = conference.Password;
            }

            _jid = conference.Jid;
            _name = conference.Name;
            _autoJoin = conference.AutoJoin;
            _time = DateTime.Now;
        }

        public MucMark(Service service, string nick, string password) : this(service)
        {
            _nick = nick;
            _password = password;
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return Jid.ToString();
                }

                return _name;
            }
        }

        public Jid Jid
        {
            get
            {
                return _jid;
            }
        }

        public string Nick
        {
            get
            {
                return _nick;
            }
            set
            {
                _nick = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        public DiscoInfo DiscoInfo
        {
            set
            {
                DiscoItem discoItem = new DiscoItem();
                discoItem.Jid = _jid;
                _service = new Service(discoItem, false);

                Service.DiscoInfo = value;
            }
        }

        public Service Service
        {
            get
            {
                return _service;
            }
        }

        public DateTime Time
        {
            get
            {
                return _time;
            }
        }

        public string Description
        {
            get
            {
                return string.Format("{0} ({1})", Name, Time);
            }
        }

        public bool AutoJoin
        {
            get
            {
                return _autoJoin;
            }

            set
            {
                _autoJoin = value;
            }
        }
    }
}