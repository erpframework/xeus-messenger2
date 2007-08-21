using System.Windows;
using agsXMPP.protocol.x.muc;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for MucOptions.xaml
    /// </summary>
    public partial class MucOptions : Window
    {
        private MucRoom _mucRoom;

        internal MucOptions(MucRoom mucRoom)
        {
            InitializeComponent();

            _mucRoom = mucRoom;

            _muc.Setup(mucRoom);

            MucCommands.RegisterCommands(this);

            _affOwner.AffContacts.SetupAffiliations(mucRoom, Affiliation.owner);
            _affOwner.AffContacts.OnChange += new MucAffContacts.EventChangeCallback(OnChange);

            _affAdmin.AffContacts.SetupAffiliations(mucRoom, Affiliation.admin);
            _affAdmin.AffContacts.OnChange += new MucAffContacts.EventChangeCallback(OnChange);

            _affBanned.AffContacts.SetupAffiliations(mucRoom, Affiliation.outcast);
            _affBanned.AffContacts.OnChange += new MucAffContacts.EventChangeCallback(OnChange);

            _affMembers.AffContacts.SetupAffiliations(mucRoom, Affiliation.member);
            _affMembers.AffContacts.OnChange += new MucAffContacts.EventChangeCallback(OnChange);
        }

        void OnChange(object sender, MucAffContact mucAffContact)
        {
            _affOwner.AffContacts.Remove(mucAffContact);
            _affAdmin.AffContacts.Remove(mucAffContact);
            _affBanned.AffContacts.Remove(mucAffContact);
            _affMembers.AffContacts.Remove(mucAffContact);
        }

        private void OnSaveConfig(object sender, RoutedEventArgs args)
        {
            _muc.Save();
        }

        private void OnResetConfig(object sender, RoutedEventArgs args)
        {
            _muc.Reset();
        }
    }
}