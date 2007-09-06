using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using agsXMPP;
using agsXMPP.Collections;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using xeus2.Properties;
using xeus2.xeus.Commands;
using xeus2.xeus.UI;
using xeus2.xeus.Utilities;
using Uri=System.Uri;

namespace xeus2.xeus.Core
{
    internal class MucRoom : NotifyInfoDispatcher, IDisposable
    {
        private delegate void TimerCallback();

        private MucRoster _mucRoster = new MucRoster();
        private MucMessages _mucMessages = new MucMessages();

        private Timer _timeTimer;

        private Service _service;
        private XmppClientConnection _xmppClientConnection = null;
        private string _nick;

        public delegate void MucContactHandler(MucMessage mucMessage);

        public event MucContactHandler OnClickMucContact;

        private FlowDocument _chatDocument = null;

        private List<Rectangle> _relativeTimes = new List<Rectangle>();

        private MucManager _mucManager = null;

        private EventMucRoom _lastEvent = null;

        private MucMessage _lastMessage = null;

        private MucContact _me = null;

        public MucContact Me
        {
            get
            {
                return _me;
            }
        }

        public MucRoom(Service service, XmppClientConnection xmppClientConnection, string nick)
        {
            _service = service;
            _xmppClientConnection = xmppClientConnection;

            _mucManager = new MucManager(_xmppClientConnection);
            _nick = nick;

            _xmppClientConnection.MesagageGrabber.Add(service.Jid, new BareJidComparer(), new MessageCB(MessageCallback),
                                                      null);
            _xmppClientConnection.PresenceGrabber.Add(service.Jid, new BareJidComparer(),
                                                      new PresenceCB(PresenceCallback),
                                                      null);

            _mucMessages.CollectionChanged += new NotifyCollectionChangedEventHandler(_mucMessages_CollectionChanged);

            Events.Instance.OnEventRaised += new Events.EventItemCallback(Instance_OnEventRaised);
        }

        private DateTime _roomStart = DateTime.Now;

        private void Instance_OnEventRaised(object sender, Event myEvent)
        {
            EventMucRoom eventMucRoom = myEvent as EventMucRoom;

            if (eventMucRoom != null && eventMucRoom.MucRoom == this)
            {
                _lastEvent = eventMucRoom;

                NotifyPropertyChanged("LastEvent");

                TimeSpan timeSpan = DateTime.Now - _roomStart;

                if (eventMucRoom.TypicalEventCode != TypicalEvent.Joined || timeSpan >= new TimeSpan(0, 0, 5))
                {
                    MucMessage mucMessage = new MucMessage(new Message(Account.Instance.Self.Jid, Service.Jid,
                                                                       string.Format("{{{0}}} {1}",
                                                                                     eventMucRoom.TypicalEventCode,
                                                                                     eventMucRoom.Message)), null);

                    _mucMessages.Add(mucMessage);
                }
            }
        }

        private void _mucMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    {
                        GenerateChatDocument(e.NewItems);

                        foreach (MucMessage mucMessage in e.NewItems)
                        {
                            if (!string.IsNullOrEmpty(mucMessage.Subject))
                            {
                                Subject = mucMessage.Subject;
                            }
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        _chatDocument = null;

                        NotifyPropertyChanged("ChatDocument");

                        break;
                    }
            }
        }

        public string Subject
        {
            set
            {
                _subject = value;
                NotifyPropertyChanged("Subject");
            }

            get
            {
                return _subject;
            }
        }

        protected void GenerateChatDocument(IList messages)
        {
            if (_chatDocument == null)
            {
                _chatDocument = new FlowDocument();
                _chatDocument.FontFamily = new FontFamily("Segoe UI");
                _chatDocument.FontSize = 11.0;
                _chatDocument.TextAlignment = TextAlignment.Left;
            }

            foreach (MucMessage message in messages)
            {
                int index = MucMessages.IndexOf(message);

                MucMessage previousMessage = null;

                if (index >= 1)
                {
                    previousMessage = MucMessages[index - 1];
                }

                _chatDocument.Blocks.Add(GenerateMessage(message, previousMessage));
            }

            if (_timeTimer == null)
            {
                _timeTimer = new Timer(5000.0);
                _timeTimer.AutoReset = true;
                _timeTimer.Elapsed += new ElapsedEventHandler(_timeTimer_Elapsed);
                _timeTimer.Start();
            }

            NotifyPropertyChanged("ChatDocument");
        }

