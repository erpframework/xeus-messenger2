using System;
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

            Closed += new System.EventHandler(HeadlinesWindow_Closed);
        }

        void HeadlinesWindow_Closed(object sender, System.EventArgs e)
        {
            Closed -= new System.EventHandler(HeadlinesWindow_Closed);

            IDisposable disposable = DataContext as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}