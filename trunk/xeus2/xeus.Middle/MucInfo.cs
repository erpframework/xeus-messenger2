using agsXMPP;
using agsXMPP.protocol.iq.disco;
using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class MucInfo
    {
        private static readonly MucInfo _instance = new MucInfo();

        public static MucInfo Instance
        {
            get
            {
                return _instance;
            }
        }

        public void DisplayMucInfo(string jidBare)
        {
            Service service;

            Jid jid = new Jid(jidBare);

            lock (Core.Services.Instance._syncObject)
            {
                service = Core.Services.Instance.FindService(jid);
            }

            if (service == null)
            {
                // not on this server
                service = new Service(new DiscoItem(), false);
                service.DiscoItem.Jid = new Jid(jidBare);
            }

            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(DisplayMucInfoInternal), service);
        }

        public void DisplayMucInfo(Service service)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(DisplayMucInfoInternal), service);
        }

        protected void DisplayMucInfoInternal(Service service)
        {
            try
            {
                RoomInfo roomInfo = new RoomInfo(service);
                roomInfo.Show();
                roomInfo.Activate();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        public void MucLogin(Service service, string forceNick)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new MucLoginCallback(MucLoginInternal), service, forceNick);
        }

        public void MucLogin(MucMark mucMark)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new MucLoginmarkCallback(MucLoginInternalMark), mucMark);
        }

        protected static void MucLoginInternalMark(MucMark mucMark)
        {
            try
            {
                RoomLogin roomLogin = new RoomLogin(mucMark, null);

                roomLogin.Show();
                roomLogin.Activate();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        protected static void MucLoginInternal(Service service, string forceNick)
        {
            try
            {
                RoomLogin roomLogin = new RoomLogin(service, forceNick);

                roomLogin.Show();
                roomLogin.Activate();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback(Service service);

        #endregion

        #region Nested type: MucLoginCallback

        private delegate void MucLoginCallback(Service servie, string forceNick);

        #endregion

        #region Nested type: MucLoginmarkCallback

        private delegate void MucLoginmarkCallback(MucMark mucMark);

        #endregion
    }
}