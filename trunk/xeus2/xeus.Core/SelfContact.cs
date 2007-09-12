using System;
using System.Windows.Media.Imaging;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.vcard;
using xeus.Data;

namespace xeus2.xeus.Core
{
    public class SelfContact : NotifyInfoDispatcher, IContact
    {
        private XmppClientConnection _clientConnection = null;
        private BitmapImage _image;
        private string _fullName;
        private string _nickName;

        #region IContact Members

        public Jid Jid
        {
            get
            {
                return _clientConnection.MyJID;
            }
        }

        public Presence Presence
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Priority
        {
            get
            {
                return _clientConnection.Priority;
            }
        }

        public string Resource
        {
            get
            {
                return _clientConnection.Resource;
            }
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

        public string Group
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsAvailable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Show
        {
            get
            {
                return _clientConnection.Show.ToString();
            }
        }

        public string StatusText
        {
            get
            {
                return _clientConnection.Status;
            }

            set
            {
                _clientConnection.Status = value;
            }
        }

        public string XStatusText
        {
            get
            {
                throw new NotImplementedException();
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

        #endregion

        public SelfContact(XmppClientConnection clientConnection)
        {
            _clientConnection = clientConnection;
        }

        public void StatusChange()
        {
            NotifyPropertyChanged("Show");
            NotifyPropertyChanged("StatusText");
        }

        public void AskMyVcard()
        {
            VcardIq viq = new VcardIq(IqType.get, Jid);
            _clientConnection.IqGrabber.SendIq(viq, new IqCB(VcardResult), null);
        }

        private void VcardResult(object sender, IQ iq, object data)
        {
            if (iq.Type == IqType.error || iq.Error != null)
            {
                if (iq.Error.Code == ErrorCode.NotFound)
                {
                    SetMyVcard(null);
                }
                else
                {
                    Events.Instance.OnEvent(this,
                                            new EventError("Error receiving my V-Card", null));
                }
            }
            else if (iq.Type == IqType.result)
            {
                SetMyVcard(iq.Vcard);

                //save it
                if (iq.Vcard != null)
                {
                    Storage.CacheVCard(iq.Vcard, Jid.Bare);
                }
            }
        }

        private void SetMyVcard(Vcard vcard)
        {
            if (vcard != null)
            {
                string hash;
                _image = Storage.ImageFromPhoto(vcard.Photo, out hash);

                _fullName = vcard.Fullname;
                _nickName = vcard.Nickname;
            
                NotifyPropertyChanged("FullName");
                NotifyPropertyChanged("NickName");
                NotifyPropertyChanged("DisplayName");
            }

            if (_image == null)
            {
                _image = Storage.GetDefaultAvatar();
            }

            NotifyPropertyChanged("Image");
            NotifyPropertyChanged("IsImageTransparent");
        }

        public void LoadMyAvatar()
        {
            NotifyPropertyChanged("Resource");
            NotifyPropertyChanged("Priority");
            NotifyPropertyChanged("Jid");
 
            Vcard vcard = Storage.GetVcard(Jid, 9999);
            SetMyVcard(vcard);
        }
    }
}