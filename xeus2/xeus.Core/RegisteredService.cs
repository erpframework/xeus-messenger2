using System;
using agsXMPP;
using agsXMPP.protocol.iq.disco;

namespace xeus2.xeus.Core
{
    internal class RegisteredService : Service
    {
        private string _userId = String.Empty;

        public RegisteredService(DiscoItem discoItem)
            : base(discoItem, false)
        {
            _askedForDiscovery = true;
        }

        public string UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
                NotifyPropertyChanged("UserId");
            }
        }

        public Jid UserNewJid
        {
            get
            {
                return new Jid(string.Format("{0}@{1}", UserId.Trim(), Jid.Bare));
            }
        }
    }
}