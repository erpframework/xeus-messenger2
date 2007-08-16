using System.ComponentModel;
using System.Windows;
using xeus2.xeus.Core;

namespace xeus2.xeus.Middle
{
    internal class MucOptions : WindowManager<MucRoom, UI.MucOptions>
    {
        private delegate void DisplayCallback(MucRoom mucRoom);

        private static MucOptions _instance = new MucOptions();

        public static MucOptions Instance
        {
            get
            {
                return _instance;
            }
        }

        protected void MucOptionsOpen(MucRoom mucRoom)
        {
            UI.MucOptions mucOptions = GetWindow(mucRoom);

            if (mucOptions == null)
            {
                mucOptions = new UI.MucOptions(mucRoom);
                mucOptions.Closing += new CancelEventHandler(registration_Closing);
                mucOptions.DataContext = mucRoom;
                AddWindow(mucRoom, mucOptions);
            }

            mucOptions.Show();
        }

        private void registration_Closing(object sender, CancelEventArgs e)
        {
            RemoveWindow(((Window) sender).DataContext as MucRoom);
            ((Window) sender).Closing -= registration_Closing;
        }

        public void DisplayMucOptions(MucRoom mucRoom)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(MucOptionsOpen), mucRoom);
        }
    }
}