using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;

namespace xeus2.xeus.Core
{
    internal class EventErrorRegistration : Event
    {
        private readonly string _error = "Unknown registration error";

        public EventErrorRegistration(Element e)
            : base(string.Empty, EventSeverity.Error)
        {
            if (e != null && e is IQ)
            {
                IQ iq = (IQ) e;

                if (iq.Error != null)
                {
                    switch (iq.Error.Code)
                    {
                        case ErrorCode.Conflict:
                            {
                                _error = "The user name already exists on the server.\nChoose a different one.";
                                break;
                            }
                        case ErrorCode.NotAcceptable:
                            {
                                _error = "Some required information is not provided.";
                                break;
                            }
                    }
                }
            }
        }

        public string Error
        {
            get
            {
                return _error;
            }
        }
    }
}