using xeus2.xeus.Commands;
using xeus2.xeus.Core;
using xeus2.xeus.UI.xeus.UI.Controls;
using xeus2.xeus.Middle;

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

            _multi.MultiWinContainerProvider = Middle.Chat.Instance;

            Activated += new System.EventHandler(Chat_Activated);
            Closed += new System.EventHandler(Chat_Closed);
        }

        void Chat_Closed(object sender, System.EventArgs e)
        {
            Activated -= new System.EventHandler(Chat_Activated);
            Closed -= new System.EventHandler(Chat_Closed);
        }

        void Chat_Activated(object sender, System.EventArgs e)
        {
            foreach (MultiTabItem window in _multi.MultiWindows)
            {
                Notification.DismissChatMessageNotification(((ContactChat)window.Container.ContentElement.DataContext).Contact);
            }

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