using System.ComponentModel;
using System.Windows;
using agsXMPP.protocol.x.muc;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for MucOptions.xaml
    /// </summary>
    public partial class MucOptions : BaseWindow
    {
        public const string _keyBase = "MucOptions";

        private readonly MucRoom _mucRoom;

        internal MucOptions(MucRoom mucRoom) : base(_keyBase, mucRoom.Service.Jid.Bare)
        {
            InitializeComponent();

            _mucRoom = mucRoom;

            DataContext = mucRoom;

            _muc.Setup(mucRoom);

            MucCommands.RegisterCommands(this);

            _affOwner.AffContacts.SetupAffiliations(mucRoom, Affiliation.owner);
            _affOwner.AffContacts.OnChange += OnChange;

            _affAdmin.AffContacts.SetupAffiliations(mucRoom, Affiliation.admin);
            _affAdmin.AffContacts.OnChange += OnChange;

            _affBanned.AffContacts.SetupAffiliations(mucRoom, Affiliation.outcast);
            _affBanned.AffContacts.OnChange += OnChange;

            _affMembers.AffContacts.SetupAffiliations(mucRoom, Affiliation.member);
            _affMembers.AffContacts.OnChange += OnChange;

            _affMembers.Loaded += _affMembers_Loaded;

            _tabForm.IsEnabled = (_mucRoom.Me.Affiliation == Affiliation.owner);
        }

        void _affMembers_Loaded(object sender, RoutedEventArgs e)
        {
            new TextFilterMucAffs(new ICollectionView[] { _affOwner.View, _affAdmin.View, _affBanned.View, _affMembers.View }, _filterAffs);
        }

        void OnChange(object sender, MucAffContact mucAffContact)
        {
            if (mucAffContact != null)
            {
                _affOwner.AffContacts.Remove(mucAffContact);
                _affAdmin.AffContacts.Remove(mucAffContact);
                _affBanned.AffContacts.Remove(mucAffContact);
                _affMembers.AffContacts.Remove(mucAffContact);
            }
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