using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class MucOptions
    {
        private static readonly MucOptions _instance = new MucOptions();

        public static MucOptions Instance
        {
            get
            {
                return _instance;
            }
        }

        protected void MucOptionsOpen(MucRoom mucRoom)
        {
            try
            {
                UI.MucOptions mucOptions = new UI.MucOptions(mucRoom);
                mucOptions.Show();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        public void DisplayMucOptions(MucRoom mucRoom)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(MucOptionsOpen), mucRoom);
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback(MucRoom mucRoom);

        #endregion
    }
}