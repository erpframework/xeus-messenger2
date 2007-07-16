using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using xeus2.Properties;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for InfoPopup.xaml
    /// </summary>
    public partial class InfoPopup
    {
        private Timer _timer = new Timer(Settings.Default.UI_ErrorDismiss);

        private delegate void CloseCallback();


        public InfoPopup()
        {
            InitializeComponent();

            Style = StyleManager.GetStyle("InfoTip");

            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);

            MouseLeave += new MouseEventHandler(InfoPopup_MouseLeave);
            MouseEnter += new MouseEventHandler(InfoPopup_MouseEnter);

            Rect rect = new Rect(0, 0, ActualWidth, ActualHeight);
            Rect mouseRect = new Rect(new Point(0.0, 0.0), Mouse.GetPosition(this));

            if (rect.IntersectsWith(mouseRect))
            {
                _timer.Start();
            }
        }

        private void InfoPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            _timer.Stop();
        }

        private void InfoPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new CloseCallback(Close));
        }

        protected void Close()
        {
            IsOpen = false;
        }
    }
}