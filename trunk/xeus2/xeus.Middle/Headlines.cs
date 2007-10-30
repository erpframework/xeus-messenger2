using xeus2.Properties;
using xeus2.xeus.Core;
using xeus2.xeus.Data;
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
                HeadlinesChat headlinesChat = new HeadlinesChat();

                foreach (HeadlineMessage message in Database.GetHeadlines(Settings.Default.UI_MaxHistoryMessages))
                {
                    headlinesChat.Messages.Add(message);
                }

                headlinesWindow.DataContext = headlinesChat;
                headlinesWindow.Show();
                headlinesWindow.Activate();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }

            Notification.DismissNotificationType(typeof(EventHeadlineMessage));
        }

        public void DisplayHeadlines()
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(HeadlinesOpen));
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback();

        #endregion
    }
}