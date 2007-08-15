using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using xeus2.Properties;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MucConversation.xaml
    /// </summary>
    public partial class MucConversation : UserControl
    {
        private MucRoom _mucRoom;

        public MucConversation()
        {
            InitializeComponent();
        }

        internal void MucConversationInit(Service service, string nick, string password)
        {
            _mucRoom = Account.Instance.JoinMuc(service, nick, password);

            _mucRoom.OnClickMucContact += new MucRoom.MucContactHandler(_mucRoom_OnClickMucContact);
            _mucRoom.MucMessages.CollectionChanged +=
                new NotifyCollectionChangedEventHandler(MucMessages_CollectionChanged);

            _mucRoom.MucRoster.CollectionChanged += new NotifyCollectionChangedEventHandler(MucRoster_CollectionChanged);

            DataContext = _mucRoom;

            new MucNikcnames(_text, _mucRoom);

            Unloaded += new RoutedEventHandler(MucConversation_Unloaded);

        }

        private void MucConversation_Unloaded(object sender, RoutedEventArgs e)
        {
            _mucRoom.LeaveRoom(Settings.Default.MucLeaveMsg);
        }

        private void MucRoster_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList items = null;

            if (e.NewItems != null)
            {
                items = e.NewItems;
            }
            else if (e.OldItems != null)
            {
                items = e.OldItems;
            }

            if (items != null)
            {
                foreach (MucContact mucContact in items)
                {
                    StringBuilder message = new StringBuilder();

                    message.Append(mucContact.Nick);

                    EventMucRoom eventMucRoom;

                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            {
                                message.AppendFormat(" is {0}", mucContact.Role);

                                if (mucContact.Affiliation != Affiliation.none)
                                {
                                    message.AppendFormat(" and {0}", mucContact.Affiliation);
                                }

                                eventMucRoom = new EventMucRoom(TypicalEvent.Joined, _mucRoom, mucContact.Presence.MucUser, message.ToString());
                                break;
                            }
                        default:
                            {
                                message.Append(" has left the room");
                                eventMucRoom = new EventMucRoom(TypicalEvent.Left, _mucRoom, mucContact.Presence.MucUser, message.ToString());
                                break;
                            }
                    }

                    Events.Instance.OnEvent(this, eventMucRoom);
                }
            }
        }

        private ScrollViewer _scrollViewer = null;

        private void MucMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = (ScrollViewer) _flowViewer.Template.FindName("PART_ContentHost", _flowViewer);
            }

            if (_scrollViewer.VerticalOffset >= _scrollViewer.ScrollableHeight - 10.0)
            {
                _scrollViewer.ScrollToBottom();
            }
        }

        private void _mucRoom_OnClickMucContact(MucMessage mucMessage)
        {
            if (!string.IsNullOrEmpty(mucMessage.Sender))
            {
                _text.Text = ReplaceNick(_text.Text, mucMessage.Sender);
                _text.Focus();
                _text.CaretIndex = _text.Text.Length;
            }
        }

        private bool IsStillInRoom(string nick)
        {
            foreach (MucContact mucContact in _mucRoom.MucRoster)
            {
                if (mucContact.Nick == nick)
                {
                    return true;
                }
            }

            return false;
        }


        private string ReplaceNick(string text, string sender)
        {
            if (!IsStillInRoom(sender))
            {
                return text;
            }

            int colon = text.IndexOf(":");

            string result = text;

            if (colon >= 0)
            {
                string nick = text.Substring(0, colon);

                foreach (MucContact mucContact in _mucRoom.MucRoster)
                {
                    if (mucContact.Nick == nick)
                    {
                        result = text.Substring(colon + 2, text.Length - (colon + 2));
                        break;
                    }
                }
            }

            return sender + ": " + result;
        }

        protected void OnSendMessage(object sender, RoutedEventArgs eventArgs)
        {
            _mucRoom.SendMessage(_text.Text);
            _text.Text = string.Empty;
        }
    }
}