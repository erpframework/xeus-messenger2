using xeus2.xeus.Commands;
using xeus2.xeus.Core;
using xeus2.xeus.UI.xeus.UI.Controls;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : BaseWindow
    {
        public const string _keyBase = "Chat";

        internal Chat() : base(_keyBase, string.Empty)
        {
            InitializeComponent();

            GeneralCommands.RegisterCommands(this);

            _multi.MultiWinContainerProvider = Middle.Chat.Instance;
        }

        public MultiTabControl TabControl
        {
            get
            {
                return _multi;
            }
        }

        internal bool AddChat(IContact contact)
        {
            ContactChat contactChat = Roster.Instance.CreateChat(contact);
            Conversation conversation = new Conversation(contactChat);

            try
            {
                MultiWin multiWin = new MultiWin(conversation, _keyBase, contact.Jid.ToString());

                _multi.MultiWindows.Add(new MultiTabItem(contact.DisplayName, multiWin));
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();

                return false;
            }

            return true;
        }
    }
}