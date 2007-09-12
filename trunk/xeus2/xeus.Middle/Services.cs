using xeus2.xeus.Commands;
using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class Services
    {
        private static readonly Services _instance = new Services();

        public static Services Instance
        {
            get
            {
                return _instance;
            }
        }

        protected void DisplayInternal()
        {
            try
            {
                ServiceWindow serviceWindow = new ServiceWindow();

                if (Core.Services.Instance.Count == 0)
                {
                    ServiceCommands.DiscoveryServices.Execute(string.Empty, null);
                }

                serviceWindow.Show();
            }

            catch (WindowExistsException e)
            {
                e.ExistingWindow.Activate();
            }

            if (Core.Services.Instance.Count == 0)
            {
                Account.Instance.Discovery(null);
            }
        }

        public void Display()
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(DisplayInternal));
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback();

        #endregion
    }
}