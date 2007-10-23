using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.filetransfer;

namespace xeus2.xeus.Core
{
    internal class EventInfoFileTransfer : Event
    {
        private readonly IQ _iq;
        private readonly IContact _contact;
        private readonly File _file;

        public EventInfoFileTransfer(IQ iq)
            : base(string.Empty, EventSeverity.Info)
        {
            _iq = iq;

            agsXMPP.protocol.extensions.si.SI si = iq.SelectSingleElement(typeof(agsXMPP.protocol.extensions.si.SI)) as agsXMPP.protocol.extensions.si.SI;

            if (si != null)
            {
                _file = si.File;

                _contact = Roster.Instance.FindContactOrGetNew(iq.From);
            }

            _message = string.Format("Incoming file '{0}' from {1}", FileName, Contact.DisplayName);
        }

        public long FileLength
        {
            get
            {
                if (_file == null)
                {
                    return 0;
                }
                else
                {
                    return _file.Size;
                }
            }
        }

        public string FileName
        {
            get
            {
                if (_file == null)
                {
                    return string.Empty;
                }
                else
                {
                    return _file.Name;
                }
            }
        }

        public IQ Iq
        {
            get
            {
                return _iq;
            }
        }

        public IContact Contact
        {
            get
            {
                return _contact;
            }
        }
    }
}
