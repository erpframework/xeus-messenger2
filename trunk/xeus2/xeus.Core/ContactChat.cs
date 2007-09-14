using System;
using System.Windows.Documents;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.chatstates;

namespace xeus2.xeus.Core
{
    internal class ContactChat : ChatBase<Message>
    {
        private readonly IContact _contact;

        private readonly ObservableCollectionDisp<Message> _messages = new ObservableCollectionDisp<Message>();

        public ContactChat(IContact contact)
        {
            _contact = contact;
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

        public void SendMessage(string text)
        {
            agsXMPP.protocol.client.Message message = new agsXMPP.protocol.client.Message();

            message.Type = MessageType.chat;
            message.To = _contact.Jid;
            message.Body = text;
            message.From = Account.Instance.Self.Jid;
            message.Chatstate = Chatstate.active;

            _xmppClientConnection.Send(message);

            Message chatMessage = new Message(message);

            lock (Messages._syncObject)
            {
                Messages.Add(chatMessage);
            }

            _xmppClientConnection.Send(message);
        }

        protected override Block GenerateMessage(Message message, Message previousMessage)
        {
            throw new NotImplementedException();
        }
    }
}