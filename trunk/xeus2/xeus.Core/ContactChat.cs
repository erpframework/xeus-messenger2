using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace xeus2.xeus.Core
{
    internal class ContactChat : ChatBase<Message>
    {
        public override ObservableCollectionDisp<Message> Messages
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override Block GenerateMessage(Message message, Message previousMessage)
        {
            throw new NotImplementedException();
        }
    }
}
