using System;
using System.Collections.Generic;
using System.Text;

namespace xeus2.xeus.Core
{
    internal class MucMark
    {
        private string _nick;
        private string _jid;
        private string _password;
        private string _name;

        public MucMark(Service service)
        {
            _jid = service.Jid.Bare;
            _name = service.Name;
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
    }
}
