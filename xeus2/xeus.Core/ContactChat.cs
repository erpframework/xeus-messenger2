using System;
using System.Windows.Documents;

namespace xeus2.xeus.Core
{
    internal class ContactChat : ChatBase<Message>
    {
        private readonly IContact _contact;

        public ContactChat(IContact contact)
        {
            _contact = contact;
        }

        public override ObservableCollectionDisp<Message> Messages
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IContact Contact
        {
            get
            {
                return _contact;
            }
        }

        protected override Block GenerateMessage(Message message, Message previousMessage)
        {
            throw new NotImplementedException();
        }
    }
}