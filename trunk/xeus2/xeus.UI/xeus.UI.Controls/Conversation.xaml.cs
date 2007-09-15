using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for Conversation.xaml
    /// </summary>
    public partial class Conversation : UserControl
    {
        private readonly ChatStateNotificator _chatStateNotificator = new ChatStateNotificator();
        private readonly ContactChat _contactChat;

        private ScrollViewer _scrollViewer = null;

        internal Conversation(ContactChat contactChat)
        {
            InitializeComponent();

            DataContext = contactChat;
            _contactChat = contactChat;

            Loaded += Conversation_Loaded;
            GotFocus += Conversation_GotFocus;
            LostFocus += Conversation_LostFocus;
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
            _chatStateNotificator.StateChanged += _chatStateNotificator_StateChanged;
            _contactChat.Messages.CollectionChanged += Messages_CollectionChanged;

            Unloaded += Conversation_Unloaded;

            ScrollToBottom(true);
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