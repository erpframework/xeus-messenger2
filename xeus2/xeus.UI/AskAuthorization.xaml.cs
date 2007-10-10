using System.Windows;
using agsXMPP;
using agsXMPP.protocol.client;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for AskAuthorization.xaml
    /// </summary>
    public partial class AskAuthorization : BaseWindow
    {
        public const string _keyBase = "AskAuthorization";

        public AskAuthorization(Contact contact) : base(_keyBase, contact.Jid.Bare)
        {
            InitializeComponent();

            DataContext = contact;
            _contact.Content = contact;
        }

        public AskAuthorization(Presence presence)
            : base(_keyBase, presence.From.Bare)
        {
            InitializeComponent();

            Contact contact = new Contact(presence);

            DataContext = contact;
            _contact.Content = contact;
        }

        void OnRefuse(object sender, RoutedEventArgs args)
        {
            IContact contact = (IContact)DataContext;

            Roster.Instance.AuthorizeContact(contact, false);

            Close();
        }

        void OnApprove(object sender, RoutedEventArgs args)
        {
            IContact contact = (IContact)DataContext;

            Roster.Instance.AuthorizeContact(contact, true);

            Close();
        }
    }
}