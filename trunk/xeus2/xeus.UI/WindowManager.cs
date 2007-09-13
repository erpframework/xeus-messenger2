using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace xeus2.xeus.UI
{
    internal class WindowManager
    {
        private static readonly object _lock = new object();
        private static readonly Dictionary<string, ContentControl> _windows = new Dictionary<string, ContentControl>();

        public static string MakeKey(string keyBase, string key)
        {
            return string.Format("/{0}/{1}", keyBase, key);
        }

        public static void CloseAllWindows()
        {
            ContentControl[] contentControls;

            lock (_lock)
            {
                contentControls = new ContentControl[_windows.Values.Count];
                _windows.Values.CopyTo(contentControls, 0);
            }

            for (int i = contentControls.Length - 1; i >= 0; i--)
            {
                BaseWindow baseWindow = contentControls[i] as BaseWindow;
                    
                if (baseWindow != null)
                {
                    baseWindow.Close();
                }
            }

            _windows.Clear();
        }

        public static void Add(string key, ContentControl ctrl)
        {
            lock (_lock)
            {
                _windows.Add(key, ctrl);
            }
        }

        public static void Remove(string key)
        {
            lock (_lock)
            {
                _windows.Remove(key);
            }
        }

        public static ContentControl Find(string key)
        {
            ContentControl ctrl;

            lock (_lock)
            {
                _windows.TryGetValue(key, out ctrl);
            }

            return ctrl;
        }

        public static void Approve(string key)
        {
            ContentControl ctrl = Find(key);

            if (ctrl != null)
            {
                throw new WindowExistsException(ctrl);
            }
        }
    }
}
