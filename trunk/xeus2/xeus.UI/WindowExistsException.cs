using System;

namespace xeus2.xeus.UI
{
    public class WindowExistsException : ApplicationException
    {
        private readonly BaseWindow _existingWindow;

        public WindowExistsException(BaseWindow existingWindow)
        {
            _existingWindow = existingWindow;
        }

        public BaseWindow ExistingWindow
        {
            get
            {
                return _existingWindow;
            }
        }
    }
}