namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for HeadlinesWindow.xaml
    /// </summary>
    public partial class HeadlinesWindow : BaseWindow
    {
        public const string _keyBase = "Headlines";

        internal HeadlinesWindow() : base(_keyBase, string.Empty)
        {
            InitializeComponent();
        }
    }
}