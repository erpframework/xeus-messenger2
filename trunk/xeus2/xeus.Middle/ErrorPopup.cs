using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class ErrorPopup
    {
        private InfoPopup _errorInfo = new InfoPopup();

        private static ErrorPopup _instance = new ErrorPopup();


        private ErrorPopup()
        {
            Events.Instance.OnEventRaised += new Events.EventItemCallback(Instance_OnEventRaised);
        }

        public static ErrorPopup Instance
        {
            get
            {
                return _instance;
            }
        }

        void Instance_OnEventRaised(object sender, Event myEvent)
        {
            if (myEvent.Severity == Event.EventSeverity.Error)
            {
                _errorInfo.Display(myEvent);
            }
        }

        public void Init()
        {
            
        }
    }
}