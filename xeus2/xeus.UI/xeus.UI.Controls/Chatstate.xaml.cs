using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for Chatstate.xaml
    /// </summary>
    public partial class Chatstate : UserControl
    {
        private IChatState _chatBase;

        public Chatstate()
        {
            InitializeComponent();

            Loaded += Chatstate_Loaded;
        }

        private void Chatstate_Loaded(object sender, RoutedEventArgs e)
        {
            Unloaded += Chatstate_Unloaded;

            _chatBase = (IChatState)DataContext;
            _chatBase.PropertyChanged += _contactChat_PropertyChanged;

            HandleChatState();
        }

        void Chatstate_Unloaded(object sender, RoutedEventArgs e)
        {
            _chatBase.PropertyChanged -= _contactChat_PropertyChanged;
        }

        void HandleChatState()
        {
            switch (_chatBase.ChatState)
            {
                case agsXMPP.protocol.extensions.chatstates.Chatstate.active:
                case agsXMPP.protocol.extensions.chatstates.Chatstate.paused:
                    {
                        Opacity = 0.15;
                        Visibility = Visibility.Visible;
                        break;
                    }
                case agsXMPP.protocol.extensions.chatstates.Chatstate.composing:
                    {
                        Opacity = 1.0;
                        Visibility = Visibility.Visible;
                        break;
                    }
                default:
                    {
                        Visibility = Visibility.Collapsed;
                        break;
                    }
            }
        }

        private void _contactChat_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ChatState")
            {
                HandleChatState();
            }
        }
    }
}