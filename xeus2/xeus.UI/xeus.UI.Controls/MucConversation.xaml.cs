using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using agsXMPP.protocol.x.muc;
using xeus2.Properties;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MucConversation.xaml
    /// </summary>
    public partial class MucConversation : UserControl, IFlyoutContainer
    {
        private readonly ChatStateNotificator _chatStateNotificator = new ChatStateNotificator();

        private readonly InlineMethod _inlineMethod = new InlineMethod();
        private readonly object _textsLock = new object();
        private MucMessage _lastFoundItem = null;
        private string _lastSearch = String.Empty;
        private MucRoom _mucRoom;
        private List<TextRange> _previousTextRanges = new List<TextRange>();
        private ScrollViewer _scrollViewer = null;
        private List<KeyValuePair<string, MucMessage>> _texts = null;
        private string _textToSearch = String.Empty;

        public MucConversation()
        {
            InitializeComponent();
        }

        internal void MucConversationInit(Service service, string nick, string password)
        {
            _mucRoom = Account.Instance.JoinMuc(service, nick, password);

            _mucRoom.OnClickMucContact += _mucRoom_OnClickMucContact;

            DataContext = _mucRoom;

            new MucNikcnames(_text, _mucRoom);

            Loaded += MucConversation_Loaded;

            _text.Loaded += _text_Loaded;

            Account.Instance.Self.PropertyChanged += Self_PropertyChanged;
        }

        void Self_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Show":
                case "StatusText":
                case "MyShow":
                    {
                        _mucRoom.SendPresence(Account.Instance.Self.MyShow, Account.Instance.Self.StatusText);
                        break;
                    }
            }
        }

        private void MucConversation_Loaded(object sender, RoutedEventArgs e)
        {
            Unloaded += MucConversation_Unloaded;

            _flowViewer.PreviewKeyDown += MucConversation_PreviewKeyDown;

            PreviewKeyDown += MucConversation_PreviewKeyDownWindow;

            _mucRoom.Messages.CollectionChanged += MucMessages_CollectionChanged;
            _mucRoom.PropertyChanged += _mucRoom_PropertyChanged;

            _inlineMethod.Finished += _inlineMethod_Finished;
            _inlineSearch.TextChanged += _inlineSearch_TextChanged;
            _inlineSearch.Closed += _inlineSearch_Closed;

            GotFocus += MucConversation_GotFocus;
            LostFocus += MucConversation_LostFocus;

            _chatStateNotificator.StateChanged += _chatStateNotificator_StateChanged;
        }

        private void _chatStateNotificator_StateChanged(agsXMPP.protocol.extensions.chatstates.Chatstate chatstate)
        {
            _mucRoom.SendChatState(chatstate);
        }

        private void MucConversation_LostFocus(object sender, RoutedEventArgs e)
        {
            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.inactive);
        }

        private void MucConversation_GotFocus(object sender, RoutedEventArgs e)
        {
            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.active);
        }

        private void _text_Loaded(object sender, RoutedEventArgs e)
        {
            _inlineSearch.Visibility = Visibility.Collapsed;
            _text.Focus();
        }

        private void MucConversation_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // all from the viewer
            if (_inlineSearch != null)
            {
                _inlineSearch.SendKey(e.Key);
            }
        }

        private void MucConversation_PreviewKeyDownWindow(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_inlineSearch.Visibility == Visibility.Visible)
                {
                    e.Handled = _inlineSearch.SendKey(e.Key);
                }
                else
                {
                    // Instance.RemoveCurrentTab();
                }
            }
            else if (_inlineSearch != null)
            {
                if (_inlineSearch.IsGlobalSearchKey(e.Key))
                {
                    e.Handled = _inlineSearch.SendKey(e.Key);
                }
            }
        }

        private void _inlineSearch_Closed(bool isEnter)
        {
            lock (_textsLock)
            {
                _texts = null;
                CleanSelection();
            }
        }

        private void CleanSelection()
        {
            foreach (TextRange range in _previousTextRanges)
            {
                range.ApplyPropertyValue(Inline.BackgroundProperty, null);
            }

            _previousTextRanges.Clear();
        }

        private void _inlineSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _inlineMethod.Go(new InlineParam(SearchInList, _inlineSearch.Text));
        }

        private void _inlineMethod_Finished(object result)
        {
            MucMessage mucMessage = (MucMessage) result;
            SelectItem(mucMessage);
        }

        private void SelectItem(MucMessage item)
        {
            if (App.CheckAccessSafe())
            {
                _inlineSearch.NotFound = true;

                if (item != null)
                {
                    foreach (Block block in _flowViewer.Document.Blocks)
                    {
                        Section section = block as Section;

                        if (section != null)
                        {
                            foreach (Block sectionBlock in section.Blocks)
                            {
                                if (sectionBlock.DataContext == item)
                                {
                                    sectionBlock.BringIntoView();

                                    SelectText(sectionBlock as Paragraph, _textToSearch);

                                    _inlineSearch.NotFound = false;

                                    break;
                                }
                            }
                        }

                        if (!_inlineSearch.NotFound)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                App.InvokeSafe(DispatcherPriority.Send, new SelectItemCallback(SelectItem), item);
            }
        }

        private void SelectText(Paragraph paragraph, string text)
        {
            CleanSelection();
            _previousTextRanges = MucRoom.SelectText(paragraph, text);
        }

        private object SearchInList(ref bool stop, object param)
        {
            lock (_textsLock)
            {
                if (_texts == null)
                {
                    _texts = new List<KeyValuePair<string, MucMessage>>();

                    lock (_mucRoom.Messages._syncObject)
                    {
                        foreach (MucMessage mucMessage in _mucRoom.Messages)
                        {
                            if (!string.IsNullOrEmpty(mucMessage.Body))
                            {
                                _texts.Add(
                                    new KeyValuePair<string, MucMessage>(mucMessage.Body.ToUpper().Trim(), mucMessage));
                            }
                        }
                    }
                }
            }

            MucMessage found = null;

            _textToSearch = (string) param;

            string toFound = ((string) param).ToUpper();

            bool searchNext = (_lastSearch == toFound);

            _lastSearch = toFound;

            if (searchNext && _lastFoundItem != null)
            {
                bool fromHere = false;

                foreach (KeyValuePair<string, MucMessage> body in _texts)
                {
                    if (stop)
                    {
                        return null;
                    }

                    if (fromHere && body.Key.Contains(toFound))
                    {
                        found = body.Value;
                        break;
                    }

                    if (_lastFoundItem == body.Value)
                    {
                        fromHere = true;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, MucMessage> body in _texts)
                {
                    if (stop)
                    {
                        return null;
                    }

                    if (((string) param) == String.Empty)
                    {
                        return null;
                    }

                    if (body.Key.Contains(toFound))
                    {
                        found = body.Value;
                        break;
                    }
                }
            }

            _lastFoundItem = found;
            return found;
        }

        private void _mucRoom_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Me" && _mucRoom.Me != null)
            {
                _mucRoom.Me.PropertyChanged += Me_PropertyChanged;

                SetMyAffIcon();
            }
        }

        private void Me_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Affiliation")
            {
                SetMyAffIcon();
            }
        }

        private void SetMyAffIcon()
        {
            _mucRoom.SendPresence(Account.Instance.Self.MyShow, Account.Instance.Self.StatusText);

            Brush brush;

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
            _mucRoom.Messages.CollectionChanged -= MucMessages_CollectionChanged;
            _mucRoom.PropertyChanged -= _mucRoom_PropertyChanged;

            _inlineMethod.Finished -= _inlineMethod_Finished;
            _inlineSearch.TextChanged -= _inlineSearch_TextChanged;
            _inlineSearch.Closed -= _inlineSearch_Closed;

            _chatStateNotificator.StateChanged -= _chatStateNotificator_StateChanged;

            Account.Instance.Self.PropertyChanged -= Self_PropertyChanged;
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    e.Handled = false;

                    TextBox textBox = sender as TextBox;

                    if (textBox != null && textBox.AcceptsReturn)
                    {
                        int index = textBox.CaretIndex + 1;
                        textBox.Text = textBox.Text.Insert(textBox.CaretIndex, Environment.NewLine);
                        textBox.CaretIndex = index;
                    }
                }
                else
                {
                    e.Handled = true;
                    OnSendMessage(sender, e);
                }
            }
        }

        private void MucMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = (ScrollViewer) _flowViewer.Template.FindName("PART_ContentHost", _flowViewer);
            }

            bool force = false;

            if (e.NewItems != null && _mucRoom.Me != null)
            {
                foreach (MucMessage message in e.NewItems)
                {
                    if (message.Sender == _mucRoom.Me.Nick)
                    {
                        force = true;
                        break;
                    }
                }
            }

            if (force || _scrollViewer.VerticalOffset >= _scrollViewer.ScrollableHeight - 15.0)
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

        private string TextStartsWithNick(string text)
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

            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.inactive);

            _text.Focus();
        }

        protected void OnContactClick(object sender, RoutedEventArgs eventArgs)
        {
            ContextMenu menu = (ContextMenu) FindResource("MucMainMenu");

            menu.PlacementTarget = _contactButton;

            menu.IsOpen = true;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MucCommands.ModifySubject.CanExecute(_mucRoom, null))
            {
                MucCommands.ModifySubject.Execute(_mucRoom, null);
            }
        }

        private void _text_TextChanged(object sender, TextChangedEventArgs e)
        {
            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.composing);
        }

        #region Nested type: SelectItemCallback

        private delegate void SelectItemCallback(MucMessage item);

        #endregion

        public event CloseContainer CloseMe;

        public void Closing()
        {
            _mucRoom.LeaveRoom(Settings.Default.MucLeaveMsg);
        }
    }
}