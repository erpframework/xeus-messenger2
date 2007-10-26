using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.disco;
using agsXMPP.protocol.x.data;
using agsXMPP.Xml.Dom;
using xeus2.Properties;
using Uri=agsXMPP.Uri;

namespace xeus2.xeus.Core
{
    internal enum MucFeature
    {
        muc_passwordprotected,
        muc_hidden,
        muc_temporary,
        muc_open,
        muc_unmoderated,
        muc_nonanonymous
    }

    internal class Service : NotifyInfoDispatcher, IJid
    {
        private readonly ObservableCollectionDisp<Service> _commands = new ObservableCollectionDisp<Service>();
        private readonly DiscoItem _discoItem = null;

        private readonly bool _isToplevel = false;
        private bool _isRegistered = false;

        private readonly Services _services = new Services();
        protected bool _askedForDiscovery = false;
        private DiscoInfo _discoInfo = null;
        private IQ _errorIq = null;
        private bool _isDiscovered = false;
        private bool? _isMucmarked = null;
        private MucRoomInfo _mucInfo;
        private agsXMPP.protocol.x.data.Data _xData = null;

        public Service(DiscoItem discoItem, bool isToplevel)
        {
            _discoItem = discoItem;
            _isToplevel = isToplevel;

            if (_isToplevel)
            {
                Account.Instance.AddDiscoInfoPrioritized(_discoItem);

                _askedForDiscovery = true;
            }
        }

        public string Key
        {
            get
            {
                return GetKey(_discoItem);
            }
        }

        public string MucInfoDescription
        {
            get
            {
                if (MucInfo != null)
                {
                    return MucInfo.Description;
                }

                return null;
            }
        }

        public int MucInfoOccupants
        {
            get
            {
                if (MucInfo != null)
                {
                    return MucInfo.Occupants;
                }

                return 0;
            }
        }

        public bool IsBytestremProxy
        {
            get
            {
                if (_discoInfo == null
                    || _discoInfo.GetIdentities() == null)
                {
                    return false;
                }

                return string.IsNullOrEmpty(Jid.User) && _discoInfo.HasFeature(Uri.BYTESTREAMS);

                /*
                foreach (DiscoIdentity identity in _discoInfo.GetIdentities())
                {
                    if (identity.Category == "proxy"
                        && identity.Type == "bytestreams")
                    {
                        return true;
                    }
                }
                return false;*/
            }
        }

        public List<string> Categories
        {
            get
            {
                List<string> categories = new List<string>();

                if (_discoInfo != null)
                {
                    foreach (DiscoIdentity identity in _discoInfo.GetIdentities())
                    {
                        categories.Add(identity.Category);
                    }
                }

                return categories;
            }
        }

        public string Name
        {
            get
            {
                if (!_askedForDiscovery)
                {
                    Account.Instance.AddDiscoInfo(_discoItem);

                    _askedForDiscovery = true;
                }

                if (DiscoInfo == null)
                {
                    if (ErrorIq == null)
                    {
                        return DiscoItem.Jid.ToString();
                    }
                    else
                    {
                        return string.Format(Resources.Error_CodeMsg, ErrorIq.Error.Code, ErrorIq.Error.LastNode.Value);
                    }
                }

                DiscoIdentity[] discoIdentities = DiscoInfo.GetIdentities();

                if (!String.IsNullOrEmpty(DiscoItem.Name))
                {
                    return DiscoItem.Name;
                }
                else if (discoIdentities.Length > 0 && !string.IsNullOrEmpty(discoIdentities[0].Name))
                {
                    return discoIdentities[0].Name;
                }
                else if (!String.IsNullOrEmpty(Node))
                {
                    return Node;
                }

                return DiscoItem.Jid.ToString();
            }
        }

        public string Group
        {
            get
            {
                if (DiscoInfo != null)
                {
                    DiscoIdentity[] discoIdentities = DiscoInfo.GetIdentities();

                    if (discoIdentities.Length > 0)
                    {
                        return discoIdentities[0].Category;
                    }
                }

                return Resources.Constant_General;
            }
        }

        public Services Services
        {
            get
            {
                return _services;
            }
        }

        public virtual DiscoInfo DiscoInfo
        {
            get
            {
                return _discoInfo;
            }

            set
            {
                _discoInfo = value;

                NotifyPropertyChanged("DiscoInfo");
                NotifyPropertyChanged("IsCommand");
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("Group");
                NotifyPropertyChanged("IsRegistrable");
                NotifyPropertyChanged("IsSearchable");
                NotifyPropertyChanged("IsChatRoom");
                NotifyPropertyChanged("IsMucPasswordProtected");
                NotifyPropertyChanged("IsMucHidden");
                NotifyPropertyChanged("IsMucTemporary");
                NotifyPropertyChanged("IsMucOpen");
                NotifyPropertyChanged("MucUnmoderated");
                NotifyPropertyChanged("IsMucNonAnonymous");
                NotifyPropertyChanged("Categories");
                NotifyPropertyChanged("Type");
            }
        }

