using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class ErrorPopup
    {
        private static readonly ErrorPopup _instance = new ErrorPopup();

        private ErrorPopup()
        {
            Events.Instance.OnEventRaised += Instance_OnEventRaised;
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
                InfoPopup errorInfo = new InfoPopup();
                errorInfo.Content = myEvent ;
                errorInfo.IsOpen = true;
            }
        }

        public void Init()
        {
            
        }
    }
}