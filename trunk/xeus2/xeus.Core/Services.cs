using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.disco;
using agsXMPP.Xml.Dom;
using xeus2.Properties;

namespace xeus2.xeus.Core
{
    internal class Services : ObservableCollectionDisp<Service>
    {
        private static readonly Services _instance = new Services();

        private readonly Dictionary<string, Service> _allServices = new Dictionary<string, Service>();

        private delegate void OnServiceItemErrorCallback(IQ iq);

        private delegate void OnCommandsItemInfoCallback(DiscoItem discoItem, IQ iq);

        private string _sessionKey = string.Empty;

        private readonly ObservableCollectionDisp<RegisteredService> _registeredTransports = new ObservableCollectionDisp<RegisteredService>();

        public static Services Instance
        {
            get
            {
                return _instance;
            }
        }

        public static ServiceCategories Categories
        {
            get
            {
                return _categories;
            }
        }

        private readonly ObservableCollectionDisp<Service> _allServicesCollection = new ObservableCollectionDisp<Service>();

        private readonly MucRooms _mucRooms = new MucRooms();
        private readonly Transports _transports = new Transports();

        public ObservableCollectionDisp<Service> AllServices
        {
            get
            {
                return _allServicesCollection;
            }
        }

        public string SessionKey
        {
            get
            {
                return _sessionKey;
            }
        }

        public MucRooms MucRooms
        {
            get
            {
                return _mucRooms;
            }
        }

        public Transports Transports
        {
            get
            {
                return _transports;
            }
        }

        public ObservableCollectionDisp<RegisteredService> RegisteredTransports
        {
            get
            {
                return _registeredTransports;
            }
        }

        public new void Clear()
        {
            lock (_syncObject)
            {
                _sessionKey = Guid.NewGuid().ToString();

                _mucRooms.Clear();
                _transports.Clear();
                _allServices.Clear();
                _allServicesCollection.Clear();

                Categories.Clear();

                base.Clear();
            }
        }


        private static readonly ServiceCategories _categories = new ServiceCategories();

        public void OnCommandsItemInfo(object sender, DiscoItem discoItem, IQ iq)
        {
            if (_sessionKey == string.Empty)
            {
                return;
            }

            App.InvokeSafe(App._dispatcherPriority,
                           new OnCommandsItemInfoCallback(OnCommandsItemInfo), discoItem, iq);
        }

        /*
        public void OnServiceItemInfo(object sender, DiscoItem discoItem, DiscoInfo info)
        {
            if (_sessionKey == string.Empty)
            {
                return;
            }

            App.InvokeSafe(App._dispatcherPriority,
                           new ServiceItemInfoCallback(OnServiceItemInfo), discoItem, info);
        }*/

        public void OnServiceItemError(object sender, IQ iq)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new OnServiceItemErrorCallback(OnServiceItemError), iq);
        }

        void OnServiceItemError(IQ iq)
        {
            EventInfo eventInfo =
                new EventInfo(string.Format(Resources.Error_ServiceDiscoFailed, iq.From, iq.Error.Code));
            Events.Instance.OnEvent(this, eventInfo);
        }

        public void OnCommandsItemInfo(DiscoItem discoItem, IQ iq)
        {
            lock (_syncObject)
            {
                Service service = FindService(discoItem);

                if (service != null)
                {
                    foreach (Node node in iq.Query.ChildNodes)
                    {
                        DiscoItem item = node as DiscoItem;

                        if (item != null)
                        {
                            Service command = FindService(item);

                            if (command == null)
                            {
                                // it is not in hierarchy
                                command = new Service(item, false);

                                _allServices.Add(command.Key, command);
                                _allServicesCollection.Add(command);

                                Account.Instance.AddDiscoInfoPrioritized(item);
                            }

                            lock (service.Commands._syncObject)
                            {
                                service.Commands.Add(command);
                            }
                        }
                    }
                }
            }
        }

        public void OnServiceItemInfo(DiscoItem discoItem, DiscoInfo info)
        {
            lock (_syncObject)
            {
                Service service = FindService(discoItem);

                if (service != null)
                {
                    service.DiscoInfo = info;

                    if (service.IsToplevel)
                    {
                        _categories.AddService(service);
                    }

                    if (service.IsChatRoom)
                    {
                        _mucRooms.AddMuc(service);
                    }
                    else if (service.IsTransport)
                    {
                        _transports.Add(service);
                        
                        DetermineRegistered(service);
                    }
                    
                    if (service.IsBytestremProxy)
                    {
                        Settings.Default.XmppBytestreamProxy = service.Jid.ToString();
                    }
                }
            }
        }

        public void OnServiceItem(IList<DiscoItem> discoItems, DiscoItem parent)
        {
            lock (_syncObject)
            {
                List<Service> services = new List<Service>(discoItems.Count);
                Service parentService = null;

                foreach (DiscoItem discoItem in discoItems)
                {
                    Service service = FindService(discoItem);

                    if (parent != null)
                    {
                        parentService = FindService(parent);
                    }

                    if (service == null)
                    {
                        Service newService = new Service(discoItem, (parent == null));
                        _allServices.Add(newService.Key, newService);
                        _allServicesCollection.Add(newService);
                        services.Add(newService);
                    }
                }

                if (parent == null)
                {
                    lock (_syncObject)
                    {
                        foreach (Service service in services)
                        {
                            Add(service);
                        }
                    }
                }
                else
                {
                    if (parentService != null)
                    {
                        lock (parentService.Services._syncObject)
                        {
                            foreach (Service service in services)
                            {
                                parentService.Services.Add(service);
                            }
                        }
                    }
                }
            }
        }

        // unsafe, lock when calling
        public Service FindService(DiscoItem discoItem)
        {
            Service service;

            _allServices.TryGetValue(Service.GetKey(discoItem), out service);

            return service;
        }

        // unsafe, lock when calling
        public Service FindService(Jid jid)
        {
            Service service;

            _allServices.TryGetValue(Service.GetKey(jid), out service);

            return service;
        }

        public void StopSession()
        {
            _sessionKey = string.Empty;
        }

        void DetermineRegistered(Service service)
        {
            service.IsRegistered = (Roster.Instance.FindContact(service.Jid) != null);
        }

        public void FindRegisteredServices()
        {
            _registeredTransports.Clear();

            foreach (MetaContact item in Roster.Instance.Items)
            {
                foreach (Contact contact in item.SubContacts)
                {
                    if (string.IsNullOrEmpty(contact.Jid.User))
                    {
                        DiscoverySingleInfo(contact.Jid);
                    }
                }
            }
        }

        private void DiscoverySingleInfo(Jid jid)
        {
           Account.Instance.DiscoMan.DisoverInformation(jid, OnDiscoServerSingleResult, null);
        }

        private void OnDiscoServerSingleResult(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                OnServiceItemError(sender, iq);
            }
            else if (iq.Type == IqType.result && iq.Query is DiscoInfo)
            {
                DiscoInfo discoInfo = iq.Query as DiscoInfo;

                if (discoInfo != null)
                {
                    DiscoItem discoItem = new DiscoItem();
                    discoItem.Jid = iq.From;
                    discoItem.Node = discoInfo.Node;

                    RegisteredService service = new RegisteredService(discoItem, false);
                    service.DiscoInfo = discoInfo;

                    if (service.IsTransport)
                    {
                        _registeredTransports.Add(service);
                    }
                }
            }
        }

    }
}