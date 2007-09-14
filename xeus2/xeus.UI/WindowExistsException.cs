using System;
using System.Windows;
using System.Windows.Controls;
using xeus2.xeus.UI.xeus.UI.Controls;

namespace xeus2.xeus.UI
{
    public class WindowExistsException : ApplicationException
    {
        private readonly ContentControl _ctrl;

        public WindowExistsException(ContentControl ctrl)
        {
            _ctrl = ctrl;
        }

        public ContentControl Control
        {
            get
            {
                return _ctrl;
            }
        }

        public void ActivateControl()
        {
            BaseWindow window = _ctrl as BaseWindow;

            if (window != null)
            {
                window.Activate();
            }
            else
            {
                UserControl ctrl = _ctrl as UserControl;

                if (ctrl != null)
                {
                    Window win = Window.GetWindow(ctrl);

                    if (win != null)
                    {
                        win.Activate();
                    }

                    MultiWin mwin = ctrl as MultiWin;

                    if (mwin != null)
                    {
                        mwin.ShowContainer();
                    }
                }
            }
        }
    }
}