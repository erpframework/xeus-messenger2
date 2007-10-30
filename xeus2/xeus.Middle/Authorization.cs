using agsXMPP;
using agsXMPP.protocol.client;
using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class Authorization
    {
        private static readonly Authorization _instance = new Authorization();

        public static Authorization Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Show(Contact contact)
        {
            try
            {
                AskAuthorization authorization = new AskAuthorization(contact);

                authorization.Show();
                authorization.Activate();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        private void ShowPresence(Presence presence)
        {
            try
            {
                AskAuthorization authorization = new AskAuthorization(presence);

                authorization.Show();
                authorization.Activate();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        public void Ask(Contact contact)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new ShowCallback(Show), contact);
        }

        public void Ask(Presence presence)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new ShowPresenceCallback(ShowPresence), presence);
        }

        #region Nested type: ShowCallback

        private delegate void ShowCallback(Contact contact);
        private delegate void ShowPresenceCallback(Presence presence);

        #endregion
    }
}