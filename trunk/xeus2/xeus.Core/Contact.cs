using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using agsXMPP;
using agsXMPP.protocol.Base;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.vcard;
using agsXMPP.Xml.Dom;
using xeus.Data;
using xeus2.Properties;
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
        private string _customName;
        private string _fullName;

        private bool _hasVCardRecieved = false;
        private BitmapImage _image;
        private string _nickName;
        private Presence _presence = null;
        private string _statusText = "Not available";
        private string _xStatusText;

        public Contact(RosterItem rosterItem)
        {
            _rosterItem = rosterItem;
        }

        public bool HasVCardRecieved
        {
            get
            {
                return _hasVCardRecieved;
            }
        }

        #region IContact Members

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

                NotifyPropertyChanged("Presence");
                NotifyPropertyChanged("IsAvailable");
                NotifyPropertyChanged("StatusText");
                NotifyPropertyChanged("XStatusText");
                NotifyPropertyChanged("Show");
                NotifyPropertyChanged("Priority");
                NotifyPropertyChanged("Resource");
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
        }

        public bool IsService
        {
            get
            {
                return (_rosterItem == null || string.IsNullOrEmpty(_rosterItem.Jid.User));
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} / {1}", Jid, Presence.Status);
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

                    BitmapImage image = Storage.ImageFromPhoto(vcard.Photo);

                    if (image != null)
                    {
                        _image = image;

                        NotifyPropertyChanged("Image");
                        NotifyPropertyChanged("IsImageTransparent");
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
    }
}