        private static Brush _forMeForegorund;
        private static Brush _textBrush;
        private static Brush _sysTextBrush;
        private static Brush _meTextBrush;
        private static Brush _textDimBrush;
        private static Brush _contactForeground;
        private static Brush _alternativeBackground;
        private static Brush _bulbBackground;
        private static Brush _ownAvatarBackground;
        private static Brush _timeRecentBackground;
        private static Brush _timeOlderBackground;
        private static Brush _timeOldBackground;
        private static Brush _timeOldestBackground;

        private static Brush _eventBan;
        private static Brush _eventKick;
        private static Brush _eventLeft;
        private static Brush _eventJoined;
        private static Brush _eventChangedNick;

        private static Brush _selectionFindBrush;

        private readonly Regex _urlregex =
            new Regex(
                @"[""'=]?(http://|ftp://|https://|www\.|ftp\.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string _subject;
        private bool _displayTime;

        private static Rectangle CreateRectangle(Brush brush)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = brush;
            rect.Width = 20;
            rect.Height = 20;

            return rect;
        }

        private static Brush GetMessageTimeBrush(MucMessage mucMessage)
        {
            Brush timeBrush;

            switch (mucMessage.MessageOldness)
            {
                case MucMessageOldness.Recent:
                    {
                        timeBrush = _timeRecentBackground;
                        break;
                    }
                case MucMessageOldness.Older:
                    {
                        timeBrush = _timeOlderBackground;
                        break;
                    }
                case MucMessageOldness.Old:
                    {
                        timeBrush = _timeOldBackground;
                        break;
                    }
                default:
                    {
                        timeBrush = _timeOldestBackground;
                        break;
                    }
            }

            return timeBrush;
        }

        private Rectangle CreateTimeRect(MucMessage message)
        {
            Rectangle timeRectangle = new Rectangle();
            timeRectangle.Fill = GetMessageTimeBrush(message);
            timeRectangle.Width = 16;
            timeRectangle.Height = 16;

            timeRectangle.Margin = new Thickness(-10.0, 2.0, 4.0, 0.0);
            timeRectangle.Cursor = Cursors.Arrow;
            _relativeTimes.Add(timeRectangle);

            return timeRectangle;
        }

        public Block GenerateMessage(MucMessage message, MucMessage previousMessage)
        {
            if (message.Sender != null)
            {
                _lastMessage = message;
                NotifyPropertyChanged("LastMessage");
            }

            if (_forMeForegorund == null)
            {
                _forMeForegorund = StyleManager.GetBrush("forme_text_design");

                _textBrush = StyleManager.GetBrush("text_design");
                _sysTextBrush = StyleManager.GetBrush("sys_text_design");
                _meTextBrush = StyleManager.GetBrush("me_text_design");
                _textDimBrush = StyleManager.GetBrush("textdim_design");

                _alternativeBackground = StyleManager.GetBrush("back_alt");

                _contactForeground = StyleManager.GetBrush("muc_contact_fore");
                _bulbBackground = StyleManager.GetBrush("jabber_design");
                _ownAvatarBackground = StyleManager.GetBrush("aff_none_design");

                _timeRecentBackground = StyleManager.GetBrush("time_now_design");
                _timeOlderBackground = StyleManager.GetBrush("time_older_design");
                _timeOldBackground = StyleManager.GetBrush("time_old_design");
                _timeOldestBackground = StyleManager.GetBrush("time_oldest_design");

                _selectionFindBrush = StyleManager.GetBrush("selection_design");

                _eventBan = StyleManager.GetBrush("aff_outcast_design");
                _eventKick = StyleManager.GetBrush("event_kicked_muc_design");
                _eventLeft = StyleManager.GetBrush("event_left_muc_design");
                _eventJoined = StyleManager.GetBrush("event_joined_muc_design");
                _eventChangedNick = StyleManager.GetBrush("event_nickchange_muc_design");
            }

            Section groupSection = null;

            if (_chatDocument != null)
            {
                groupSection = _chatDocument.Blocks.LastBlock as Section;
            }

            Paragraph paragraph = new Paragraph();

            paragraph.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
            paragraph.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);

