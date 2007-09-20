using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for VCardDisplay.xaml
    /// </summary>
    public partial class VCardDisplay : BaseWindow
    {
        public const string _keyBase = "VCard";

        public VCardDisplay(IContact contact) : base(_keyBase, contact.Jid.Bare)
        {
            InitializeComponent();

            Roster.Instance.SetFreshVcard(contact, 0);

            DataContext = contact;
        }
    }
}