using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using agsXMPP;
using agsXMPP.protocol.client;
using FastDynamicPropertyAccessor;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    internal class MetaContact : NotifyInfoDispatcher, IContact
    {
        private static readonly Dictionary<string, PropertyAccessor> _propertyAccessors =
            new Dictionary<string, PropertyAccessor>();

        private Contact _activeContact = null;

        private readonly object _propertyAccessorLock = new object();
        private readonly ObservableCollectionDisp<Contact> _subContacts = new ObservableCollectionDisp<Contact>();

        readonly string _id ;

        public MetaContact()
        {
            _id = Guid.NewGuid().ToString();
        }

        public MetaContact(Contact contact) : this()
        {
            _activeContact = contact;

            AddContact(contact);
        }

        public ObservableCollectionDisp<Contact> SubContacts
        {
            get
            {
                return _subContacts;
            }
        }

        #region IContact Members

        public Jid Jid
        {
            get
            {
                return (Jid) GetValueSafe("Jid");
            }
        }

        public Presence Presence
        {
            get
            {
                return (Presence) GetValueSafe("Presence");
            }
        }

        public string Resource
        {
            get
            {
                return (string) GetValueSafe("Resource");
            }
        }

        public string DisplayName
        {
            get
            {
                return (string) GetValueSafe("DisplayName");
            }
        }

        public string Group
        {
            get
            {
                return (string) GetValueSafe("Group");
            }
        }

        public bool IsAvailable
        {
            get
            {
                return (bool) GetValueSafe("IsAvailable");
            }
        }

        public string Show
        {
            get
            {
                return (string) GetValueSafe("Show");
            }
        }

        public int Priority
        {
            get
            {
                return (int) GetValueSafe("Priority");
            }
        }

        public string StatusText
        {
            get
            {
                return (string) GetValueSafe("StatusText");
            }
        }

        public string XStatusText
        {
            get
            {
                return (string) GetValueSafe("XStatusText");
            }
        }

        public string FullName
        {
            get
            {
                return (string) GetValueSafe("FullName");
            }
        }

        public string NickName
        {
            get
            {
                return (string) GetValueSafe("NickName");
            }
        }

        public BitmapImage Image
        {
            get
            {
                return (BitmapImage) GetValueSafe("Image");
            }
        }

        public bool IsImageTransparent
        {
            get
            {
                return (bool) GetValueSafe("IsImageTransparent");
            }
        }

        public string CustomName
        {
            get
            {
                return (string) GetValueSafe("CustomName");
            }
        }

        public bool IsService
        {
            get
            {
                return (bool) GetValueSafe("IsService");
            }
        }

        public string ClientVersion
        {
            get
            {
                return (string)GetValueSafe("ClientVersion");
            }
        }

        public string ClientNode
        {
            get
            {
                return (string)GetValueSafe("ClientNode");
            }
        }

        public string[] ClientExtensions
        {
            get
            {
                return (string[])GetValueSafe("ClientExtensions");
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        #endregion

        public void AddContact(Contact contact)
        {
            contact.PropertyChanged += contact_PropertyChanged;

            lock (SubContacts._syncObject)
            {
                SubContacts.Add(contact);
            }
        }

        public void AddFomMetaContact(MetaContact metaContact)
        {
            foreach (Contact contact in metaContact.SubContacts)
            {
                AddContact(contact);
            }
        }

        public Contact FindContact(Jid jid)
        {
            foreach (Contact contact in SubContacts)
            {
                if (JidUtil.Equals(jid, contact.Jid))
                {
                    return contact;
                }
            }

            return null;
        }

        private void contact_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock (_subContacts._syncObject)
            {
                if (_subContacts.Count > 1)
                {
                }
            }

            if (sender == _activeContact)
            {
                NotifyPropertyChanged(e.PropertyName);
            }
        }

        private object GetValueSafe(string name)
        {
            PropertyAccessor propertyAccessor;

            lock (_propertyAccessorLock)
            {
                _propertyAccessors.TryGetValue(name, out propertyAccessor);

                if (propertyAccessor == null)
                {
                    propertyAccessor =
                        new PropertyAccessor(typeof (Contact), name);

                    _propertyAccessors.Add(name, propertyAccessor);
                }
            }

            return propertyAccessor.Get(_activeContact);
        }
    }
}