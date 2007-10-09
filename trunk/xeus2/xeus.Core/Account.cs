using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using agsXMPP;
using agsXMPP.net;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.commands;
using agsXMPP.protocol.iq.disco;
using agsXMPP.protocol.iq.last;
using agsXMPP.protocol.iq.register;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.iq.search;
using agsXMPP.protocol.x.muc;
using agsXMPP.protocol.x.muc.iq.owner;
using agsXMPP.Xml.Dom;
using Win32_API;
using xeus2.Properties;
using xeus2.xeus.Data;
using xeus2.xeus.Middle;
using Search=xeus2.xeus.Middle.Search;
using Timer=System.Timers.Timer;
using Uri=agsXMPP.Uri;
using Version=agsXMPP.protocol.iq.version.Version;

namespace xeus2.xeus.Core
{
    internal class DiscoverySessionData
    {
        private readonly object _data;
        private readonly string _sessionKey;

        public DiscoverySessionData(object data)
        {
            _data = data;
            _sessionKey = Services.Instance.SessionKey;
        }

        public string SessionKey
        {
            get
            {
                return _sessionKey;
            }
        }

        public object Data
        {
            get
            {
                return _data;
            }
        }
    }

    internal class Account : NotifyInfoDispatcher
    {
        private static readonly Account _instance = new Account();
        private readonly object _discoLock = new object();
        private readonly Timer _discoTime = new Timer(250);
        private readonly ArrayList _pendingCommand = ArrayList.Synchronized(new ArrayList());
        private readonly ArrayList _pendingDiscoInfo = ArrayList.Synchronized(new ArrayList());
        private readonly SelfContact _selfContact = new SelfContact();

        private readonly XmppClientConnection _xmppConnection = new XmppClientConnection();
        private XmppConnectionState _connectionState = XmppConnectionState.Disconnected;

        private DiscoManager _discoManager;

        private bool _isLogged = false;
        private MucMarkManager _mucMarkManager;
        private int _servicesCount;
        private int _servicesDoneCount;
        private volatile bool _working = false;

        public static Account Instance
        {
            get
            {
                return _instance;
            }
        }

        Account()
        {
            Prepare();
        }

        public int MyPriority
        {
            get
            {
                return XmppConnection.Priority;
            }
        }

        public bool IsLogged
        {
            get
            {
                return _isLogged;
            }

            protected set
            {
                if (_isLogged != value)
                {
                    NotifyPropertyChanged("IsLogged");
                }

                _isLogged = value;
            }
        }

        public DiscoManager DiscoMan
        {
            get
            {
                return _discoManager;
            }
        }

        public int ServicesCount
        {
            get
            {
                return _servicesCount;
            }
            set
            {
                _servicesCount = value;

                NotifyPropertyChanged("ServicesCount");
            }
        }

        public int ServicesDoneCount
        {
            get
            {
                return _servicesDoneCount;
            }
            set
            {
                _servicesDoneCount = value;

                NotifyPropertyChanged("ServicesDoneCount");
            }
        }

        public MucMarkManager MucMarkManager
        {
            get
            {
                return _mucMarkManager;
            }
        }

        public XmppClientConnection XmppConnection
        {
            get
            {
                return _xmppConnection;
            }
        }

        public SelfContact Self
        {
            get
            {
                return _selfContact;
            }
        }

        public XmppConnectionState ConnectionState
        {
            get
            {
                return _connectionState;
            }
        }

        protected static void Cleanup()
        {
            Services.Instance.Clear();
            Roster.Instance.Items.Clear();
        }

        public void Create()
        {
            Close();
            Cleanup();
            CreateAccount();                        
        }

        private void CreateAccount()
        {
            OpenInternal(true);
        }

        public void Login()
        {
            Close();
            Cleanup();
            Open();
        }

