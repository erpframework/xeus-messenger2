using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for InfoPopup.xaml
    /// </summary>
    public partial class InfoPopup
    {
        public InfoPopup()
        {
            InitializeComponent();
        }

        internal void Display(Event eventInfo)
        {
            _text.Text = eventInfo.Message;
        }
    }
}