        public string Type
        {
            get
            {
                if (_discoInfo != null
                    && _discoInfo.GetIdentities().Length > 0)
                {
                    return _discoInfo.GetIdentities()[0].Type;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsDiscovered
        {
            get
            {
                return _isDiscovered;
            }

            set
            {
                bool notify = (_isDiscovered != value);

                _isDiscovered = value;

                if (notify)
                {
                    NotifyPropertyChanged("IsDiscovered");
                }
            }
        }

        public IQ ErrorIq
        {
            get
            {
                return _errorIq;
            }

            set
            {
                _errorIq = value;
            }
        }

        public string Node
        {
            get
            {
                return DiscoItem.Node;
            }
        }

        public DiscoItem DiscoItem
        {
            get
            {
                return _discoItem;
            }
        }

        public bool IsCommand
        {
            get
            {
                return (_discoInfo != null && _discoInfo.HasFeature(Uri.COMMANDS));
            }
        }

        public bool IsChatRoom
        {
            get
            {
                if (_discoInfo == null)
                {
                    return false;
                }

                return _discoInfo.HasFeature(Uri.MUC);
            }
        }

        public bool IsTransport
        {
            get
            {
                if (Categories == null)
                {
                    return false;
                }

                foreach (string category in Categories)
                {
                    if (category == "gateway")
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsRegistrable
        {
            get
            {
                if (_discoInfo == null)
                {
                    return false;
                }

                return _discoInfo.HasFeature(Uri.IQ_REGISTER);
            }
        }

        public bool IsSearchable
        {
            get
            {
                if (_discoInfo == null || IsCommand)
                {
                    return false;
                }

                return _discoInfo.HasFeature(Uri.IQ_SEARCH);
            }
        }


        public bool IsMucPasswordProtected
        {
            get
            {
                return (DiscoInfo != null && DiscoInfo.HasFeature(MucFeature.muc_passwordprotected.ToString()));
            }
        }

        public bool IsMucHidden
        {
            get
            {
                return (DiscoInfo != null && DiscoInfo.HasFeature(MucFeature.muc_hidden.ToString()));
            }
        }

        public bool IsMucTemporary
        {
            get
            {
                return (DiscoInfo != null && DiscoInfo.HasFeature(MucFeature.muc_temporary.ToString()));
            }
        }

        public bool IsMucOpen
        {
            get
            {
                return (DiscoInfo != null && DiscoInfo.HasFeature(MucFeature.muc_open.ToString()));
            }
        }

        public bool IsMucModerated
        {
            get
            {
                return !(DiscoInfo != null && DiscoInfo.HasFeature(MucFeature.muc_unmoderated.ToString()));
            }
        }

        public bool IsMucNonAnonymous
        {
            get
            {
                return (DiscoInfo != null && DiscoInfo.HasFeature(MucFeature.muc_nonanonymous.ToString()));
            }
        }

        public bool IsToplevel
        {
            get
            {
                return _isToplevel;
            }
        }

        public ObservableCollectionDisp<Service> Commands
        {
            get
            {
                return _commands;
            }
        }

        public agsXMPP.protocol.x.data.Data XData
        {
            get
            {
                return _xData;
            }
            set
            {
                _xData = value;

                _mucInfo = new MucRoomInfo(_xData);

                NotifyPropertyChanged("XData");
                NotifyPropertyChanged("XDataCollection");
                NotifyPropertyChanged("MucInfo");
            }
        }

        public List<Field> XDataCollection
        {
            get
            {
                if (_xData == null)
                {
                    return null;
                }

                List<Field> collection = new List<Field>();

                foreach (Node node in _xData.ChildNodes)
                {
                    Field field = node as Field;

                    if (field != null
                        && field.Label != null
                        && field.GetValues().Length > 0
                        && field.Var != null)
                    {
                        collection.Add(field);
                    }
                }

                return collection;
            }
        }

        public MucRoomInfo MucInfo
        {
            get
            {
                return _mucInfo;
            }
        }

        public bool IsMucMarked
        {
            get
            {
                if (_isMucmarked == null)
                {
                    _isMucmarked = MucMarks.Instance.IsBookmarked(this);
                }

                return (bool) _isMucmarked;
            }

            set
            {
                _isMucmarked = value;
                NotifyPropertyChanged("IsMucMarked");
            }
        }

        #region IJid Members

        public Jid Jid
        {
            get
            {
                return DiscoItem.Jid;
            }
        }

        public bool IsRegistered
        {
            get
            {
                return _isRegistered;
            }
            set
            {
                _isRegistered = value;
                NotifyPropertyChanged("IsRegistered");
            }
        }

        #endregion

        public static string GetKey(DiscoItem discoItem)
        {
            if (string.IsNullOrEmpty(discoItem.Node))
            {
                return
                    string.Format("{0}/", discoItem.Jid.ToString().ToLowerInvariant());
            }
            else
            {
                return
                    string.Format("{0}/{1}", discoItem.Jid.ToString().ToLowerInvariant(),
                                  discoItem.Node.ToLowerInvariant());
            }
        }

        public static string GetKey(Jid jid)
        {
            return string.Format("{0}/", jid.ToString().ToLowerInvariant());
        }

        public override string ToString()
        {
            return Name;
        }
    }
}