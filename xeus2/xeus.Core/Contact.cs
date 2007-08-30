using System;
using agsXMPP;
using agsXMPP.protocol.Base;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.vcard;
using agsXMPP.Xml.Dom;
using xeus2.Properties;

namespace xeus2.xeus.Core
{
    public class Contact : NotifyInfoDispatcher, IContact
    {
        public delegate void VcardHandler(Vcard vcard);

        private RosterItem _rosterItem = null;
        private Presence _presence = new Presence();
        private string _customName;
        private string _xStatusText;

        private bool _hasVCardRecieved = false;

        public Contact(RosterItem rosterItem)
        {
            _rosterItem = rosterItem;
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

        public Presence Presence
        {
            get
            {
                return _presence;
            }

            set
            {
                _presence = value;

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

                        if (_presence.Status != null && _presence.Status != String.Empty)
                        {
                            _xStatusText = _presence.Status;
                        }
                        else
                        {
                            _xStatusText = _statusText;
                        }
                    }
                }

                NotifyPropertyChanged("Presence");
                NotifyPropertyChanged("StatusText");
                NotifyPropertyChanged("XStatusText");
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

        private string _statusText = "Not available";
        private string _fullName;

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
                return _xStatusText;
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
                if (_presence.Nickname == null)
                {
                    return String.Empty;
                }

                return _presence.Nickname.ToString();
            }
        }

        public string CustomName
        {
            get
            {
                return _customName;
            }
        }

        public bool HasVCardRecieved
        {
            get
            {
                return _hasVCardRecieved;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} / {1}", Jid, Presence.Status);
        }

        public void SetVcard(Vcard vcard)
        {
            if (vcard != null)
            {
                if (App.Current.Dispatcher.CheckAccess())
                {
                    _hasVCardRecieved = true;

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

                    NotifyPropertyChanged("FullName");
                    NotifyPropertyChanged("HasVCardRecieved");

                    // _nickName = vcard.Nickname;

                    /*
                    Organization organization = vcard.Organization;
                    if (organization != null)
                    {
                        Organization = vcard.Organization.Name;
                    }

                    
                    Role = vcard.Role;
                    Title = vcard.Title;
                    Url = vcard.Url;
                    
                    BitmapImage image = Storage.ImageFromPhoto(vcard.Photo);

                    Image = image;
                     */
                }
                /*
                if (Image == null)
                {
                    if (!IsInitialized)
                    {
                        Image = Storage.GetDefaultAvatar();
                    }
                    else if (IsService)
                    {
                        Image = Storage.GetDefaultServiceAvatar();
                    }
                    else
                    {
                        Image = Storage.GetDefaultAvatar();
                    }
                }*/
            }
            else
            {
                App.InvokeSafe(App._dispatcherPriority, new VcardHandler(SetVcard), vcard);
            }
        }
    }
}
