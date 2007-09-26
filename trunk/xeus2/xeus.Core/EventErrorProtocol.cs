using agsXMPP.Xml.Dom;

namespace xeus2.xeus.Core
{
    public class EventErrorProtocol : Event
    {
        private readonly Element _element;

        public EventErrorProtocol(string message, Element element)
            : base(message, EventSeverity.Error)
        {
            _element = element;
        }

        public Element Element
        {
            get
            {
                return _element;
            }
        }
    }
}