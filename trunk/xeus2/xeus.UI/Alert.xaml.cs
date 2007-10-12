using System;
using System.Windows;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for Alert.xaml
    /// </summary>
    public partial class Alert : BaseWindow
    {
        [Flags]
        public enum Buttons
        {
            Ok = 1,
            Yes = 2,
            No = 4,
            Cancel = 8
        }

        private Buttons _return = Buttons.Cancel;

        public const string _keyBase = "AlertWindow";

        public Alert(string text, string resourceImage, Buttons buttonsToDisplay)
            : base(_keyBase, string.Empty)
        {
            InitializeComponent();

            _text.Text = text;
            _image.Fill = StyleManager.GetBrush(resourceImage);

            if ((buttonsToDisplay & Buttons.Ok) != Buttons.Ok)
            {
                _ok.Visibility = Visibility.Collapsed;
            }

            if ((Buttons.Yes & buttonsToDisplay) != Buttons.Yes)
            {
                _yes.Visibility = Visibility.Collapsed;
            }

            if ((Buttons.No & buttonsToDisplay) != Buttons.No)
            {
                _no.Visibility = Visibility.Collapsed;
            }

            if ((Buttons.Cancel & buttonsToDisplay) != Buttons.Cancel)
            {
                _cancel.Visibility = Visibility.Collapsed;
            }
        }

        public Buttons Return
        {
            get
            {
                return _return;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == _ok)
            {
                _return = Buttons.Ok;
            }
            else if (sender == _yes)
            {
                _return = Buttons.Yes;
            }
            else if (sender == _no)
            {
                _return = Buttons.No;
            }
            else if (sender == _cancel)
            {
                _return = Buttons.Cancel;
            }

            Close();
        }
    }
}