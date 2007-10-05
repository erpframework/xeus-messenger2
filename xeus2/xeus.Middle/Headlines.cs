using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class Headlines
    {
        private static readonly Headlines _instance = new Headlines();

        public static Headlines Instance
        {
            get
            {
                return _instance;
            }
        }

        protected void HeadlinesOpen()
        {
            try
            {
                HeadlinesWindow headlinesWindow = new HeadlinesWindow();
                headlinesWindow.DataContext = new HeadlinesChat();
                headlinesWindow.Show();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        public void DisplayMucOptions()
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(HeadlinesOpen));
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback();

        #endregion
    }
}