using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Media.Imaging;
using agsXMPP;
using agsXMPP.protocol.Base;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.caps;
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

        private readonly RosterItem _rosterItem = null;
        private readonly int _metaId;
        private string _customName;
        private string _fullName;

        private bool _hasVCardRecieved = false;
        private BitmapImage _image;
        private string _nickName;
        private Presence _presence = null;
        private string _statusText = "Not available";
        private string _xStatusText;
        private string _avatarHash = String.Empty;

        public Contact(IDataRecord reader, RosterItem rosterItem)
        {
            _rosterItem = rosterItem;
            _metaId = (int)(Int64)reader["MetaId"];

            if (!reader.IsDBNull(reader.GetOrdinal("CustomName")))
            {
                _customName = (string) reader["CustomName"];
            }
        }

        public Contact(RosterItem rosterItem, int metaId)
        {
            _rosterItem = rosterItem;
            _metaId = metaId;
        }

        public Contact(Presence presence)
        {
            _rosterItem = new RosterItem();
            _rosterItem.Jid = presence.From;
        }

        public bool HasVCardRecieved
        {
            get
            {
                return _hasVCardRecieved;
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
                if (!string.IsNullOrEmpty(CustomName))
                {
                    return CustomName;
                }
                else if (!string.IsNullOrEmpty(FullName))
                {
                    return FullName;
                }
                else if (!string.IsNullOrEmpty(NickName))
                {
                    return NickName;
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

        private Capabilities _capabilities;

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
                    _capabilities = capabilities;

                    NotifyPropertyChanged("ClientVersion");
                    NotifyPropertyChanged("ClientNode");
                    NotifyPropertyChanged("ClientExtensions");
                }

                NotifyPropertyChanged("Presence");
                NotifyPropertyChanged("IsAvailable");
                NotifyPropertyChanged("StatusText");
                NotifyPropertyChanged("XStatusText");
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

        public string ClientVersion
        {
            get
            {
                if (_capabilities == null)
                {
                    return null;
                }
                else
                {
                    return _capabilities.Version;
                }
            }
        }

        public string ClientNode
        {
            get
            {
                if (_capabilities == null)
                {
                    return null;
                }
                else
                {
                    return _capabilities.Node;
                }
            }
        }

        public string[] ClientExtensions
        {
            get
            {
                if (_capabilities == null)
                {
                    return null;
                }
                else
                {
                    return _capabilities.Extensions;
                }
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} / {1}", Jid, Presence.Status);
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

        public void SetVcard(Vcard vcard)
        {
            if (App.Current.Dispatcher.CheckAccess())
            {
                _hasVCardRecieved = true;
                NotifyPropertyChanged("HasVCardRecieved");

                if (vcard != null)
                {
                    /*
                        Birthday = vcard.Birthday;
                        Description = vcard.Description;

                        Email email = vcard.GetPreferedEmailAddress();
                        if (email != null)
                        {
                            EmailPreferred = email.UserId;
                        }
                         */

                    _fullName = vcard.Fullname;
                    _nickName = vcard.Nickname;

                    NotifyPropertyChanged("FullName");
                    NotifyPropertyChanged("NickName");
                    NotifyPropertyChanged("DisplayName");

                    string hash;
                    Image = Storage.ImageFromPhoto(vcard.Photo, out hash);

                    if (Image != null)
                    {
                        _avatarHash = hash;
                        NotifyPropertyChanged("AvatarHash");
                    }

                    /*
                        Organization organization = vcard.Organization;
                        if (organization != null)
                        {
                            Organization = vcard.Organization.Name;
                        }

                    
                        Role = vcard.Role;
                        Title = vcard.Title;
                        Url = vcard.Url;
                        */
                }
            }
            else
            {
                App.InvokeSafe(App._dispatcherPriority, new VcardHandler(SetVcard), vcard);
            }
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