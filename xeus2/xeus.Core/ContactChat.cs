using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.chatstates;
using xeus2.Properties;
using xeus2.xeus.Data;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    internal class ContactChat : ChatBase<Message>, IDisposable
    {
        private readonly IContact _contact;

        private readonly ObservableCollectionDisp<Message> _messages = new ObservableCollectionDisp<Message>();
        private Chatstate _chatState = Chatstate.None;

        public ContactChat(IContact contact, XmppClientConnection clientConnection)
        {
            _contact = contact;
            _xmppClientConnection = clientConnection;

            _messages.CollectionChanged += _messages_CollectionChanged;
        }

        public override ObservableCollectionDisp<Message> Messages
        {
            get
            {
                return _messages;
            }
        }

        public IContact Contact
        {
            get
            {
                return _contact;
            }
        }

        public Chatstate ChatState
        {
            get
            {
                return _chatState;
            }
            set
            {
                _chatState = value;
                NotifyPropertyChanged("ChatState");
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _messages.CollectionChanged -= _messages_CollectionChanged;
        }

        #endregion

        private void _messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    {
                        GenerateChatDocument(e.NewItems);

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

        public void SendChatState(Chatstate chatState)
        {
            agsXMPP.protocol.client.Message message = new agsXMPP.protocol.client.Message();

            message.Type = MessageType.chat;
            message.To = _contact.Jid;
            message.From = Account.Instance.Self.Jid;
            message.Chatstate = chatState;

            _xmppClientConnection.Send(message);
        }

        public void SendMessage(string text)
        {
            agsXMPP.protocol.client.Message message = new agsXMPP.protocol.client.Message();

            message.Type = MessageType.chat;
            message.To = _contact.Jid;
            message.Body = text;
            message.From = Account.Instance.Self.Jid;
            message.Chatstate = Chatstate.active;

            Message chatMessage = new Message(message);

            lock (Messages._syncObject)
            {
                Database.SaveMessage(chatMessage);

                Messages.Add(chatMessage);
            }

            _xmppClientConnection.Send(message);
        }

        protected override Block GenerateMessage(Message message, Message previousMessage)
        {
            IContact sender = null;

            if (JidUtil.BareEquals(message.From, Account.Instance.Self.Jid))
            {
                if (message.From != null)
                {
                    sender = Account.Instance.Self;
                }
            }
            else
            {
                if (message.From != null)
                {
                    sender = _contact;
                }
            }

            if (sender != Account.Instance.Self)
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
                || !JidUtil.BareEquals(previousMessage.From, message.From)
                || (message.DateTime - previousMessage.DateTime >
                    TimeSpan.FromMinutes(Settings.Default.UI_GroupMessagesByMinutes)))
            {
                if (sender != null)
                {
                    if (sender == Account.Instance.Self)
                    {
                        paragraph.Inlines.Add(CreateRectangle(_ownAvatarBackground));
                    }

                    Bold contactName = new Bold();
                    contactName.Cursor = Cursors.Hand;
                    contactName.Foreground = _contactForeground;
                    contactName.Inlines.Add(sender.DisplayName);

                    paragraph.Inlines.Add(contactName);
                    paragraph.Inlines.Add("  ");
                }
                else
                {
                    paragraph.Inlines.Add(CreateRectangle(_bulbBackground));
                }

                newSection = true;
            }

            string body = message.Body;

            if (sender == null)
            {
                // system message
                paragraph.Foreground = _sysTextBrush;
            }

            if (!string.IsNullOrEmpty(body))
            {
                FormatParagraph(paragraph, body);
            }

            paragraph.DataContext = message;

            if (newSection)
            {
                groupSection = new Section();


                groupSection.Blocks.Add(paragraph);
                groupSection.Margin = new Thickness(3.0, 10.0, 3.0, 0.0);
                groupSection.BorderThickness = new Thickness(0.0, 2.0, 0.0, 0.0);
                groupSection.BorderBrush = _alternativeBackground;
            }

            groupSection.DataContext = sender;
            groupSection.Blocks.Add(paragraph);

            return groupSection;
        }
    }
}