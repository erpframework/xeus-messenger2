using System.Collections.Generic;
using System.Collections.Specialized;
using agsXMPP;
using xeus.Data;

namespace xeus2.xeus.Core
{
    internal class MucMarks : ObservableCollectionDisp<MucMark>
    {
        private static MucMarks _instance = new MucMarks();

        private Dictionary<string, MucMark> _mucMarks = new Dictionary<string, MucMark>();

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

        public void AddBookmark(MucRoom mucRoom)
        {
            AddBookmark(mucRoom.Service);
        }

        public void DeleteBookmark(MucMark mark)
        {
            lock (_syncObject)
            {
                Database.DeleteMucMark(mark);
                Remove(mark);

                lock (Services.Instance._syncObject)
                {
                    Service service = Services.Instance.FindService(new Jid(mark.Jid));
                    if (service != null)
                    {
                        service.IsMucMarked = false;
                    }
                }
            }
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
                            _mucMarks.Add(mucMark.Jid, mucMark);
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
                            _mucMarks.Remove(mucMark.Jid);
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
    }
}