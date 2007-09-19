using System;
using System.Windows.Media.Imaging;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.caps;
using agsXMPP.protocol.iq.disco;
using xeus2.xeus.Data;

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
                return "1.0";
            }
        }

        public string ClientNode
        {
            get
            {
                return "http://xeus.net/caps";
            }
        }

        public string[] ClientExtensions
        {
            get
            {
                return null;
            }
        }

        public Capabilities Caps
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DiscoInfo Disco
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public VCard Card
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? LastOnlineTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        #endregion
    }
}