        public void Prepare()
        {
            _discoManager = new DiscoManager(XmppConnection);

            XmppConnection.UseCompression = true;
            XmppConnection.AutoResolveConnectServer = true;

            XmppConnection.ConnectServer = null;

            XmppConnection.Resource = Settings.Default.XmppResource;
            XmppConnection.Priority = Settings.Default.XmppPriority;

            XmppConnection.SocketConnectionType = SocketConnectionType.Direct;
            XmppConnection.UseStartTLS = true;

            XmppConnection.AutoRoster = true;
            XmppConnection.AutoAgents = true;

            XmppConnection.OnClose += _xmppConnection_OnClose;
            XmppConnection.OnLogin += _xmppConnection_OnLogin;
            XmppConnection.OnRosterItem += _xmppConnection_OnRosterItem;
            XmppConnection.OnRosterEnd += _xmppConnection_OnRosterEnd;
            XmppConnection.OnPresence += _xmppConnection_OnPresence;
            XmppConnection.OnError += _xmppConnection_OnError;
            XmppConnection.OnAuthError += _xmppConnection_OnAuthError;
            XmppConnection.OnXmppConnectionStateChanged += XmppConnection_OnXmppConnectionStateChanged;
            XmppConnection.OnMessage += XmppConnection_OnMessage;
            XmppConnection.OnPasswordChanged += XmppConnection_OnPasswordChanged;
            XmppConnection.OnBinded += XmppConnection_OnBinded;
            XmppConnection.OnRegistered += XmppConnection_OnRegistered;
            XmppConnection.OnRegisterError += XmppConnection_OnRegisterError;
            XmppConnection.OnRegisterInformation += XmppConnection_OnRegisterInformation;
            XmppConnection.OnSocketError += XmppConnection_OnSocketError;
            XmppConnection.OnXmppError += XmppConnection_OnXmppError;

            XmppConnection.OnIq += _xmppConnection_OnIq;

            _mucMarkManager = new MucMarkManager(XmppConnection);

            new MucManager(XmppConnection);

            _discoTime.Elapsed += _discoTime_Elapsed;
            _discoTime.AutoReset = false;
        }

        void OpenInternal(bool newAccount)
        {
            XmppConnection.RegisterAccount = newAccount;
            XmppConnection.Username = Settings.Default.XmppUserName;
            XmppConnection.Password = Settings.Default.XmppPassword;
            XmppConnection.Server = Settings.Default.XmppServer;

            /*XmppConnection.Capabilities = Self.Caps;
            XmppConnection.EnableCapabilities = true;*/

            Settings.Default.Save();

            XmppConnection.Open();

            _selfContact.LoadMyAvatar();            
        }

        public void Open()
        {
            OpenInternal(false);
        }

        void XmppConnection_OnXmppError(object sender, Element e)
        {
            EventError eventError = new EventError(string.Format("XMPP Error: {0}", e), null);
            Events.Instance.OnEvent(this, eventError);
        }

        void XmppConnection_OnSocketError(object sender, Exception ex)
        {
            EventErrorConnection eventError = new EventErrorConnection("Connection error", ex);
            Events.Instance.OnEvent(this, eventError);
        }

        void XmppConnection_OnRegisterInformation(object sender, RegisterEventArgs args)
        {
            // Events.Instance.OnEvent(this, new EventErrorRegistration(args.Register.Instructions));
        }

        void XmppConnection_OnRegisterError(object sender, Element e)
        {
            Events.Instance.OnEvent(this, new EventErrorRegistration(e));
        }

        private void _xmppConnection_OnAuthError(object sender, Element e)
        {
            EventErrorAuth eventError = new EventErrorAuth(Resources.Event_AuthFailed);
            Events.Instance.OnEvent(this, eventError);
        }

        private void _xmppConnection_OnError(object sender, Exception ex)
        {
            EventError eventError = new EventError(ex.Message, null);
            Events.Instance.OnEvent(this, eventError);
        }

        void XmppConnection_OnRegistered(object sender)
        {
            
        }

        void XmppConnection_OnBinded(object sender)
        {
        }

        void XmppConnection_OnPasswordChanged(object sender)
        {
            throw new NotImplementedException();
        }

