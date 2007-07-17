using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP.protocol.iq.disco;

namespace xeus2.xeus.Core
{
    internal class MucMark
    {
        private string _nick;
        private string _jid;
        private string _password;
        private string _name;
        private Service _service;
        private DateTime _time;

        public MucMark(Service service)
        {
            _jid = service.Jid.Bare;
            _name = service.Name;

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
                    return Jid;
                }

                return _name;
            }
        }

        public string Jid
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
        }

        public DiscoInfo DiscoInfo
        {
            set
            {
                _service = new Service(new DiscoItem(), false);
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
    }
}
