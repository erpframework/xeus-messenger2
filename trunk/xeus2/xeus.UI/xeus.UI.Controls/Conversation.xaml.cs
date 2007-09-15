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
        private readonly ContactChat _contactChat;
        
        internal Conversation(ContactChat contactChat)
        {
            InitializeComponent();

            DataContext = contactChat;
            _contactChat = contactChat;
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
            _text.Focus();
        }
    }
}