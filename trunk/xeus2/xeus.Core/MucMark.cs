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

        public MucMark(Service service)
        {
            _jid = service.Jid.Bare;
        }

        public MucMark(Service service, string nick, string password) : this(service)
        {
            _nick = nick;
            _password = password;
        }
    }
}
