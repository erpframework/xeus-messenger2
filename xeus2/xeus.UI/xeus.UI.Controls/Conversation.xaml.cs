using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for Conversation.xaml
    /// </summary>
    public partial class Conversation : UserControl
    {
        private delegate void SelectItemCallback(Message item);

        private readonly ChatStateNotificator _chatStateNotificator = new ChatStateNotificator();
        private readonly ContactChat _contactChat;

        private readonly InlineMethod _inlineMethod = new InlineMethod();
        private Message _lastFoundItem = null;
        private string _lastSearch = String.Empty;
        private List<TextRange> _previousTextRanges = new List<TextRange>();
        private ScrollViewer _scrollViewer = null;
        private List<KeyValuePair<string, Message>> _texts = null;
        private readonly object _textsLock = new object();
        private string _textToSearch = String.Empty;

        internal Conversation(ContactChat contactChat)
        {
            InitializeComponent();

            DataContext = contactChat;
            _contactChat = contactChat;

            Loaded += Conversation_Loaded;

            _text.Loaded += _text_Loaded;
        }

        void Conversation_PreviewKeyDown(object sender, KeyEventArgs e)
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

        void Conversation_LostFocus(object sender, RoutedEventArgs e)
        {
            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.inactive);
        }

        void Conversation_GotFocus(object sender, RoutedEventArgs e)
        {
            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.active);
        }

        void Conversation_Loaded(object sender, RoutedEventArgs e)
        {
            GotFocus += Conversation_GotFocus;
            LostFocus += Conversation_LostFocus;
            PreviewKeyDown += Conversation_PreviewKeyDown;

            _flowViewer.PreviewKeyDown += new KeyEventHandler(_flowViewer_PreviewKeyDown);

            _chatStateNotificator.StateChanged += _chatStateNotificator_StateChanged;
            _contactChat.Messages.CollectionChanged += Messages_CollectionChanged;

            _inlineMethod.Finished += _inlineMethod_Finished;
            _inlineSearch.TextChanged += _inlineSearch_TextChanged;
            _inlineSearch.Closed += _inlineSearch_Closed;

            Unloaded += Conversation_Unloaded;

            ScrollToBottom(true);
        }

        void _flowViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // all from the viewer
            if (_inlineSearch != null)
            {
                _inlineSearch.SendKey(e.Key);
            }
        }

        void _text_Loaded(object sender, RoutedEventArgs e)
        {
            _inlineSearch.Visibility = Visibility.Collapsed;
            _text.Focus();
        }

        private void _inlineMethod_Finished(object result)
        {
            Message message = (Message)result;
            SelectItem(message);
        }

        private void _inlineSearch_Closed(bool isEnter)
        {
            lock (_textsLock)
            {
                _texts = null;
                CleanSelection();
            }
        }

        private void _inlineSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _inlineMethod.Go(new InlineParam(SearchInList, _inlineSearch.Text));
        }

        private object SearchInList(ref bool stop, object param)
        {
            lock (_textsLock)
            {
                if (_texts == null)
                {
                    _texts = new List<KeyValuePair<string, Message>>();

                    lock (_contactChat.Messages._syncObject)
                    {
                        foreach (Message mucMessage in _contactChat.Messages)
                        {
                            if (!string.IsNullOrEmpty(mucMessage.Body))
                            {
                                _texts.Add(
                                    new KeyValuePair<string, Message>(mucMessage.Body.ToUpper().Trim(), mucMessage));
                            }
                        }
                    }
                }
            }

            Message found = null;

            _textToSearch = (string)param;

            string toFound = ((string)param).ToUpper();

            bool searchNext = (_lastSearch == toFound);

            _lastSearch = toFound;

            if (searchNext && _lastFoundItem != null)
            {
                bool fromHere = false;

                foreach (KeyValuePair<string, Message> body in _texts)
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
                foreach (KeyValuePair<string, Message> body in _texts)
                {
                    if (stop)
                    {
                        return null;
                    }

                    if (((string)param) == String.Empty)
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

        private void SelectItem(Message item)
        {
            if (App.Current.Dispatcher.CheckAccess())
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
            _previousTextRanges = ContactChat.SelectText(paragraph, text);
        }

        private void CleanSelection()
        {
            foreach (TextRange range in _previousTextRanges)
            {
                range.ApplyPropertyValue(Inline.BackgroundProperty, null);
            }

            _previousTextRanges.Clear();
        }

        void ScrollToBottom(bool force)
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = (ScrollViewer)_flowViewer.Template.FindName("PART_ContentHost", _flowViewer);
            }

            if ( force || (_scrollViewer.VerticalOffset >= _scrollViewer.ScrollableHeight - 15.0))
            {
                _scrollViewer.ScrollToBottom();
            }
        }

        void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ScrollToBottom(false);
        }

        private void Conversation_Unloaded(object sender, RoutedEventArgs e)
        {
            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.gone);

            _chatStateNotificator.StateChanged -= _chatStateNotificator_StateChanged;
            _contactChat.Messages.CollectionChanged -= Messages_CollectionChanged;
        }

        private void _chatStateNotificator_StateChanged(agsXMPP.protocol.extensions.chatstates.Chatstate chatstate)
        {
            _contactChat.SendChatState(chatstate);
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return &&
                (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                OnSendMessage(sender, e);
            }
        }

        protected void OnSendMessage(object sender, RoutedEventArgs eventArgs)
        {
            _contactChat.SendMessage(_text.Text);
            _text.Text = string.Empty;

            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.inactive);

            _text.Focus();
        }

        void OnTypingUnchecked(object sender, RoutedEventArgs e)
        {
            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.inactive);
            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.gone);
        }

        void OnTypingChecked(object sender, RoutedEventArgs e)
        {
            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.active);
        }

        private void _text_TextChanged(object sender, TextChangedEventArgs e)
        {
            _chatStateNotificator.ChangeChatState(agsXMPP.protocol.extensions.chatstates.Chatstate.composing);
        }

    }
}