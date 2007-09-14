using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.bookmarks;
using agsXMPP.protocol.iq.@private;
using xeus2.xeus.Core;

namespace xeus2.xeus.Data
{
    internal class MucMarkManager
    {
        private readonly BookmarkManager _bookmarkManager;

        public MucMarkManager(XmppClientConnection xmppClientConnection)
        {
            _bookmarkManager = new BookmarkManager(xmppClientConnection);
        }

        public void LoadMucMarks()
        {
            _bookmarkManager.RequestBookmarks(OnRequestResult);
        }

        private void OnRequestResult(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                EventError eventError = new EventError("Request for bookmarks on server failed", iq.Error);
                Events.Instance.OnEvent(this, eventError);
            }
            else if (iq.Type == IqType.result)
            {
                Private privateData = iq.Query as Private;

                if (privateData != null && privateData.Storage != null)
                {
                    Conference[] conferences = privateData.Storage.GetConferences();

                    lock (MucMarks.Instance._syncObject)
                    {
                        foreach (Conference conference in conferences)
                        {
                            MucMarks.Instance.AddBookmark(conference);
                        }
                    }
                }
            }
        }

        public void SaveBookmarks()
        {
            Conference[] conferences;

            lock (MucMarks.Instance._syncObject)
            {
                conferences = new Conference[MucMarks.Instance.Count];

                int i = 0;
                foreach (MucMark mucMark in MucMarks.Instance)
                {
                    conferences[i++] = new Conference(mucMark.Jid, mucMark.Name, mucMark.Password);
                }
            }

            _bookmarkManager.StoreBookmarks(conferences, OnStoreResult);
        }

        private void OnStoreResult(object sender, IQ iq, object data)
        {
            if (iq.Error != null)
            {
                EventError eventError = new EventError("Saving for bookmarks on server failed", iq.Error);
                Events.Instance.OnEvent(this, eventError);
            }
        }
    }
}