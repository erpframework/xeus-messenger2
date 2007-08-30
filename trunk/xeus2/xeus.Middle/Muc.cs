using System;
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
                GetMucWindow();
                _muc.Show();

                return _muc._multi;
            }
        }

        public void ShrinkMainWindow(double points)
        {
            lock (_lock)
            {
                if (_muc != null)
                {
                    _muc.Width += points;
                }
            }
        }

        void GetMucWindow()
        {
            lock (_lock)
            {
                if (_muc == null || !_muc.IsVisible)
                {
                    _muc = new UI.Muc();
                    _muc.Closed += new EventHandler(_muc_Closed);
                }
            }           
        }

        void _muc_Closed(object sender, EventArgs e)
        {
            lock (_lock)
            {
                _muc.Closed -= new EventHandler(_muc_Closed);
                _muc = null;
            }
        }

        public void DisplayMuc(Service service, string nick, string password)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new MucLoginCallback(DisplayMucInternal), service, nick, password);
        }

        protected void DisplayMucInternal(Service service, string nick, string password)
        {
            GetMucWindow();

            _muc.AddMuc(service, nick, password);
            _muc.Show();
        }
    }
}