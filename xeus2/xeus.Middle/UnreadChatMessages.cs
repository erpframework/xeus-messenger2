using System.Collections.Generic;
using xeus2.xeus.Core;
using xeus2.xeus.UI.xeus.UI.Controls;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Middle
{
    internal class UnreadChatMessages : ObservableCollectionDisp<EventChatMessage>
    {
        /*
        private static readonly UnreadChatMessages _instance = new UnreadChatMessages();

        public UnreadChatMessages()
        {
            Events.Instance.OnEventRaised += Instance_OnEventRaised;
        }

        public void ClickedNotifyIcon()
        {
            lock (_syncObject)
            {
                EventChatMessage eventChatMessage = null;

                if (Count > 0)
                {
                    eventChatMessage = this[0];
                    RemoveAt(0);

                    Contact contact = eventChatMessage.Contact;

                    List<EventChatMessage> toBeRemoved = new List<EventChatMessage>();

                    foreach (EventChatMessage chatMessage in this)
                    {
                        if (JidUtil.BareEquals(chatMessage.Contact.Jid, contact.Jid))
                        {
                            toBeRemoved.Add(chatMessage);
                        }
                    }

                    foreach (EventChatMessage chatMessage in toBeRemoved)
                    {
                        Remove(chatMessage);
                    }
                }

                if (eventChatMessage != null)
                {
                    Chat.Instance.DisplayChat(eventChatMessage.Contact);
                }
            }
        }

        public static UnreadChatMessages Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Instance_OnEventRaised(object sender, Event myEvent)
        {
            if (myEvent is EventChatMessage)
            {
                lock (_syncObject)
                {
                    Add((EventChatMessage) myEvent);
                }
            }
        }
        */
    }
}