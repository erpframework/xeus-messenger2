using System.Windows;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for SingleValueBox.xaml
    /// </summary>
    public partial class SingleValueBox : BaseWindow
    {
        public const string _keyBase = "SingleValueBox";

        public SingleValueBox(string text, string buttonText) : base(_keyBase, text)
        {
            InitializeComponent();

            Title = text;
            _ok.Content = buttonText;
        }

        public string Text
        {
            get
            {
                return _text.Text;
            }
        }

        protected void OnOk(object sender, RoutedEventArgs eventArgs)
        {
            DialogResult = true;
            Close();
        }
    }
}