using System.Windows;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for AskAuthorization.xaml
    /// </summary>
    public partial class AskAuthorization : Window
    {
        public AskAuthorization(Contact contact)
        {
            InitializeComponent();

            DataContext = contact;
            _contact.Content = contact;
        }

        void Refuse()
        {
            IContact contact = (IContact) DataContext;

            Account.Instance.GetPresenceManager().RefuseSubscriptionRequest(contact.Jid);

            EventInfo eventinfo =
                new EventInfo(string.Format("'{0}' - authorization denied", contact.Jid));
            Events.Instance.OnEvent(this, eventinfo);
        }

        void Approve()
        {
            IContact contact = (IContact)DataContext;

            Account.Instance.GetPresenceManager().ApproveSubscriptionRequest(contact.Jid);

            EventInfo eventinfo =
                new EventInfo(string.Format("'{0} ({1})' is now authorized", contact.DisplayName, contact.Jid));
            Events.Instance.OnEvent(this, eventinfo);
            
        }
    }
}