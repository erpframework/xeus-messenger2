using System;
using System.Windows;

namespace xeus2.xeus.UI
{
    public class BaseWindow : Window
    {
        private readonly string _key;

        private bool _hideOnMimnimize = false;

        public BaseWindow()
        {
            throw new NotImplementedException();
        }

        public BaseWindow(string key)
        {
            _key = key;

            WindowManager.Approve(_key);

            Initialized += BaseWindow_Initialized;
        }

        public BaseWindow(string keyBase, string key) : this(WindowManager.MakeKey(keyBase, key))
        {
        }

        public string Key
        {
            get
            {
                return _key;
            }
        }

        public bool HideOnMimnimize
        {
            get
            {
                return _hideOnMimnimize;
            }
            set
            {
                _hideOnMimnimize = value;
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (_suppressStateChange)
            {
                _suppressStateChange = false;
                return;
            }

            if (HideOnMimnimize)
            {
                if (WindowState == WindowState.Minimized)
                {
                    Hide();
                }
                else if (WindowState != WindowState.Minimized)
                {
                    Show();
                    Activate();
                }
            }
        }

        private void BaseWindow_Initialized(object sender, EventArgs e)
        {
            WindowManager.Add(_key, this);

            Initialized -= BaseWindow_Initialized;

            Closed += BaseWindow_Closed;
        }

        private void BaseWindow_Closed(object sender, EventArgs e)
        {
            WindowManager.Remove(_key);

            Closed -= BaseWindow_Closed;
        }

        private bool _suppressStateChange = false;

        public void ShowHide()
        {
            _suppressStateChange = true;

            if (WindowState == WindowState.Minimized)
            {
                if (HideOnMimnimize)
                {
                    Show();
                    Activate();
                }

                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Minimized;

                if (HideOnMimnimize)
                {
                    Hide();
                }
            }
        }
    }
}