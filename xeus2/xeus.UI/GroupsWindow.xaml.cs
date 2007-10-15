using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for GroupsWindow.xaml
    /// </summary>
    public partial class GroupsWindow : BaseWindow
    {
        public const string _keyBase = "GroupsWindow";

        public GroupsWindow(IContact contact) : base(_keyBase, string.Empty)
        {
            DataContext = contact;

            InitializeComponent();
        }
    }
}