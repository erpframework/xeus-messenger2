using System;
using System.Windows.Media.Imaging;
using agsXMPP;
using agsXMPP.protocol.client;
using xeus2.xeus.Data;
using Version=agsXMPP.protocol.iq.version.Version;

namespace xeus2.xeus.Core
{
    public class DummyContact : IContact
    {
        #region IContact Members

        public Jid Jid
        {
            get
            {
                return new Jid("john.smith@jabber.org");
            }
        }

        public Jid FullJid
        {
            get
            {
                return new Jid("john.smith", "jabber.org", "home");
            }
        }

        public Presence Presence
        {
            get
            {
                return new Presence(ShowType.xa, "I am away", 1);
            }
        }

        public int Priority
        {
            get
            {
                return 1;
            }
        }

        public string Resource
        {
            get
            {
                return "At Home";
            }
        }

        public string DisplayName
        {
            get
            {
                return "John Smith";
            }
        }

        public string Group
        {
            get
            {
                return "Group";
            }
        }

        public bool IsAvailable
        {
            get
            {
                return true;
            }
        }

        public string Show
        {
            get
            {
                return ShowType.xa.ToString();
            }
        }

        public string StatusText
        {
            get
            {
                return "Away";
            }
        }

        public string XStatusText
        {
            get
            {
                return "I am away";
            }
        }

        public string FullName
        {
            get
            {
                return "John Smith";
            }
        }

        public string NickName
        {
            get
            {
                return "jOhnnnie";
            }
        }

        public BitmapImage Image
        {
            get
            {
                return Storage.GetDefaultAvatar();
            }
        }

        public bool IsImageTransparent
        {
            get
            {
                return true;
            }
        }

        public string CustomName
        {
            get
            {
                return null;
            }
        }

        public bool IsService
        {
            get
            {
                return false;
            }
        }

        public string ClientVersion
        {
            get
            {
                return Account.Instance.Self.ClientVersion;
            }
        }

        public VCard Card
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        RelativeOldness _relativeOldness = new RelativeOldness();

        public RelativeOldness LastOnline
        {
            get
            {
                return _relativeOldness;
            }
        }

        public bool HasFeature(string feature)
        {
            return false;
        }

        public string SearchLowerText
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ClientOS
        {
            get
            {
                return Account.Instance.Self.ClientOS;
            }
        }

        public string ClientName
        {
            get
            {
                return Account.Instance.Self.ClientName;
            }
        }

        public void SetVersion(Version version)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}