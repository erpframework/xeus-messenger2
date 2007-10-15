using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Windows.Media.Imaging;
using agsXMPP;
using agsXMPP.protocol.Base;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.caps;
using agsXMPP.protocol.iq.disco;
using agsXMPP.protocol.iq.last;
using agsXMPP.protocol.iq.vcard;
using agsXMPP.Xml.Dom;
using xeus2.Properties;
using xeus2.xeus.Data;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    public class Contact : NotifyInfoDispatcher, IContact
    {
        #region Delegates

        public delegate void VcardHandler(Vcard vcard);

        #endregion

        private static readonly PresenceCompare _presenceCompare = new PresenceCompare();
        private readonly Dictionary<string, Presence> _presences = new Dictionary<string, Presence>(3);
        private readonly object _presencesLock = new object();

        private string _searchLowerText = string.Empty;

        private RosterItem _rosterItem = null;
        private readonly int _metaId;
        private string _customName;
        private string _fullName;

        private bool _hasVCardRecieved = false;
        private bool _hasDiscoRecieved = false;

        private BitmapImage _image;
        private string _nickName;
        private Presence _presence = null;
        private string _statusText = "Not available";
        private string _xStatusText;
        private string _avatarHash = String.Empty;

        private DiscoInfo _discoInfo = null;
        private DiscoInfo _extendedDiscoInfo = null;

        public Contact(IDataRecord reader, RosterItem rosterItem)
        {
            _rosterItem = rosterItem;
            _metaId = (int)(Int64)reader["MetaId"];

            if (!reader.IsDBNull(reader.GetOrdinal("CustomName")))
            {
                _customName = (string) reader["CustomName"];
            }

            AskForLastTime();

            BuildSearchText();
        }

        public Contact(RosterItem rosterItem, int metaId)
        {
            _rosterItem = rosterItem;
            _metaId = metaId;

            AskForLastTime();

            BuildSearchText();
        }

        public Contact(Presence presence)
        {
            _rosterItem = new RosterItem();
            _rosterItem.Jid = presence.From;

            BuildSearchText();
        }

        public bool HasVCardRecieved
        {
            get
            {
                return _hasVCardRecieved;
            }

            set
            {
                _hasVCardRecieved = value;
            }
        }

        #region IContact Members

        public Dictionary<string, object> GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("Jid", Jid.Bare);
            data.Add("MetaId", MetaId);
            data.Add("CustomName", CustomName);

            return data;
        }

        public string DisplayName
        {
            get
            {
                if (!TextUtil.IsNullOrEmptyTrimmed(CustomName))
                {
                    return CustomName;
                }
                else if (!TextUtil.IsNullOrEmptyTrimmed(FullName))
                {
                    return FullName;
                }
                else if (!TextUtil.IsNullOrEmptyTrimmed(NickName))
                {
                    return NickName;
                }
                else if (!TextUtil.IsNullOrEmptyTrimmed(Jid.User))
                {
                    return Jid.User;
                }

                return Jid.ToString();
            }
        }

        public Jid Jid
        {
            get
            {
                return _rosterItem.Jid;
            }
        }

        public Jid FullJid
        {
            get
            {
                if (Presence == null)
                {
                    return Jid;
                }
                else
                {
                    return Presence.From;
                }
            }
        }

        private bool _iqAvatarLoadedFromCache = false;

        private Capabilities _caps = null;
        private DateTime? _lastOnline = null ;
        private VCard _card = null;
        private agsXMPP.protocol.iq.version.Version _version = null;

        public Presence Presence
        {
            get
            {
                lock (_presencesLock)
                {
                    return _presence;
                }
            }

            set
            {
                lock (_presencesLock)
                {
                    // find best presence
                    _presences[value.From.ToString()] = value;

                    if (_presences.Count > 1)
                    {
                        Presence[] presences = new Presence[_presences.Values.Count];
                        _presences.Values.CopyTo(presences, 0);

                        Array.Sort(presences, _presenceCompare);

                        _presence = presences[presences.Length - 1];
                    }
                    else
                    {
                        _presence = value;
                    }
                }

                if (_presence == null || _presence.Type != PresenceType.available)
                {
                    _statusText = "Not available";
                }
                else
                {
                    if (_presence.Type == PresenceType.error)
                    {
                        _statusText = "Error";
                    }
                    else
                    {
                        switch (_presence.Show)
                        {
                            case ShowType.away:
                                {
                                    _statusText = "Away";
                                    break;
                                }
                            case ShowType.dnd:
                                {
                                    _statusText = "Do not Disturb";
                                    break;
                                }
                            case ShowType.chat:
                                {
                                    _statusText = "Free for Chat";
                                    break;
                                }
                            case ShowType.xa:
                                {
                                    _statusText = "Extended Away";
                                    break;
                                }
                            default:
                                {
                                    _statusText = "Online";
                                    break;
                                }
                        }

                        if (_presence.Nickname != null
                            && !string.IsNullOrEmpty(_presence.Nickname.Value))
                        {
                            _nickName = _presence.Nickname.Value;
                        }
                    }

                    if (!string.IsNullOrEmpty(_presence.Status))
                    {
                        _xStatusText = _presence.Status;
                    }
                }

                Capabilities capabilities = _presence.SelectSingleElement(typeof(Capabilities)) as Capabilities;

                if (capabilities != null)
                {
                    _caps = capabilities;

                    NotifyPropertyChanged("ClientNode");
                    NotifyPropertyChanged("ClientExtensions");
                }

                NotifyPropertyChanged("Presence");
                NotifyPropertyChanged("IsAvailable");
                NotifyPropertyChanged("StatusText");
                NotifyPropertyChanged("XStatusText");
                BuildSearchText();

                NotifyPropertyChanged("Show");
                NotifyPropertyChanged("Priority");
                NotifyPropertyChanged("Resource");

                if (!_iqAvatarLoadedFromCache)
                {
                    string hash;
                    BitmapImage image = Storage.GetIqAvatar(_presence.From.Bare, out hash);

                    if (image != null)
                    {
                        Image = image;

                        _avatarHash = hash;
                        NotifyPropertyChanged("AvatarHash");
                    }

                    _iqAvatarLoadedFromCache = true;
                }

                if (!IsAvailable)
                {
                    AskForLastTime();
                }
            }
        }

        private void AskForLastTime()
        {
            LastIq lastIq = new LastIq(IqType.get);
            lastIq.From = Account.Instance.Self.Jid;
            lastIq.To = Jid;
            Account.Instance.XmppConnection.IqGrabber.SendIq(lastIq, OnLastIqResult, null);
        }

        void OnLastIqResult(object sender, IQ iq, object data)
        {
            if (iq.Type == IqType.result)
            {
                Last last = iq.Query as Last;

                if (last != null)
                {
                    SetLastOnline(last);
                }
            }
        }


        public string Resource
        {
            get
            {
                if (_presence == null || _presence.From == null
                    || _presence.From.Resource == null
                    || _presence.From.Resource.TrimStart() == String.Empty)
                {
                    return null;
                }
                else
                {
                    return _presence.From.Resource;
                }
            }
        }

        public string Group
        {
            get
            {
                if (IsService)
                {
                    return "Service";
                }

                foreach (Element element in _rosterItem.GetGroups())
                {
                    return element.Value;
                }

                return Resources.Constant_General;
            }
        }

        public bool IsAvailable
        {
            get
            {
                return (_presence != null && _presence.Type == PresenceType.available);
            }
        }

        public string Show
        {
            get
            {
                if (_presence != null)
                {
                    return _presence.Show.ToString();
                }
                else
                {
                    return "NotAvailable";
                }
            }
        }

        public int Priority
        {
            get
            {
                if (_presence == null)
                {
                    return -1;
                }
                else
                {
                    return _presence.Priority;
                }
            }
        }

        public string StatusText
        {
            get
            {
                return _statusText;
            }
        }

        public string XStatusText
        {
            get
            {
                if (string.IsNullOrEmpty(_xStatusText))
                {
                    return StatusText;
                }
                else
                {
                    return _xStatusText;
                }
            }
        }

        public string FullName
        {
            get
            {
                return _fullName;
            }
        }

        public string NickName
        {
            get
            {
                return _nickName;
            }
        }

        public BitmapImage Image
        {
            get
            {
                if (_image == null)
                {
                    if (!IsService)
                    {
                        _image = Storage.GetDefaultAvatar();
                    }
                    else
                    {
                        _image = Storage.GetDefaultServiceImage();
                    }
                }

                return _image;
            }

            private set
            {
                if (value != null)
                {
                    _image = value;

                    NotifyPropertyChanged("Image");
                    NotifyPropertyChanged("IsImageTransparent");
                }
            }
        }

        public bool IsImageTransparent
        {
            get
            {
                if (_image == null)
                {
                    return true;
                }
                else
                {
                    return (_image.Format.Masks.Count >= 4);
                }
            }
        }

        public string CustomName
        {
            get
            {
                return _customName;
            }

            set
            {
                _customName = value;

                Database.SaveContact(this);
            }
        }

        public bool IsService
        {
            get
            {
                return (_rosterItem == null || string.IsNullOrEmpty(_rosterItem.Jid.User));
            }
        }

        public void SetVersion(agsXMPP.protocol.iq.version.Version version)
        {
            _version = version;
            NotifyPropertyChanged("ClientVersion");
            NotifyPropertyChanged("ClientName");
            NotifyPropertyChanged("ClientOS");
        }

        public string ClientVersion
        {
            get
            {
                if (_version == null)
                {
                    return null;
                }
                else
                {
                    return _version.Ver;
                }
            }
        }

        public Capabilities Caps
        {
            get
            {
                return _caps;
            }
            set
            {
                _caps = value;
                NotifyPropertyChanged("Caps");
            }
        }

        public VCard Card
        {
            get
            {
                return _card;
            }
        }

        public DateTime? LastOnlineTime
        {
            get
            {
                return _lastOnline;
            }
        }

        #endregion

        public override string ToString()
        {
            return DisplayName;
        }

        public string AvatarHash
        {
            get
            {
                return _avatarHash;
            }
        }

        public int MetaId
        {
            get
            {
                return _metaId;
            }
        }

        public bool HasDiscoRecieved
        {
            get
            {
                return _hasDiscoRecieved;
            }
        }

        public DiscoInfo Disco
        {
            get
            {
                return _discoInfo;
            }
            set
            {
                _discoInfo = value;
                NotifyPropertyChanged("Disco");
            }
        }

        public DiscoInfo ExtendedDisco
        {
            get
            {
                return _extendedDiscoInfo;
            }
            set
            {
                _extendedDiscoInfo = value;
                NotifyPropertyChanged("ExtendedDisco");
            }
        }

        public bool HasFeature(string feature)
        {
            bool has = (_discoInfo != null && _discoInfo.HasFeature(feature));

            if (!has)
            {
                has = (_extendedDiscoInfo != null && _extendedDiscoInfo.HasFeature(feature));
            }

            return has;
        }

        public string SearchLowerText
        {
            get
            {
                return _searchLowerText;
            }
        }

        public string ClientOS
        {
            get
            {
                if (_version != null)
                {
                    return _version.Os;
                }
                else
                {
                    return null;
                }
            }
        }

        public string ClientName
        {
            get
            {
                if (_version != null)
                {
                    return _version.Name;
                }
                else
                {
                    return null;
                }
            }
        }

        public RosterItem RosterItem
        {
            get
            {
                return _rosterItem;
            }

            set
            {
                _rosterItem = value;

                NotifyPropertyChanged("RosterItem");
                NotifyPropertyChanged("Group");
                NotifyPropertyChanged("IsService");
            }
        }

        void BuildSearchText()
        {
            StringBuilder builder = new StringBuilder();

            if (!string.IsNullOrEmpty(FullName))
            {
                builder.Append(FullName.ToLower());
            }

            if (!string.IsNullOrEmpty(NickName))
            {
                builder.Append(NickName.ToLower());
            }

            if (!string.IsNullOrEmpty(XStatusText))
            {
                builder.Append(XStatusText.ToLower());
            }

            builder.Append(DisplayName.ToLower());

            _searchLowerText = builder.ToString();

            NotifyPropertyChanged("SearchLowerText");
        }

        public void SetVcard(Vcard vcard)
        {
            if (App.Current.Dispatcher.CheckAccess())
            {
                _hasVCardRecieved = true;
                NotifyPropertyChanged("HasVCardRecieved");

                if (vcard != null)
                {
                    _fullName = vcard.Fullname;
                    _nickName = vcard.Nickname;

                    NotifyPropertyChanged("FullName");
                    NotifyPropertyChanged("NickName");
                    NotifyPropertyChanged("DisplayName");

                    BuildSearchText();

                    string hash;
                    Image = Storage.ImageFromPhoto(vcard.Photo, out hash);

                    if (Image != null)
                    {
                        _avatarHash = hash;
                        NotifyPropertyChanged("AvatarHash");
                    }

                    _card = new VCard(vcard, Jid);

                    NotifyPropertyChanged("Card");

                    Storage.CacheVCard(vcard, Jid.Bare);
                }
            }
            else
            {
                App.InvokeSafe(App._dispatcherPriority, new VcardHandler(SetVcard), vcard);
            }
        }

        public void SetLastOnline(Last last)
        {
            _lastOnline = DateTime.Now.Subtract(new TimeSpan(0, 0, last.Seconds));

            NotifyPropertyChanged("LastOnlineTime");
        }

        public void SetIqAvatar(byte[] data)
        {
            string hash;
            Image = Storage.BitmapFromBytes(data, out hash);

            if (Image != null)
            {
                _avatarHash = hash;
                NotifyPropertyChanged("AvatarHash");
            }
        }
    }
}