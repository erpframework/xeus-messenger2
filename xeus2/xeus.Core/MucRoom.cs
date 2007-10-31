using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using agsXMPP;
using agsXMPP.Collections;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.chatstates;
using agsXMPP.protocol.x.muc;
using xeus2.Properties;
using xeus2.xeus.Commands;
using xeus2.xeus.Utilities;
using Uri=agsXMPP.Uri;

namespace xeus2.xeus.Core
{
    internal class MucRoom : ChatBase<MucMessage>, IDisposable, IChatState
    {
        #region Delegates

        public delegate void MucContactHandler(MucMessage mucMessage);

        #endregion

        private readonly MucManager _mucManager = null;
        private readonly MucMessages _mucMessages = new MucMessages();
        private readonly MucRoster _mucRoster = new MucRoster();

        private readonly Service _service;
        private MucContact _me = null;
        private string _nick;

        private string _subject;

        public MucRoom(Service service, XmppClientConnection xmppClientConnection, string nick, string password)
        {
            _service = service;
            _xmppClientConnection = xmppClientConnection;

            _mucManager = new MucManager(_xmppClientConnection);
            _nick = nick;

            _xmppClientConnection.MesagageGrabber.Add(service.Jid, new BareJidComparer(), MessageCallback, null);
            _xmppClientConnection.PresenceGrabber.Add(service.Jid, new BareJidComparer(), PresenceCallback, null);

            _mucMessages.CollectionChanged += _mucMessages_CollectionChanged;

            Events.Instance.OnEventRaised += Instance_OnEventRaised;

            if (service.IsMucPasswordProtected)
            {
                _mucManager.JoinRoom(service.Jid, nick, password);
            }
            else
            {
                _mucManager.JoinRoom(service.Jid, nick);
            }

            RecentItems.Instance.Add(this);
        }

