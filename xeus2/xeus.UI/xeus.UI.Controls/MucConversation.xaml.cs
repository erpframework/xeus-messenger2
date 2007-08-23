using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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

            DataContext = _mucRoom;

            new MucNikcnames(_text, _mucRoom);

            Unloaded += new RoutedEventHandler(MucConversation_Unloaded);

            _mucRoom.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_mucRoom_PropertyChanged);

            _text.Focus();
        }

        void _mucRoom_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Me" && _mucRoom.Me != null)
            {
                _mucRoom.Me.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Me_PropertyChanged);

                SetMyAffIcon();
            }
        }

        void Me_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Affiliation")
            {
                SetMyAffIcon();
            }
        }

        void SetMyAffIcon()
        {
            Brush brush ; 

            switch (_mucRoom.Me.Affiliation)
            {
                case Affiliation.admin:
                    {
                        brush = StyleManager.GetBrush("aff_admin_design");
                        break;
                    }
                case Affiliation.member:
                    {
                        brush = StyleManager.GetBrush("aff_member_design");
                        break;
                    }
                case Affiliation.outcast:
                    {
                        brush = StyleManager.GetBrush("aff_outcast_design");
                        break;
                    }
                case Affiliation.owner:
                    {
                        brush = StyleManager.GetBrush("aff_owner_design");
                        break;
                    }
                default:
                    {
                        brush = StyleManager.GetBrush("aff_none_design");
                        break;
                    }
            }

            _contactButton.Background = brush;
        }

        private void MucConversation_Unloaded(object sender, RoutedEventArgs e)
        {
            _mucRoom.LeaveRoom(Settings.Default.MucLeaveMsg);
        }

        void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return &&
                                    (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                OnSendMessage(sender, e);
            }
        }
        private ScrollViewer _scrollViewer = null;

        private void MucMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = (ScrollViewer) _flowViewer.Template.FindName("PART_ContentHost", _flowViewer);
            }

            if (_scrollViewer.VerticalOffset >= _scrollViewer.ScrollableHeight - 15.0)
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

        string TextStartsWithNick(string text)
        {
            foreach (MucContact mucContact in _mucRoom.MucRoster)
            {
                if (text.StartsWith(string.Format("{0}:", mucContact.Nick)))
                {
                    return mucContact.Nick;
                }
            }

            return null;
        }

        protected void OnSendMessage(object sender, RoutedEventArgs eventArgs)
        {
            _mucRoom.SendMessage(_text.Text);

            string nick = TextStartsWithNick(_text.Text);

            if (nick != null)
            {
                _text.Text = nick + ": ";
                _text.SelectAll();
            }
            else
            {
                _text.Text = string.Empty;
            }

            _text.Focus();
        }

        protected void OnContactClick(object sender, RoutedEventArgs eventArgs)
        {
            ContextMenu menu = FindResource("MucMainMenu") as ContextMenu;

            menu.PlacementTarget = _contactButton;

            menu.IsOpen = true;
        }    
    }
}