            bool newSection = (groupSection == null);

            paragraph.Inlines.Add(CreateTimeRect(message));

            if (previousMessage == null
                || previousMessage.Sender != message.Sender
                ||
                (message.DateTime - previousMessage.DateTime >
                 TimeSpan.FromMinutes(Settings.Default.UI_GroupMessagesByMinutes)))
            {
                if (!string.IsNullOrEmpty(message.Sender))
                {
                    if (message.Sender == Nick)
                    {
                        paragraph.Inlines.Add(CreateRectangle(_ownAvatarBackground));
                    }

                    Bold contactName = new Bold();
                    contactName.Cursor = Cursors.Hand;
                    contactName.Foreground = _contactForeground;
                    contactName.Inlines.Add(message.Sender);

                    paragraph.Inlines.Add(contactName);
                    paragraph.Inlines.Add("  ");

                    contactName.MouseDown += new MouseButtonEventHandler(contactName_MouseDown);
                    contactName.MouseEnter += new MouseEventHandler(contactName_MouseEnter);
                    contactName.MouseLeave += new MouseEventHandler(contactName_MouseLeave);
                }
                else
                {
                    paragraph.Inlines.Add(CreateRectangle(_bulbBackground));
                }

                newSection = true;
            }

            string body = message.Body;

            if (string.IsNullOrEmpty(message.Sender))
            {
                // system message
                paragraph.Foreground = _sysTextBrush;
            }

            if (!string.IsNullOrEmpty(body))
            {
                if (body.TrimStart().StartsWith("/me "))
                {
                    // /me
                    body = body.Replace("/me ", String.Empty);
                    paragraph.Foreground = _meTextBrush;
                }

                if (body.StartsWith("{" + TypicalEvent.RoomCreated + "}"))
                {
                    paragraph.Inlines.Add(
                        "If you want to make this room permanent, configure it here ");

                    Button buttonOpenConfig = new Button();
                    buttonOpenConfig.Click += new RoutedEventHandler(buttonOpenConfig_Click);
                    buttonOpenConfig.Content = "Open configuration";
                    paragraph.Inlines.Add(buttonOpenConfig);

                    body = PrepareBody(body, TypicalEvent.RoomCreated);
                }
                else if (body.StartsWith("{" + TypicalEvent.Banned + "}"))
                {
                    paragraph.Inlines.Add(CreateRectangle(_eventBan));

                    body = PrepareBody(body, TypicalEvent.Banned);
                }
                else if (body.StartsWith("{" + TypicalEvent.Kicked + "}"))
                {
                    paragraph.Inlines.Add(CreateRectangle(_eventKick));

                    body = PrepareBody(body, TypicalEvent.Kicked);
                }
                else if (body.StartsWith("{" + TypicalEvent.Joined + "}"))
                {
                    paragraph.Inlines.Add(CreateRectangle(_eventJoined));

                    body = PrepareBody(body, TypicalEvent.Joined);
                }
                else if (body.StartsWith("{" + TypicalEvent.Left + "}"))
                {
                    paragraph.Inlines.Add(CreateRectangle(_eventLeft));

                    body = PrepareBody(body, TypicalEvent.Left);
                }
                else if (body.StartsWith("{" + TypicalEvent.NickChange + "}"))
                {
                    paragraph.Inlines.Add(CreateRectangle(_eventChangedNick));

                    body = PrepareBody(body, TypicalEvent.NickChange);
                }

                MatchCollection matches = _urlregex.Matches(body);

                if (matches.Count > 0)
                {
                    string[] founds = new string[matches.Count];

                    for (int i = 0; i < founds.Length; i++)
                    {
                        founds[i] = matches[i].ToString();
                    }

                    string[] bodies = body.Split(founds, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 0; j < bodies.Length || j < founds.Length; j++)
                    {
                        bool wrongUri = false;

                        if (bodies.Length > j)
                        {
                            paragraph.Inlines.Add(bodies[j]);
                        }

                        if (founds.Length > j)
                        {
                            Run hyperlinkRun = new Run(founds[j]);
                            Hyperlink hyperlink = new XeusHyperlink(hyperlinkRun);
                            hyperlink.Foreground = Brushes.DarkSalmon;

                            try
                            {
                                string url = hyperlinkRun.Text;

                                if (!url.Contains(":"))
                                {
                                    url = string.Format("http://{0}", url);
                                }

                                hyperlink.NavigateUri = new Uri(url);
                            }

                            catch
                            {
                                // improper uri format
                                wrongUri = true;
                            }

                            if (wrongUri)
                            {
                                paragraph.Inlines.Add(hyperlinkRun);
                            }
                            else
                            {
                                paragraph.Inlines.Add(hyperlink);
                            }
                        }
                    }
                }
                else
                {
                    paragraph.Inlines.Add(body);
                }
            }
            else if (!string.IsNullOrEmpty(message.Subject))
            {
                paragraph.Inlines.Add("Changed topic: " + message.Subject);
            }

