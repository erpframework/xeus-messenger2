using System.Collections.Generic;
using System.Collections.Specialized;
using agsXMPP.protocol.extensions.bookmarks;

namespace xeus2.xeus.Core
{
    internal class MucMarks : ObservableCollectionDisp<MucMark>
    {
        private static readonly MucMarks _instance = new MucMarks();

        private readonly Dictionary<string, MucMark> _mucMarks = new Dictionary<string, MucMark>();

        public static MucMarks Instance
        {
            get
            {
                return _instance;
            }
        }

        public void AddBookmark(Service service)
        {
            if (!service.IsMucMarked)
            {
                Add(new MucMark(service));

                service.IsMucMarked = true;
            }
        }

        public void AddBookmark(Conference conference)
        {
            lock (Services.Instance._syncObject)
            {
                Service service = Services.Instance.FindService(conference.Jid);
                
                if (service != null)
                {
                    service.IsMucMarked = true;
                }
            }

            Add(new MucMark(conference));
        }

        public void AddBookmark(MucRoom mucRoom)
        {
            AddBookmark(mucRoom.Service);
        }

        public void DeleteBookmark(MucMark mark)
        {
            lock (_syncObject)
            {
                Remove(mark);

                lock (Services.Instance._syncObject)
                {
                    Service service = Services.Instance.FindService(mark.Jid);
                    if (service != null)
                    {
                        service.IsMucMarked = false;
                    }
                }
            }

            Account.Instance.MucMarkManager.SaveBookmarks();
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (MucMark mucMark in e.NewItems)
                        {
                            _mucMarks.Add(mucMark.Jid.ToString(), mucMark);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        _mucMarks.Clear();
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (MucMark mucMark in e.OldItems)
                        {
                            _mucMarks.Remove(mucMark.Jid.ToString());
                        }

                        break;
                    }
            }
        }

        public bool IsBookmarked(Service service)
        {
            lock (_syncObject)
            {
                MucMark mucMark;
                return _mucMarks.TryGetValue(service.Jid.Bare, out mucMark);
            }
        }

        public MucMark Find(string bare)
        {
            MucMark mucMark;

            lock (_syncObject)
            {
                _mucMarks.TryGetValue(bare, out mucMark);
            }

            return mucMark;
        }

        public MucMark Find(Service service)
        {
            return Find(service.Jid.Bare);
        }
    }
}