using xeus2.xeus.Commands;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for AddContact.xaml
    /// </summary>
    public partial class AddContact : BaseWindow
    {
        public const string _keyBase = "AddContact";

        public AddContact() : base(_keyBase, string.Empty)
        {
            AccountCommands.RegisterCommands(CommandBindings);

            InitializeComponent();
        }
    }
}