        public MucContact Me
        {
            get
            {
                return _me;
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

        public MucRoster MucRoster
        {
            get
            {
                return _mucRoster;
            }
        }

        public Service Service
        {
            get
            {
                return _service;
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

        public override ObservableCollectionDisp<MucMessage> Messages
        {
            get
            {
                return _mucMessages;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _mucMessages.CollectionChanged -= _mucMessages_CollectionChanged;
        }

        #endregion

        public event MucContactHandler OnClickMucContact;

        public void SendChatState(Chatstate chatState)
        {
            if (_service.DiscoInfo != null && _service.DiscoInfo.HasFeature(Uri.CHATSTATES))
            {
                agsXMPP.protocol.client.Message message = new agsXMPP.protocol.client.Message();

                message.Type = MessageType.groupchat;
                message.To = _service.Jid;
                message.Chatstate = chatState;

                _xmppClientConnection.Send(message);
            }
        }

        public void SendPresence(ShowType showType, string status)
        {
            if (_me.Show != showType
                || _me.StatusText != status)
            {
                Presence presence = new Presence(showType, status);
                presence.From = Account.Instance.Self.FullJid;
                presence.To = _me.MucJid;

                _xmppClientConnection.Send(presence);
            }
        }

        private void Instance_OnEventRaised(object sender, Event myEvent)
        {
            EventMucRoom eventMucRoom = myEvent as EventMucRoom;

            if (eventMucRoom != null && eventMucRoom.MucRoom == this)
            {
                TimeSpan timeSpan = DateTime.Now - _roomStart;

                if (eventMucRoom.TypicalEventCode != TypicalEvent.Joined || timeSpan >= new TimeSpan(0, 0, 5))
                {
                    MucMessage mucMessage =
                        new MucMessage(new agsXMPP.protocol.client.Message(Account.Instance.Self.Jid, Service.Jid,
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

        protected override Block GenerateMessage(MucMessage message, MucMessage previousMessage)
        {
            if (message.Sender != null)
            {
                _lastMessage = message;
                NotifyPropertyChanged("LastMessage");
            }

            LoadBrushes();

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
                (message.DateTime.DateTime - previousMessage.DateTime.DateTime >
                 TimeSpan.FromMinutes(Settings.Default.UI_GroupMessagesByMinutes)))
            {
                if (!string.IsNullOrEmpty(message.Sender))
                {
                    if (message.Sender == Nick)
                    {
                        // paragraph.Inlines.Add(CreateRectangle(_ownAvatarBackground));
                        InsertAvatar(paragraph, Account.Instance.Self.Image);
                    }
                    else
                    {
                        if (message.Sender != null)
                        {
                            //_mucRoster.Find();
                        }
                    }

                    Bold contactName = new Bold();
                    contactName.Cursor = Cursors.Hand;
                    contactName.Foreground = _contactForeground;
                    contactName.Inlines.Add(message.Sender);

                    paragraph.Inlines.Add(contactName);
                    paragraph.Inlines.Add("  ");

                    contactName.MouseDown += contactName_MouseDown;
                    contactName.MouseEnter += contactName_MouseEnter;
                    contactName.MouseLeave += contactName_MouseLeave;
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
                if (body.StartsWith("{" + TypicalEvent.RoomCreated + "}"))
                {
                    paragraph.Inlines.Add(
                        "If you want to make this room permanent, configure it here ");

                    Button buttonOpenConfig = new Button();
                    buttonOpenConfig.Click += buttonOpenConfig_Click;
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

                FormatParagraph(paragraph, body);
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

        private static string PrepareBody(string originalBody, TypicalEvent typicalEvent)
        {
            return originalBody.Replace("{" + typicalEvent + "}", String.Empty);
        }

        private void buttonOpenConfig_Click(object sender, RoutedEventArgs e)
        {
            MucCommands.Options.Execute(this, null);
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

        private void MessageCallback(object sender, agsXMPP.protocol.client.Message msg, object data)
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

                    if (msg.Body != null)
                    {
                        _mucMessages.OnMessage(msg, contact);
                    }

                    if (msg.Chatstate != Chatstate.None && contact != Me)
                    {
                        ChatState = msg.Chatstate;
                    }
                }
            }
            else
            {
                App.InvokeSafe(App._dispatcherPriority,
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

                        SendPresence(Account.Instance.Self.MyShow, Account.Instance.Self.StatusText);
                    }
                }
            }
            else
            {
                App.InvokeSafe(App._dispatcherPriority,
                               new PresenceCB(PresenceCallback), sender, presence, data);
            }
        }

        public void SendMessage(string text)
        {
            agsXMPP.protocol.client.Message message = new agsXMPP.protocol.client.Message();

            message.Type = MessageType.groupchat;
            message.To = _service.Jid;
            message.Body = text;
            message.Chatstate = Chatstate.active;

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
                    MucContact mucContact = (MucContact) data;

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
                    MucContact mucContact = (MucContact) data;

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
            _mucManager.KickOccupant(_service.Jid, mucContact.Nick, reason, OnKickResult, mucContact);
        }

        public void Ban(MucContact mucContact, string reason)
        {
            _mucManager.BanUser(_service.Jid, mucContact.Jid, reason, OnBanResult, mucContact);
        }

        public void LeaveRoom(string message)
        {
            _xmppClientConnection.MesagageGrabber.Remove(_service.Jid);
            _xmppClientConnection.PresenceGrabber.Remove(_service.Jid);

            Presence presence = new Presence();
            presence.To = _service.Jid;
            presence.Type = PresenceType.unavailable;

            if (!string.IsNullOrEmpty(message))
            {
                presence.Status = message;
            }

            _xmppClientConnection.Send(presence);
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
            _mucManager.GrantOwnershipPrivileges(Service.Jid, mucContact.Jid, OnPrivilegesResult,
                                                 mucContact);
        }

        public void GrantModerator(MucContact contact)
        {
            _mucManager.GrantModeratorPrivileges(Service.Jid, contact.Nick);
        }

        public void GrantMember(MucContact contact)
        {
            _mucManager.GrantMembership(Service.Jid, contact.Nick, String.Empty);
        }

        public void GrantAdmin(MucContact contact)
        {
            _mucManager.GrantAdminPrivileges(Service.Jid, contact.Jid, OnPrivilegesResult, contact);
        }

        public void RevokeModerator(MucContact contact)
        {
            _mucManager.RevokeModerator(Service.Jid, contact.Nick, String.Empty, OnPrivilegesResult, contact);
        }

        public void RevokeMembership(MucContact contact)
        {
            _mucManager.RevokeMembership(Service.Jid, contact.Nick, String.Empty, OnPrivilegesResult, contact);
        }

        public void RevokeVoice(MucContact contact)
        {
            _mucManager.RevokeVoice(Service.Jid, contact.Nick, String.Empty, OnPrivilegesResult, contact);
        }

        public void GrantVoice(MucContact contact)
        {
            _mucManager.GrantVoice(Service.Jid, contact.Nick, String.Empty, OnPrivilegesResult, contact);
        }

        public void Destroy(MucContact contact)
        {
            _mucManager.DestroyRoom(_service.Jid, string.Empty, OnPrivilegesResult, contact);
        }
    }
}