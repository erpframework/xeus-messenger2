using System.Windows;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for SingleValueBox.xaml
    /// </summary>
    public partial class SingleValueBox : Window
    {
        public SingleValueBox(string text, string buttonText)
        {
            InitializeComponent();

            Name = text;
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