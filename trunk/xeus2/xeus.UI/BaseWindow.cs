using System;
using System.Windows;
using xeus2.xeus.Commands;
using System.Timers;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace xeus2.xeus.UI
{
    public class BaseWindow : Window
    {
        [DllImport("user32.dll", PreserveSig = false)]
        private static extern int FlashWindow(IntPtr hwnd, bool invert);

        private readonly string _key;

        private bool _hideOnMimnimize = false;

        public BaseWindow()
        {
            throw new NotImplementedException();
        }

        IntPtr _windowPtr;

        public BaseWindow(string key)
        {
            _key = key;

            WindowManager.Approve(_key);

            Initialized += BaseWindow_Initialized;

            Loaded += new RoutedEventHandler(BaseWindow_Loaded);
        }

        void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _windowPtr = new WindowInteropHelper(this).Handle;
        }

        public BaseWindow(string keyBase, string key) : this(WindowManager.MakeKey(keyBase, key))
        {
        }

        public void FlashUntilActivated()
        {
            FlashWindow(_windowPtr, true);
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
            MucCommands.RegisterCommands(this);
            AccountCommands.RegisterCommands(this.CommandBindings);
            GeneralCommands.RegisterCommands(this);
            ContactCommands.RegisterCommands(this);
            RosterCommands.RegisterCommands(this);
            ServiceCommands.RegisterCommands(this);

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

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            FlashWindow(_windowPtr, false);
        }
    }
}