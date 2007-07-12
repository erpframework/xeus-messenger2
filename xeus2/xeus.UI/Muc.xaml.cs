using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using xeus2.Properties;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for Muc.xaml
    /// </summary>
    public partial class Muc : Window
    {
        private MucRoom _mucRoom;

        internal Muc(Service service, string nick, string password)
        {
            InitializeComponent();

            _mucRoom = Account.Instance.JoinMuc(service, nick, password);

            _mucRoom.OnClickMucContact += new MucRoom.MucContactHandler(_mucRoom_OnClickMucContact);
            _mucRoom.MucMessages.CollectionChanged +=
                new NotifyCollectionChangedEventHandler(MucMessages_CollectionChanged);

            _mucRoom.MucRoster.CollectionChanged += new NotifyCollectionChangedEventHandler(MucRoster_CollectionChanged);

            DataContext = _mucRoom;

            MucCommands.RegisterCommands(this);
        }

        private void MucRoster_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList items = null;
            
            if (e.NewItems != null)
            {
                items = e.NewItems;   
            }
            else if ( e.OldItems != null )
            {
                items = e.OldItems;   
            }

            if ( items != null )
            {
                foreach (MucContact mucContact in items)
                {
                    StringBuilder message = new StringBuilder();

                    message.Append(mucContact.Nick);

                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            {
                                message.AppendFormat(" is {0}", mucContact.Role);

                                if (mucContact.Affiliation != Affiliation.none)
                                {
                                    message.AppendFormat(" and {0}", mucContact.Affiliation);
                                }
                                break;
                            }
                        default:
                            {
                                message.Append(" has left the room");
                                break;
                            }
                    }

                    MucMessage mucMessage = new MucMessage(new Message(Account.Instance.MyJid, _mucRoom.Service.Jid,
                                                                       message.ToString()), null);

                    _mucRoom.MucMessages.Add(mucMessage);
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

            if (_scrollViewer.VerticalOffset >= _scrollViewer.ScrollableHeight - 2.0)
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

        private string ReplaceNick(string text, string sender)
        {
            int colon = text.IndexOf(": ");

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

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            _mucRoom.LeaveRoom(Settings.Default.MucLeaveMsg);
        }
    }
}