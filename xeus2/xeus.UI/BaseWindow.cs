using System;
using System.Collections.Generic;
using System.Windows;

namespace xeus2.xeus.UI
{
    public class BaseWindow : Window
    {
        private static readonly object _lock = new object();
        private static readonly Dictionary<string, BaseWindow> _windows = new Dictionary<string, BaseWindow>();
        private readonly string _key;

        public BaseWindow()
        {
            throw new NotImplementedException();
        }

        public static void CloseAllWindows()
        {
            BaseWindow[] baseWindows;

            lock (_lock)
            {
                baseWindows = new BaseWindow[_windows.Values.Count];
                _windows.Values.CopyTo(baseWindows, 0);
            }

            foreach (BaseWindow window in baseWindows)
            {
                window.Close();
            }
        }

        public BaseWindow(string keyBase, string key)
        {
            _key = string.Format("/{0}/{1}", keyBase, key);

            BaseWindow exisitngWindow = Find(_key);

            if (exisitngWindow != null)
            {
                throw new WindowExistsException(exisitngWindow);
            }

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
            lock (_lock)
            {
                _windows.Add(_key, this);
            }

            Initialized -= BaseWindow_Initialized;

            Closed += BaseWindow_Closed;
        }

        private void BaseWindow_Closed(object sender, EventArgs e)
        {
            lock (_lock)
            {
                _windows.Remove(_key);
            }

            Closed -= BaseWindow_Closed;
        }

        public BaseWindow Find(string key)
        {
            BaseWindow baseWindow;

            lock (_lock)
            {
                _windows.TryGetValue(key, out baseWindow);
            }

            return baseWindow;
        }
    }
}