        private void XmppConnection_OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            Roster.Instance.OnMessage(sender, msg);
        }

        private void XmppConnection_OnXmppConnectionStateChanged(object sender, XmppConnectionState state)
        {
            _connectionState = state;

            NotifyPropertyChanged("ConnectionState");
        }

        private void _discoTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_working)
            {
                _discoTime.Start();
                return;
            }

            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            lock (_discoLock)
            {
                _working = true;

                try
                {
                    for (int i = 0; i < 40; i++)
                    {
                        if (_pendingCommand.Count > 0)
                        {
                            DiscoItem discoItem = (DiscoItem) _pendingCommand[0];
                            _pendingCommand.RemoveAt(0);

                            _discoManager.DisoverItems(discoItem.Jid,
                                                       Uri.COMMANDS,
                                                       OnCommandsServerResult,
                                                       new DiscoverySessionData(discoItem));
                        }
                        else if (_pendingDiscoInfo.Count > 0)
                        {
                            DiscoItem discoItem = (DiscoItem) _pendingDiscoInfo[0];
                            _pendingDiscoInfo.RemoveAt(0);

                            ServicesCount++;

                            if (string.IsNullOrEmpty(discoItem.Node))
                            {
                                _discoManager.DisoverInformation(discoItem.Jid, new IqCB(OnDiscoInfoResult),
                                                                 new DiscoverySessionData(discoItem));
                            }
                            else
                            {
                                _discoManager.DisoverInformation(discoItem.Jid, discoItem.Node,
                                                                 OnDiscoInfoResult,
                                                                 new DiscoverySessionData(discoItem));
                            }

                            // one option
                            Jid jid;

                            jid = discoItem.Jid;

                            if (discoItem.Node != null)
                            {
                                _discoManager.DisoverItems(jid, discoItem.Node, OnDiscoServerResult,
                                                           new DiscoverySessionData(discoItem));
                            }
                            else
                            {
                                _discoManager.DisoverItems(jid, new IqCB(OnDiscoServerResult),
                                                           new DiscoverySessionData(discoItem));
                            }
                        }
                    }
                }

                finally
                {
                    _working = false;
                    _discoTime.Start();
                }
            }
        }

        public void StopDiscovery()
        {
            _pendingDiscoInfo.Clear();

            Services.Instance.StopSession();
        }

        private void _xmppConnection_OnIq(object sender, IQ iq)
        {
            if (iq != null)
            {
                // No Iq with query
                /*
                if (iq.HasTag(typeof (SI)))
                {
                    if (iq.Type == IqType.set)
                    {
                        SI si =
                            iq.SelectSingleElement(typeof (SI)) as SI;

                        File file = si.File;

                        if (file != null)
                        {
                            TransferWindow.Transfer(XmppConnection, iq);
                        }
                    }
                    else if (iq.Type == IqType.result)
                    {
                    }
                }*/
            }

            if (iq != null && iq.Type == IqType.get)
            {
                Element query = iq.Query;

                if (query != null)
                {
                    if (query.GetType() == typeof (Version))
                    {
                        // its a version IQ VersionIQ
                        Version version = (Version) query;

                        // Somebody wants to know our client version, so send it back
                        iq.SwitchDirection();
                        iq.Type = IqType.result;

                        version.Name = Self.ClientName;
                        version.Ver = Self.ClientVersion;
                        version.Os = Self.ClientOS;

                        XmppConnection.Send(iq);
                    }
                    else if (query.GetType() == typeof (DiscoInfo))
                    {
                        iq.SwitchDirection();
                        iq.Type = IqType.result;

                        iq.AddChild(Self.Disco);

                        XmppConnection.Send(iq);
                    }
                    else if (query.GetType() == typeof (Last))
                    {
                        iq.SwitchDirection();
                        iq.Type = IqType.result;

                        Last last = new Last();

                        last.Seconds = (int) (Win32.GetIdleTime() / 1000);
                        iq.AddChild(last);

                        XmppConnection.Send(iq);
                    }
                }
            }
        }

        private void _xmppConnection_OnPresence(object sender, Presence pres)
        {
            Roster.Instance.OnPresence(sender, pres);
        }

        private void _xmppConnection_OnRosterEnd(object sender)
        {
            RecentItems.Instance.LoadItems();
            CapsCache.Instance.Load();

            SendMyPresence();
            _selfContact.AskMyVcard();
        }

        private void DiscoveryInternal(string serverJid)
        {
            StopDiscovery();

            Services.Instance.Clear();

            ServicesDoneCount = 0;
            ServicesCount = 0;

            _discoTime.Start();

            Jid jid;

            if (string.IsNullOrEmpty(serverJid))
            {
                jid = new Jid(XmppConnection.Server);
            }
            else
            {
                jid = new Jid(serverJid);
            }

            Discovery(jid);
        }

        public void Discovery(string serverJid)
        {
            DiscoveryInternal(serverJid);
        }

        public void SendMyPresence()
        {
            _xmppConnection.Send(Self.Presence);

            _selfContact.PresenceChange();
        }

        private void _xmppConnection_OnRosterItem(object sender, RosterItem item)
        {
            Roster.Instance.OnRosterItem(sender, item);
        }

        private void _xmppConnection_OnLogin(object sender)
        {
            IsLogged = true;

            _mucMarkManager.LoadMucMarks();
        }

        private void _xmppConnection_OnClose(object sender)
        {
            IsLogged = false;
        }

        private void Discovery(Jid jid)
        {
            _discoManager.DisoverItems(jid, new IqCB(OnDiscoServerResult), new DiscoverySessionData(null));
        }

        public void AddDiscoInfo(DiscoItem discoItem)
        {
            _pendingDiscoInfo.Add(discoItem);
        }

        public void AddDiscoInfoPrioritized(DiscoItem discoItem)
        {
            _pendingDiscoInfo.Insert(0, discoItem);
        }

        private void AddCommand(DiscoItem discoItem)
        {
            _pendingCommand.Add(discoItem);
        }

        private static bool CheckSessionKey(object data)
        {
            DiscoverySessionData sessionData = data as DiscoverySessionData;

            return (Services.Instance.SessionKey != string.Empty
                    && sessionData.SessionKey == Services.Instance.SessionKey);
        }

        private void OnDiscoServerResult(object sender, IQ iq, object data)
        {
            if (!CheckSessionKey(data))
            {
                return;
            }

            if (iq.Error != null)
            {
                EventInfo eventError = new EventInfo(string.Format(Resources.Error_DiscoFailed, iq.From));
                Events.Instance.OnEvent(this, eventError);
            }
            else if (iq.Type == IqType.result)
            {
                Element query = iq.Query;

                if (query != null && query is DiscoItems)
                {
                    DiscoItems items = query as DiscoItems;
                    DiscoItem[] itms = items.GetDiscoItems();

                    DiscoverySessionData sessionData = data as DiscoverySessionData;
                    Services.Instance.OnServiceItem(itms, sessionData.Data as DiscoItem);
                }
            }
        }

        private void OnDiscoInfoResult(object sender, IQ iq, object data)
        {
            if (!CheckSessionKey(data))
            {
                return;
            }

            if (iq.Error != null)
            {
                ServicesDoneCount++;

                Services.Instance.OnServiceItemError(sender, iq);
            }
            else if (iq.Type == IqType.result && iq.Query is DiscoInfo)
            {
                DiscoInfo di = iq.Query as DiscoInfo;

                DiscoverySessionData sessionData = data as DiscoverySessionData;

                DiscoItem discoItem = sessionData.Data as DiscoItem;

                Services.Instance.OnServiceItemInfo(discoItem, di);

                ServicesDoneCount++;

                if (di.HasFeature(Uri.COMMANDS) && di.Node == null)
                {
                    AddCommand(discoItem);
                }
            }
        }

        private void OnCommandsServerResult(object sender, IQ iq, object data)
        {
            if (!CheckSessionKey(data))
            {
                return;
            }

            if (iq.Error != null)
            {
                EventError eventError =
                    new EventError(string.Format(Resources.Error_CommandResultFailed, iq.From), iq.Error);
                Events.Instance.OnEvent(this, eventError);
            }
            else if (iq.Type == IqType.result)
            {
                DiscoverySessionData discoverySessionData = data as DiscoverySessionData;
                Services.Instance.OnCommandsItemInfo(this, discoverySessionData.Data as DiscoItem, iq);
            }
        }

        public void DoSearchService(Service service, agsXMPP.protocol.x.data.Data data)
        {
            SearchIq searchIq = new SearchIq(IqType.set, service.Jid);
            searchIq.To = service.Jid;
            searchIq.Query.AddChild(data);

            XmppConnection.IqGrabber.SendIq(searchIq, OnServiceSearched, service);
        }

        public void DoSearchService(Service service, string first, string last, string nick, string email)
        {
            SearchIq searchIq = new SearchIq(IqType.set, service.Jid);
            searchIq.Query.Firstname = first;
            searchIq.Query.Lastname = last;
            searchIq.Query.Nickname = nick;
            searchIq.Query.Email = email;

            XmppConnection.IqGrabber.SendIq(searchIq, OnServiceSearched, service);
        }

        private void OnServiceSearched(object sender, IQ iq, object data)
        {
            Service service = data as Service;

            agsXMPP.protocol.iq.search.Search search = iq.Query as agsXMPP.protocol.iq.search.Search;

            if (iq.Error != null)
            {
                EventError eventError = new EventError(string.Format(Resources.Event_SearchFailed,
                                                                     service.Name, iq.Error.Condition), iq.Error);
                Events.Instance.OnEvent(this, eventError);
            }
            else if (iq.Type == IqType.result && search != null)
            {
                Search.Instance.DisplaySearchResult(search, (Service) data);

                EventInfo eventinfo = new EventInfo(string.Format(Resources.Even_SearchSucceeded, service.Name));
                Events.Instance.OnEvent(this, eventinfo);
            }
        }

        public void DoRegisterService(Service service, agsXMPP.protocol.x.data.Data data)
        {
            RegisterIq registerIq = new RegisterIq(IqType.set, service.Jid);
            registerIq.To = service.Jid;
            registerIq.Query.AddChild(data);

            XmppConnection.IqGrabber.SendIq(registerIq, OnServiceRegistered, service);
        }

        public void DoRegisterService(Service service, Dictionary<string, string> values)
        {
            RegisterIq registerIq = new RegisterIq(IqType.set, service.Jid);

            foreach (KeyValuePair<string, string> value in values)
            {
                registerIq.Query.AddTag(value.Key, value.Value);
            }

            XmppConnection.IqGrabber.SendIq(registerIq, OnServiceRegistered, service);
        }

        private void OnServiceRegistered(object sender, IQ iq, object data)
        {
            Service service = data as Service;

            if (iq.Error != null)
            {
                EventError eventError = new EventError(string.Format(Resources.Event_RegistrationFailed,
                                                                     service.Name, iq.Error.Condition), iq.Error);
                Events.Instance.OnEvent(this, eventError);
            }
            else if (iq.Type == IqType.result)
            {
                EventInfo eventinfo = new EventInfo(string.Format(Resources.Event_RegistrationSucceeded, service.Name));
                Events.Instance.OnEvent(this, eventinfo);
            }
        }

        public void GetService(Service service)
        {
            RegisterIq registerIq = new RegisterIq(IqType.get, service.Jid);

            XmppConnection.IqGrabber.SendIq(registerIq, OnRegisterServiceGet, service);
        }

        public void GetServiceSearch(Service service)
        {
            SearchIq searchIq = new SearchIq(IqType.get, service.Jid);

            XmppConnection.IqGrabber.SendIq(searchIq, OnRegisterServiceGetSearch, service);
        }

        private void OnRegisterServiceGetSearch(object sender, IQ iq, object data)
        {
            agsXMPP.protocol.iq.search.Search search = iq.Query as agsXMPP.protocol.iq.search.Search;

            if (iq.Error != null)
            {
                Service service = data as Service;

                EventError eventError = new EventError(string.Format(Resources.Event_SearchInfoFailed,
                                                                     service.Name), iq.Error);

                Events.Instance.OnEvent(this, eventError);
            }
            else if (iq.Type == IqType.result && search != null)
            {
                Search.Instance.DisplaySearch(search, (Service) data);
            }
        }

        private void OnRegisterServiceGet(object sender, IQ iq, object data)
        {
            Register register = iq.Query as Register;

            if (iq.Error != null)
            {
                Service service = data as Service;

                EventError eventError = new EventError(string.Format(Resources.Event_RegInfoFailed,
                                                                     service.Name), iq.Error);

                Events.Instance.OnEvent(this, eventError);
            }
            else if (iq.Type == IqType.result && register != null)
            {
                Registration.Instance.DisplayInBandRegistration(register, (Service) data);
            }
        }

        public void ServiceCommand(Service service)
        {
            IQ commandIq = new IQ(IqType.set);

            commandIq.GenerateId();
            commandIq.To = service.Jid;

            Command command = new Command(service.Node);
            command.Action = Action.execute;

            commandIq.AddChild(command);

            XmppConnection.IqGrabber.SendIq(commandIq, OnCommandResult, service);
        }

        private void OnCommandResult(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                Service service = data as Service;

                EventError eventError = new EventError(string.Format(Resources.Event_CommandExecFailed,
                                                                     service.Name), iq.Error);

                Events.Instance.OnEvent(this, eventError);
            }
            else if (iq.Type == IqType.result)
            {
                foreach (Node node in iq.ChildNodes)
                {
                    Command command = node as Command;

                    if (command != null)
                    {
                        Service service = data as Service;

                        if (service == null)
                        {
                            service = (data as ServiceCommandExecution).Service;
                        }

                        CommandExecutor.Instance.DisplayQuestionaire(command, service);
                        break;
                    }
                }
            }
        }

        public void Close()
        {
            if (IsLogged)
            {
                XmppConnection.Close();
            }
        }

        protected void ExecuteServiceCommand(ServiceCommandExecution command, Action action)
        {
            IQ commandIq = new IQ(IqType.set);

            commandIq.GenerateId();
            commandIq.To = command.Service.Jid;

            Command commandExec = new Command(command.Command.Node);
            commandExec.Action = action;
            commandExec.SessionId = command.Command.SessionId;
            commandExec.Data = command.GetResult();

            commandIq.AddChild(commandExec);

            XmppConnection.IqGrabber.SendIq(commandIq, OnCommandResult, command);
        }

        public void ServiceCommandComplete(ServiceCommandExecution command)
        {
            ExecuteServiceCommand(command, Action.complete);
        }

        public void ServiceCommandNext(ServiceCommandExecution command)
        {
            ExecuteServiceCommand(command, Action.next);
        }

        public void ServiceCommandPrevious(ServiceCommandExecution command)
        {
            ExecuteServiceCommand(command, Action.prev);
        }

        public void ServiceCommandCancel(ServiceCommandExecution command)
        {
            ExecuteServiceCommand(command, Action.cancel);
        }

        public void JoinMuc(Jid jid)
        {
            _discoManager.DisoverInformation(jid, new IqCB(OnRoomeDiscovered));
        }

        private void OnRoomeDiscovered(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                if (iq.Error.Code == ErrorCode.NotFound)
                {
                    // new room
                    Service service = new Service(new DiscoItem(), false);
                    service.DiscoItem.Jid = iq.From;
                    service.DiscoInfo = (DiscoInfo) iq.Query;

                    JoinMuc(service, null);
                }
                else
                {
                    GeneralResultError(sender, iq);
                }
            }

            if (iq.Type == IqType.result && iq.Query is DiscoInfo)
            {
                Service service = new Service(new DiscoItem(), false);
                service.DiscoItem.Jid = iq.From;
                service.DiscoInfo = (DiscoInfo) iq.Query;

                DiscoverReservedRoomNickname(service);
            }
        }

        public void JoinMuc(Service service)
        {
            DiscoverReservedRoomNickname(service);
        }

        public void GeneralResultError(object sender, IQ iq)
        {
            EventError eventError =
                new EventError(string.Format("Error from '{0}'", sender), iq.Error);

            Events.Instance.OnEvent(this, eventError);
        }

        protected void DiscoverReservedRoomNickname(Service service)
        {
            IQ iq = new IQ(IqType.get, _selfContact.Jid, service.Jid);

            iq.GenerateId();
            DiscoInfo di = new DiscoInfo();
            di.Node = "x-roomuser-item";
            iq.Query = di;

            XmppConnection.IqGrabber.SendIq(iq, new IqCB(OnRoomNicknameResult), service);
        }

        private void OnRoomNicknameResult(object sender, IQ iq, object data)
        {
            string nick = null;

            if (iq.Error != null)
            {
                EventError eventError = new EventError(string.Format(Resources.Error_RoomNickFailed,
                                                                     iq.From), iq.Error);

                Events.Instance.OnEvent(this, eventError);
            }
            else if (iq.Type == IqType.result && iq.Query is DiscoInfo)
            {
                DiscoInfo di = iq.Query as DiscoInfo;

                if (di != null && di.Node == "x-roomuser-item"
                    && di.GetIdentities() != null
                    && di.GetIdentities().Length > 0)
                {
                    nick = di.GetIdentities()[0].Name;
                }
            }

            if (data is Service)
            {
                JoinMuc(data as Service, nick);
            }
            else
            {
                JoinMuc(data as MucMark, nick);
            }
        }

        protected void JoinMuc(Service service, string nick)
        {
            MucInfo.Instance.MucLogin(service, nick);
        }

        protected void JoinMuc(MucMark mucMark, string nick)
        {
            if (!string.IsNullOrEmpty(nick))
            {
                mucMark.Nick = nick;
            }
            _discoManager.DisoverInformation(mucMark.Jid, new IqCB(OnDiscoInfoResultMucRoom), mucMark);
        }

        private void OnDiscoInfoResultMucRoom(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                Services.Instance.OnServiceItemError(sender, iq);
            }
            else if (iq.Type == IqType.result && iq.Query is DiscoInfo)
            {
                DiscoInfo di = iq.Query as DiscoInfo;

                MucMark mucMark = (MucMark) data;
                mucMark.DiscoInfo = di;

                MucInfo.Instance.MucLogin(mucMark);
            }
        }

        public MucRoom JoinMuc(Service service, string nick, string password)
        {
            return new MucRoom(service, XmppConnection, nick, password);
        }

        public MucManager GetMucManager()
        {
            return new MucManager(XmppConnection);
        }

        public PresenceManager GetPresenceManager()
        {
            return new PresenceManager(XmppConnection);
        }

        public void DoSaveMucConfig(MucRoom mucRoom, agsXMPP.protocol.x.data.Data data)
        {
            OwnerIq saveIq = new OwnerIq(IqType.set, mucRoom.Service.Jid);

            saveIq.Query.AddChild(data);

            XmppConnection.IqGrabber.SendIq(saveIq, OnMucConfigSaved, mucRoom);
        }

        private void OnMucConfigSaved(object sender, IQ iq, object data)
        {
            MucRoom mucRoom = data as MucRoom;

            if (iq.Error != null)
            {
                EventError eventError = new EventError(string.Format(Resources.EventError_MucConfigFailed,
                                                                     mucRoom.Service.Name, iq.Error.Condition), iq.Error);
                Events.Instance.OnEvent(this, eventError);
            }
        }
    }
}