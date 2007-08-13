using System.Windows;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for Muc.xaml
    /// </summary>
    public partial class Muc : Window
    {
        internal Muc(Service service, string nick, string password)
        {
            InitializeComponent();

            _conversation.MucConversationInit(service, nick, password);

            MucCommands.RegisterCommands(this);
        }
    }
}