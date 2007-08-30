using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using FastDynamicPropertyAccessor;

namespace xeus2.xeus.Core
{
    internal class MetaContact : NotifyInfoDispatcher, IContact
    {
        private ObservableCollectionDisp<Contact> _subContacts = new ObservableCollectionDisp<Contact>();

        static Dictionary<string, PropertyAccessor> _propertyAccessors = new Dictionary<string, PropertyAccessor>();
        private object _propertyAccessorLock = new object();

        public MetaContact(Contact contact)
        {
            AddContact(contact);
        }

        public void AddContact(Contact contact)
        {
            contact.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(contact_PropertyChanged);

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
                if (Utilities.JidUtil.BareEquals(jid, contact.Jid))
                {
                    return contact;
                }
            }

            return null;
        }

        void contact_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        object GetValueSafe(string name)
        {
            PropertyAccessor propertyAccessor;

            lock (_propertyAccessorLock)
            {
                _propertyAccessors.TryGetValue(name, out propertyAccessor);

                if (propertyAccessor == null)
                {
                    propertyAccessor =
                        new PropertyAccessor(typeof(Contact), name);
                }
            }

            return propertyAccessor.Get(_subContacts[0]);
        }

        public Jid Jid
        {
            get
            {
                return GetValueSafe("Jid") as Jid;
            }
        }

        public Presence Presence
        {
            get
            {
                return GetValueSafe("Presence") as Presence;
            }
        }

        public string DisplayName
        {
            get
            {
                return GetValueSafe("DisplayName") as string;
            }
        }

        public string Group
        {
            get
            {
                return GetValueSafe("Group") as string;
            }
        }

        public string StatusText
        {
            get
            {
                return GetValueSafe("StatusText") as string;
            }
        }

        public string XStatusText
        {
            get
            {
                return GetValueSafe("XStatusText") as string;
            }
        }

        public string FullName
        {
            get
            {
                return GetValueSafe("FullName") as string;
            }
        }

        public string NickName
        {
            get
            {
                return GetValueSafe("NickName") as string;
            }
        }

        public string CustomName
        {
            get
            {
                return GetValueSafe("CustomName") as string;
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
