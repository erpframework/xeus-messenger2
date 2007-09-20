using System;
using System.Reflection;
using System.Timers;
using System.Windows.Media.Imaging;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.caps;
using agsXMPP.protocol.iq.disco;
using agsXMPP.protocol.iq.vcard;
using agsXMPP.protocol.x.vcard_update;
using xeus2.Properties;
using xeus2.xeus.Data;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    public class SelfContact : NotifyInfoDispatcher, IContact
    {
        private readonly Timer _updateTimer = new Timer(500);

        private BitmapImage _image;
        private string _fullName;
        private string _nickName;
        
        private readonly Capabilities _caps;

        private readonly DiscoIdentity _identity = new DiscoIdentity("pc", "xeus", "client");
        private readonly DiscoInfo _discoInfo = new DiscoInfo();
        
        private VCard _card = null;

        #region IContact Members

        public Jid Jid
        {
            get
            {
                return Account.Instance.XmppConnection.MyJID;
            }
        }

        public Presence Presence
        {
            get
            {
                _presence = new Presence(Settings.Default.XmppMyPresence,
                                                    Settings.Default.XmppStatusText,
                                                    Settings.Default.XmppPriority);
                if (_avatarHash != null)
                {
                    _presence.AddChild(new VcardUpdate(_avatarHash));
                }

                _presence.AddChild(_caps);

                return _presence;
            }
        }

        public int Priority
        {
            get
            {
                return _presence.Priority;
            }

            set
            {
                Settings.Default.XmppPriority = value;
                
            }
        }


        public string Resource
        {
            get
            {
                return Account.Instance.XmppConnection.Resource;
            }

            set
            {
                Settings.Default.XmppResource = value;
                RestartTimer();
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
                return _presence.Show.ToString();
            }
        }

        public ShowType MyShow
        {
            get
            {
                return _presence.Show;
            }

            set
            {
                Settings.Default.XmppMyPresence = value;
                RestartTimer();
            }
        }

        public string StatusText
        {
            get
            {
                return _presence.Status;
            }

            set
            {
                Settings.Default.XmppStatusText = value;
                RestartTimer();
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

            set
            {
                _image = value;
                NotifyPropertyChanged("Image");

                _card.SetImage(value);
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

        public string ClientVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
                throw new NotImplementedException();
            }
        }

        public VCard Card
        {
            get
            {
                return _card;
            }

            set
            {
                _card = value;
                NotifyPropertyChanged("Card");
            }
        }

        public DateTime? LastOnlineTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Capabilities Caps
        {
            get
            {
                return _caps;
            }
        }

        public bool HasFeature(string feature)
        {
            return _discoInfo.HasFeature(feature);
        }

        public void PresenceChange()
        {
            NotifyPropertyChanged("Show");
            NotifyPropertyChanged("StatusText");
            NotifyPropertyChanged("MyShow");
            NotifyPropertyChanged("Priority");
            NotifyPropertyChanged("Resource");
        }

        #endregion


        public SelfContact()
        {
            _updateTimer.AutoReset = false;
            _updateTimer.Elapsed += _updateTimer_Elapsed;

            _caps = new Capabilities(TextUtil.GenerateVerAttribute(_discoInfo),
                                                                        "http://xeus.net/#2.0");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.BYTESTREAMS));
            _caps.AddExtension("bs");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.CAPS));
            _caps.AddExtension("caps");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.COMMANDS));
            _caps.AddExtension("cmd");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.CHATSTATES));
            _caps.AddExtension("cs");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.DISCO_INFO));
            _caps.AddExtension("di");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.VCARD_UPDATE));
            _caps.AddExtension("vcup");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.VCARD));
            _caps.AddExtension("vc");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.IQ_LAST));
            _caps.AddExtension("las");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.MUC_ADMIN));
            _caps.AddExtension("adm");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.MUC_OWNER));
            _caps.AddExtension("own");

            _discoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.MUC_USER));
            _caps.AddExtension("usr");

            _discoInfo.AddIdentity(_identity);
        }

        static void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Account.Instance.SendMyPresence();
        }

        public void AskMyVcard()
        {
            VcardIq viq = new VcardIq(IqType.get, Jid);
            Account.Instance.XmppConnection.IqGrabber.SendIq(viq, new IqCB(VcardResult), null);
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
            }
        }

        void RestartTimer()
        {
            _updateTimer.Stop();
            _updateTimer.Start();
        }

        private string _avatarHash = null;
        private Presence _presence= new Presence(Settings.Default.XmppMyPresence,
                                                    Settings.Default.XmppStatusText,
                                                    Settings.Default.XmppPriority);

        private void SetMyVcard(Vcard vcard)
        {
            if (vcard != null)
            {
                _image = Storage.ImageFromPhoto(vcard.Photo, out _avatarHash);

                _fullName = vcard.Fullname;
                _nickName = vcard.Nickname;
            
                NotifyPropertyChanged("FullName");
                NotifyPropertyChanged("NickName");
                NotifyPropertyChanged("DisplayName");

                Storage.CacheVCard(vcard, Jid.Bare);
            }

            if (_image == null)
            {
                _image = Storage.GetDefaultAvatar();
            }

            NotifyPropertyChanged("Image");
            NotifyPropertyChanged("IsImageTransparent");

            _card = new VCard(vcard, Jid);

            // avatar could be changed
            // Account.Instance.SendMyPresence();
        }

        public void LoadMyAvatar()
        {
            NotifyPropertyChanged("Resource");
            NotifyPropertyChanged("Priority");
            NotifyPropertyChanged("Jid");
 
            Vcard vcard = Storage.GetVcard(Jid, 99999);
            SetMyVcard(vcard);
        }

        public void PublishVCard()
        {
            VcardIq vcardIq = new VcardIq(IqType.set, _card.Vcard);
            Account.Instance.XmppConnection.Send(vcardIq);

            Account.Instance.SendMyPresence();           
        }
    }
}