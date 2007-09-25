using System;
using System.Windows;

namespace xeus2.xeus.UI
{
    public class BaseWindow : Window
    {
        private readonly string _key;

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

        public void ShowHide()
        {
            if (WindowState == WindowState.Minimized)
            {
                if (!ShowInTaskbar)
                {
                    Show();
                }

                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Minimized;

                if (!ShowInTaskbar)
                {
                    Hide();
                }
            }
        }
    }
}