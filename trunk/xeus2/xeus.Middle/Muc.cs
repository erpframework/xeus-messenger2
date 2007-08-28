using xeus2.xeus.Core;
using xeus2.xeus.UI.xeus.UI.Controls;

namespace xeus2.xeus.Middle
{
    internal class Muc : IMultiWinContainerProvider
    {
        private static Muc _instance = new Muc();
        private object _lock = new object();

        private UI.Muc _muc = null;

        private delegate void MucLoginCallback(Service servie, string nick, string password);

        public static Muc Instance
        {
            get
            {
                return _instance;
            }
        }

        public MultiTabControl MultiTabControl
        {
            get
            {
                lock (_lock)
                {
                    if (_muc == null || !_muc.IsVisible)
                    {
                        _muc = new UI.Muc();
                        _muc.Show();
                    }
                }

                return _muc._multi;
            }
        }

        public void DisplayMuc(Service service, string nick, string password)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new MucLoginCallback(DisplayMucInternal), service, nick, password);
        }

        protected void DisplayMucInternal(Service service, string nick, string password)
        {
            lock (_lock)
            {
                if (_muc == null || !_muc.IsVisible)
                {
                    _muc = new UI.Muc();
                }
            }

            _muc.AddMuc(service, nick, password);
            _muc.Show();
        }
    }
}