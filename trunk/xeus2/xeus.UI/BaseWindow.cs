using System;
using System.Collections.Generic;
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

        public BaseWindow(string keyBase, string key)
        {
            _key = WindowManager.MakeKey(keyBase, key);

            WindowManager.Approve(_key);

            Initialized += BaseWindow_Initialized;
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
    }
}