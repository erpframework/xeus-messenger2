using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using agsXMPP;
using agsXMPP.Collections;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using xeus2.Properties;
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

        public MucContact Me
        {
            get
            {
                lock (_mucRoster._syncObject)
                {
                    foreach (MucContact mucContact in _mucRoster)
                    {
                        if (mucContact.Nick == Nick)
                        {
                            return mucContact;
                        }
                    }
                }

                return null;
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

        void Instance_OnEventRaised(object sender, Event myEvent)
        {
            EventMucRoom eventMucRoom = myEvent as EventMucRoom;

            if (eventMucRoom != null && eventMucRoom.MucRoom == this)
            {
                _lastEvent = eventMucRoom;

                NotifyPropertyChanged("LastEvent");

                MucMessage mucMessage = new MucMessage(new Message(Account.Instance.MyJid, Service.Jid,
                                                                   eventMucRoom.Message), null);

                _mucMessages.Add(mucMessage);
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
        private static Brush _textDimBrush;
        private static Brush _contactForeground;
        private static Brush _alternativeBackground;
        private static Brush _bulbBackground;
        private static Brush _ownAvatarBackground;
        private static Brush _timeRecentBackground;
        private static Brush _timeOlderBackground;
        private static Brush _timeOldBackground;
        private static Brush _timeOldestBackground;

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
                case MucMessage.MucMessageOldness.Recent:
                    {
                        timeBrush = _timeRecentBackground;
                        break;
                    }
                case MucMessage.MucMessageOldness.Older:
                    {
                        timeBrush = _timeOlderBackground;
                        break;
                    }
                case MucMessage.MucMessageOldness.Old:
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

        Rectangle CreateTimeRect(MucMessage message)
        {
            Rectangle timeRectangle = new Rectangle();
            timeRectangle.Fill = GetMessageTimeBrush(message);
            timeRectangle.Width = 16;
            timeRectangle.Height = 16;

            timeRectangle.Margin = new Thickness(-10.0, 2.0, 4.0, 0.0);
            timeRectangle.Cursor = Cursors.Help;
            //timeRectangle.DataContext = message;
            _relativeTimes.Add(timeRectangle);

            return timeRectangle;
        }

        public Block GenerateMessage(MucMessage message, MucMessage previousMessage)
        {
            if (_forMeForegorund == null)
            {
                _forMeForegorund = StyleManager.GetBrush("forme_text_design");

                _textBrush = StyleManager.GetBrush("text_design");
                _sysTextBrush = StyleManager.GetBrush("sys_text_design");
                _textDimBrush = StyleManager.GetBrush("textdim_design");

                _alternativeBackground = StyleManager.GetBrush("back_alt");

                _contactForeground = StyleManager.GetBrush("muc_contact_fore");
                _bulbBackground = StyleManager.GetBrush("jabber_design");
                _ownAvatarBackground = StyleManager.GetBrush("aff_none_design");

                _timeRecentBackground = StyleManager.GetBrush("time_now_design");
                _timeOlderBackground = StyleManager.GetBrush("time_older_design");
                _timeOldBackground = StyleManager.GetBrush("time_old_design");
                _timeOldestBackground = StyleManager.GetBrush("time_oldest_design");
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

            if (string.IsNullOrEmpty(message.Sender))
            {
                // system message
                paragraph.Foreground = _sysTextBrush;
            }

            if (!string.IsNullOrEmpty(message.Body))
            {
                MatchCollection matches = _urlregex.Matches(message.Body);

                if (matches.Count > 0)
                {
                    string[] founds = new string[matches.Count];

                    for (int i = 0; i < founds.Length; i++)
                    {
                        founds[i] = matches[i].ToString();
                    }

                    string[] bodies = message.Body.Split(founds, StringSplitOptions.RemoveEmptyEntries);

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
                    paragraph.Inlines.Add(message.Body);
                }
            }
            else if (!string.IsNullOrEmpty(message.Subject))
            {
                paragraph.Inlines.Add("Changed topic: " + message.Subject);
            }

            paragraph.DataContext = message;

            if (!string.IsNullOrEmpty(message.Body)
                && TextUtil.ContainsNick(Nick, message.Body))
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
                _lastEvent.RefreshrelativeTime();
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
                    if (presence.Type == PresenceType.unavailable
                        && presence.MucUser != null
                        && presence.MucUser.Status != null)
                    {
                        if (presence.MucUser.Status.Code == StatusCode.NewNickname)
                        {
                            EventMucRoom eventMucRoom = new EventMucRoom( TypicalEvent.NickChange,
                                                                            this, presence.MucUser,
                                                                         string.Format(
                                                                             "'{0}' is now known as '{1}'",
                                                                             presence.From.Resource,
                                                                             presence.MucUser.Item.Nickname));

                            Events.Instance.OnEvent(this, eventMucRoom);

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
                                message = string.Format("{0} has been kicked from the room with reason '{1}'",
                                                        presence.From.Resource, presence.MucUser.Item.Reason);
                            }
                            else
                            {
                                message = string.Format("{0} has been kicked from the room", presence.From.Resource);
                            }

                            EventMucRoom eventMucRoom = new EventMucRoom(TypicalEvent.Kicked, this, presence.MucUser, message);

                            Events.Instance.OnEvent(this, eventMucRoom);
                        }

                        if (presence.MucUser.Status.Code == StatusCode.Banned)
                        {
                            string message;

                            if (presence.MucUser.Item != null
                                && !string.IsNullOrEmpty(presence.MucUser.Item.Reason))
                            {
                                message = string.Format("{0} has been banned from the room with reason '{1}'",
                                                        presence.From.Resource, presence.MucUser.Item.Reason);
                            }
                            else
                            {
                                message = string.Format("{0} has been banned from the room", presence.From.Resource);
                            }

                            EventMucRoom eventMucRoom = new EventMucRoom(TypicalEvent.Banned, this, presence.MucUser, message);

                            Events.Instance.OnEvent(this, eventMucRoom);
                        }
                    }

                    MucRoster.OnPresence(presence, this);
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
            string nick = "this user";

            if (iq.Error != null)
            {
                if (iq.Error.Code == ErrorCode.NotAllowed)
                {
                    MucMessage mucMessage = new MucMessage(new Message(Account.Instance.MyJid, Service.Jid,
                                                                       string.Format(
                                                                           "You are not allowed to ban {0}",
                                                                           nick)), null);

                    _mucMessages.Add(mucMessage);
                }
            }
        }

        private void OnKickResult(object sender, IQ iq, object data)
        {
            string nick = "this user";

            if (iq.Error != null)
            {
                if (iq.Error.Code == ErrorCode.NotAllowed)
                {
                    MucMessage mucMessage = new MucMessage(new Message(Account.Instance.MyJid, Service.Jid,
                                                                       string.Format(
                                                                           "You are not allowed to kick {0}",
                                                                           nick)), null);

                    _mucMessages.Add(mucMessage);
                }
            }
        }

        public void Kick(string nick, string reason)
        {
            _mucManager.KickOccupant(_service.Jid, nick, reason, new IqCB(OnKickResult));
        }

        public void Ban(Jid jid, string reason)
        {
            _mucManager.BanUser(_service.Jid, jid, reason, new IqCB(OnBanResult));
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
    }
}