using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;

namespace xeus2.xeus.Core
{
    internal class EventErrorRegistration : Event
    {
        public EventErrorRegistration(Element e)
            : base("Unknown registration error", EventSeverity.Error)
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
                                _message = "The user name already exists on the server.\nChoose a different one.";
                                break;
                            }
                        case ErrorCode.NotAcceptable:
                            {
                                _message = "Some required information is not provided.";
                                break;
                            }
                    }
                }
            }
        }
    }
}