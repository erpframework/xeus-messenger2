using System;
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
        private ObservableCollectionDisp<Contact> _subContacts = new ObservableCollectionDisp<Contact>();
        private Contact _activeContact = null;

        private static Dictionary<string, PropertyAccessor> _propertyAccessors =
            new Dictionary<string, PropertyAccessor>();

        private object _propertyAccessorLock = new object();

        public MetaContact(Contact contact)
        {
            _activeContact = contact;

            AddContact(contact);
        }

        public void AddContact(Contact contact)
        {
            contact.PropertyChanged += new PropertyChangedEventHandler(contact_PropertyChanged);

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
                if (JidUtil.BareEquals(jid, contact.Jid))
                {
                    return contact;
                }
            }

            return null;
        }

        private void contact_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            /// needs to switch active contact when presence changes

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
                }
            }

            return propertyAccessor.Get(_activeContact);
        }

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
                return (string)GetValueSafe("Resource");
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
                return (BitmapImage)GetValueSafe("Image");
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

        public ObservableCollectionDisp<Contact> SubContacts
        {
            get
            {
                return _subContacts;
            }
        }
    }
}