            paragraph.DataContext = message;

            if (!string.IsNullOrEmpty(body)
                && TextUtil.ContainsNick(Nick, body))
            {
                paragraph.Foreground = _forMeForegorund;
            }

            if (newSection)
            {
                groupSection = new Section();


                groupSection.Blocks.Add(paragraph);
                groupSection.Margin = new Thickness(3.0, 10.0, 3.0, 0.0);
                groupSection.BorderThickness = new Thickness(0.0, 2.0, 0.0, 0.0);
                groupSection.BorderBrush = _alternativeBackground;
            }

            groupSection.DataContext = message.Sender;
            groupSection.Blocks.Add(paragraph);

            return groupSection;
        }

        private string PrepareBody(string originalBody, TypicalEvent typicalEvent)
        {
            return originalBody.Replace("{" + typicalEvent + "}", String.Empty);
        }

        private void buttonOpenConfig_Click(object sender, RoutedEventArgs e)
        {
            MucCommands.Options.Execute(this, null);
        }

        private void _timeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new TimerCallback(OnRelativeTimer));
        }

        private void OnRelativeTimer()
        {
            foreach (Rectangle time in _relativeTimes)
            {
                string text;

                MucMessage message = (MucMessage) time.DataContext;
                DateTime dateTime = message.DateTime;
                text = string.Format("{0}\n{1}", dateTime, TimeUtilities.FormatRelativeTime(dateTime));

                Brush brush = GetMessageTimeBrush(message);

                if (time.Fill != brush)
                {
                    time.Fill = brush;
                }

                time.ToolTip = text;
            }

            // refresh relative time
            if (_lastEvent != null)
            {
                _lastEvent.RefreshRelativeTime();
            }
            if (_lastMessage != null)
            {
                _lastMessage.RefreshRelativeTime();
            }
        }

        public bool DisplayTime
        {
            get
            {
                return _displayTime;
            }

            set
            {
                _displayTime = value;

                OnRelativeTimer();
            }
        }

        private void contactName_MouseLeave(object sender, MouseEventArgs e)
        {
            Span contactSpan = sender as Span;

            if (contactSpan != null)
            {
                MucMessage mucMessage = contactSpan.DataContext as MucMessage;

                if (mucMessage != null)
                {
                    Highlight(mucMessage.Sender, _textBrush, _contactForeground, _forMeForegorund);
                }
            }
        }

        private void contactName_MouseEnter(object sender, MouseEventArgs e)
        {
            Span contactSpan = sender as Span;

            if (contactSpan != null)
            {
                MucMessage mucMessage = contactSpan.DataContext as MucMessage;

                if (mucMessage != null)
                {
                    Highlight(mucMessage.Sender, _textDimBrush, _textDimBrush, _textDimBrush);
                }
            }
        }

        private void Highlight(string sender, Brush brush, Brush nameBrush, Brush formeBrush)
        {
            foreach (Block block in _chatDocument.Blocks)
            {
                Section section = block as Section;

                if (section != null)
                {
                    string sectionSender = section.DataContext as string;

                    if (sectionSender == null || sectionSender != sender)
                    {
                        section.Foreground = brush;

                        Bold name = ((Paragraph) (section.Blocks.FirstBlock)).Inlines.FirstInline as Bold;
                        if (name != null)
                        {
                            name.Foreground = nameBrush;
                        }
                        else
                        {
                            Inline inline = ((Paragraph) (section.Blocks.FirstBlock)).Inlines.FirstInline;

                            if (inline != null)
                            {
                                name = inline.NextInline as Bold;
                                if (name != null)
                                {
                                    name.Foreground = nameBrush;
                                }
                            }
                        }

                        MucMessage mucMessage = section.Blocks.FirstBlock.DataContext as MucMessage;

                        if (mucMessage != null)
                        {
                            if (!string.IsNullOrEmpty(mucMessage.Body)
                                && mucMessage.Body.IndexOf(Nick, 0, StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                section.Blocks.FirstBlock.Foreground = formeBrush;
                            }
                        }
                    }
                }
            }
        }

        private void contactName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Span contactSpan = sender as Span;

            if (contactSpan != null)
            {
                MucMessage mucMessage = contactSpan.DataContext as MucMessage;

                if (mucMessage != null && OnClickMucContact != null)
                {
                    OnClickMucContact(mucMessage);

                    e.Handled = true;
                }
            }
        }

        public MucRoster MucRoster
        {
            get
            {
                return _mucRoster;
            }
        }

        public MucMessages MucMessages
        {
            get
            {
                return _mucMessages;
            }
        }

        public Service Service
        {
            get
            {
                return _service;
            }
        }

        public FlowDocument ChatDocument
        {
            get
            {
                return _chatDocument;
            }
        }

        public string Nick
        {
            get
            {
                return _nick;
            }

            private set
            {
                _nick = value;

                NotifyPropertyChanged("Nick");
                NotifyPropertyChanged("Me");
            }
        }

        public EventMucRoom LastEvent
        {
            get
            {
                return _lastEvent;
            }
        }

        public MucMessage LastMessage
        {
            get
            {
                return _lastMessage;
            }
        }

        //public MucContact

        private void MessageCallback(object sender, Message msg, object data)
        {
            if (App.CheckAccessSafe())
            {
                if (msg.Error != null)
                {
                    EventError eventError = new EventError(string.Format("Message error in MUC from {0}", msg.From),
                                                           msg.Error);
                    Events.Instance.OnEvent(this, eventError);
                }
                else
                {
                    MucContact contact;

                    lock (MucRoster._syncObject)
                    {
                        contact = MucRoster.Find(msg.From);
                    }

                    MucMessages.OnMessage(msg, contact);
                }
            }
            else
            {
                App.Current.Dispatcher.BeginInvoke(App._dispatcherPriority,
                                                   new MessageCB(MessageCallback), sender, msg, data);
            }
        }

        private void PresenceCallback(object sender, Presence presence, object data)
        {
            if (App.CheckAccessSafe())
            {
                if (presence.Error != null)
                {
                    EventError eventError =
                        new EventError(string.Format("Presence error in MUC from {0}", presence.From),
                                       presence.Error);
                    Events.Instance.OnEvent(this, eventError);
                }
                else
                {
                    if (presence.MucUser != null
                        && presence.MucUser.Status != null)
                    {
                        switch (presence.MucUser.Status.Code)
                        {
                            case StatusCode.RoomCreated:
                                {
                                    EventMucRoom eventMucRoom = new EventMucRoom(TypicalEvent.RoomCreated,
                                                                                 this, presence.MucUser,
                                                                                 "This MUC room has been just created");

                                    Events.Instance.OnEvent(this, eventMucRoom, DispatcherPriority.ApplicationIdle);

                                    break;
                                }
                        }
                    }

                    if (presence.Type == PresenceType.unavailable
                        && presence.MucUser != null
                        && presence.MucUser.Status != null)
                    {
                        if (presence.MucUser.Status.Code == StatusCode.NewNickname)
                        {
                            EventMucRoom eventMucRoom = new EventMucRoom(TypicalEvent.NickChange,
                                                                         this, presence.MucUser,
                                                                         string.Format(
                                                                             "'{0}' is now known as '{1}'",
                                                                             presence.From.Resource,
                                                                             presence.MucUser.Item.Nickname));

                            Events.Instance.OnEvent(this, eventMucRoom, DispatcherPriority.ApplicationIdle);

                            // changed my nick
                            if (presence.From.Resource == Nick)
                            {
                                Nick = presence.MucUser.Item.Nickname;
                            }
                        }

                        if (presence.MucUser.Status.Code == StatusCode.Kicked)
                        {
                            string message;

                            if (presence.MucUser.Item != null
                                && !string.IsNullOrEmpty(presence.MucUser.Item.Reason))
                            {
                                message = string.Format("{0} has been kicked with reason '{1}'",
                                                        presence.From.Resource, presence.MucUser.Item.Reason);
                            }
                            else
                            {
                                message = string.Format("{0} has been kicked", presence.From.Resource);
                            }

                            EventMucRoom eventMucRoom =
                                new EventMucRoom(TypicalEvent.Kicked, this, presence.MucUser, message);

                            Events.Instance.OnEvent(this, eventMucRoom, DispatcherPriority.ApplicationIdle);
                        }

                        if (presence.MucUser.Status.Code == StatusCode.Banned)
                        {
                            string message;

                            if (presence.MucUser.Item != null
                                && !string.IsNullOrEmpty(presence.MucUser.Item.Reason))
                            {
                                message = string.Format("{0} has been banned with reason '{1}'",
                                                        presence.From.Resource, presence.MucUser.Item.Reason);
                            }
                            else
                            {
                                message = string.Format("{0} has been banned", presence.From.Resource);
                            }

                            EventMucRoom eventMucRoom =
                                new EventMucRoom(TypicalEvent.Banned, this, presence.MucUser, message);

                            Events.Instance.OnEvent(this, eventMucRoom, DispatcherPriority.ApplicationIdle);
                        }
                    }

                    MucContact contact = MucRoster.OnPresence(presence, this);

                    if (contact != null && contact.Nick == Nick)
                    {
                        _me = contact;
                        NotifyPropertyChanged("Me");
                    }
                }
            }
            else
            {
                App.Current.Dispatcher.BeginInvoke(App._dispatcherPriority,
                                                   new PresenceCB(PresenceCallback), sender, presence, data);
            }
        }

        public void SendMessage(string text)
        {
            Message message = new Message();

            message.Type = MessageType.groupchat;
            message.To = _service.Jid;
            message.Body = text;

            _xmppClientConnection.Send(message);
        }

        public void ChangeMucTopic(string topic)
        {
            _mucManager.ChangeSubject(_service.Jid, topic);
        }

        public void ChangeNickname(string nick)
        {
            _mucManager.ChangeNickname(_service.Jid, nick);
        }

        private void OnBanResult(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                if (iq.Error.Code == ErrorCode.NotAllowed)
                {
                    MucContact mucContact = data as MucContact;

                    EventMucRoom eventMucRoom =
                        new EventMucRoom(TypicalEvent.Error, this, mucContact, string.Format(
                                                                                   "You are not allowed to ban {0}",
                                                                                   mucContact.Nick));

                    Events.Instance.OnEvent(this, eventMucRoom, DispatcherPriority.ApplicationIdle);
                }
            }
        }

        private void OnKickResult(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                if (iq.Error.Code == ErrorCode.NotAllowed)
                {
                    MucContact mucContact = data as MucContact;

                    EventMucRoom eventMucRoom =
                        new EventMucRoom(TypicalEvent.Error, this, mucContact, string.Format(
                                                                                   "You are not allowed to kick {0}",
                                                                                   mucContact.Nick));

                    Events.Instance.OnEvent(this, eventMucRoom, DispatcherPriority.ApplicationIdle);
                }
            }
        }

        public void Kick(MucContact mucContact, string reason)
        {
            _mucManager.KickOccupant(_service.Jid, mucContact.Nick, reason, new IqCB(OnKickResult), mucContact);
        }

        public void Ban(MucContact mucContact, string reason)
        {
            _mucManager.BanUser(_service.Jid, mucContact.UserJid, reason, new IqCB(OnBanResult), mucContact);
        }

        public void LeaveRoom(string message)
        {
            Presence presence = new Presence();
            presence.To = _service.Jid;
            presence.Type = PresenceType.unavailable;

            if (!string.IsNullOrEmpty(message))
            {
                presence.Status = message;
            }

            _xmppClientConnection.Send(presence);

            _xmppClientConnection.MesagageGrabber.Remove(_service.Jid);
            _xmppClientConnection.PresenceGrabber.Remove(_service.Jid);
        }

        public void Dispose()
        {
            _mucMessages.CollectionChanged -= _mucMessages_CollectionChanged;
        }

        private void OnPrivilegesResult(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                MucContact mucContact = data as MucContact;

                EventMucRoom eventMucRoom =
                    new EventMucRoom(TypicalEvent.Error, this, mucContact, "You have no privilege to do this");

                Events.Instance.OnEvent(this, eventMucRoom, DispatcherPriority.ApplicationIdle);
            }
        }

        public void GrantOwnerPrivilege(MucContact mucContact)
        {
            _mucManager.GrantOwnershipPrivileges(Service.Jid, mucContact.UserJid, new IqCB(OnPrivilegesResult),
                                                 mucContact);
        }

        public void GrantModerator(MucContact contact)
        {
            _mucManager.GrantModeratorPrivilegesPrivileges(Service.Jid, contact.Nick);
        }

        public void GrantMember(MucContact contact)
        {
            _mucManager.GrantMembershipship(Service.Jid, contact.Nick, String.Empty);
        }

        public void GrantAdmin(MucContact contact)
        {
            _mucManager.GrantAdminPrivileges(Service.Jid, contact.UserJid, new IqCB(OnPrivilegesResult), contact);
        }

        public void RevokeModerator(MucContact contact)
        {
            _mucManager.RevokeModerator(Service.Jid, contact.Nick, String.Empty, new IqCB(OnPrivilegesResult), contact);
        }

        public void RevokeMembership(MucContact contact)
        {
            _mucManager.RevokeMembership(Service.Jid, contact.Nick, String.Empty, new IqCB(OnPrivilegesResult), contact);
        }

        public void RevokeVoice(MucContact contact)
        {
            _mucManager.RevokeVoice(Service.Jid, contact.Nick, String.Empty, new IqCB(OnPrivilegesResult), contact);
        }

        public void GrantVoice(MucContact contact)
        {
            _mucManager.GrantVoice(Service.Jid, contact.Nick, String.Empty, new IqCB(OnPrivilegesResult), contact);
        }

        public void Destroy(MucContact contact)
        {
            _mucManager.DestroyRoom(_service.Jid, string.Empty, new IqCB(OnPrivilegesResult), contact);
        }

        internal List<TextRange> SelectText(Paragraph paragraph, string text)
        {
            List<TextRange> textRanges = new List<TextRange>();

            for (Inline inline = paragraph.Inlines.FirstInline; inline != null; inline = inline.NextInline)
            {
                Run run = null;

                Hyperlink hyperlink = inline as Hyperlink;

                if (hyperlink != null)
                {
                    run = hyperlink.Inlines.FirstInline as Run;
                }
                else
                {
                    run = inline as Run;
                }

                if (run != null)
                {
                    int firstStart = 0;

                    while (true)
                    {
                        if (firstStart > run.Text.Length - 1)
                        {
                            break;
                        }

                        int start = run.Text.IndexOf(text, firstStart, StringComparison.InvariantCultureIgnoreCase);
                        int end = start + text.Length;

                        firstStart = start + 1;

                        if (start >= 0)
                        {
                            TextRange textRange;

                            textRange = new TextRange(run.ContentStart.GetPositionAtOffset(start),
                                                      run.ContentStart.GetPositionAtOffset(end));

                            textRange.ApplyPropertyValue(Run.BackgroundProperty, _selectionFindBrush);

                            textRanges.Add(textRange);
                        }

                        else
                        {
                            break;
                        }
                    }
                }
            }

            return textRanges;
        }
    }
}