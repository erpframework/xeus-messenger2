namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for Wizard.xaml
    /// </summary>
    public partial class Wizard : BaseWindow
    {
        public const string _keyBase = "Wizard";

        public Wizard() : base(_keyBase, string.Empty)
        {
            InitializeComponent();
        }
    }
}