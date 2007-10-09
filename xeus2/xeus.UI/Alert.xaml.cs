using System.Windows;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for Alert.xaml
    /// </summary>
    public partial class Alert : Window
    {
        public Alert(string text, string resourceImage)
        {
            InitializeComponent();

            _text.Text = text;
            _image.Fill = StyleManager.GetBrush(resourceImage);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}