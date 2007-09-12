using System.Windows;
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