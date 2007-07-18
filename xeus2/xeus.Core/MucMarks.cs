using System;
using System.Collections.Generic;
using System.Text;
using xeus.Data;

namespace xeus2.xeus.Core
{
    internal class MucMarks : ObservableCollectionDisp<MucMark>
    {
        private static MucMarks _instance = new MucMarks();

        public static MucMarks Instance
        {
            get
            {
                return _instance;
            }
        }

        public void AddBookmark(Service service)
        {
            lock (_syncObject)
            {
                foreach (MucMark mark in this)
                {
                    if (mark.Jid == service.Jid.Bare)
                    {
                        return;
                    }
                }
                Add(new MucMark(service));
            }
        }

        public void AddBookmark(MucRoom mucRoom)
        {
            lock (_syncObject)
            {
                AddBookmark(mucRoom.Service);
            }
        }

        public void AddBookmark(Service service, string nick, string password)
        {
            lock (_syncObject)
            {
                Add(new MucMark(service, nick, password));
            }
        }

        public void DeleteBookmark(MucMark mark)
        {
            lock (_syncObject)
            {
                Database.DeleteMucMark(mark);
                Remove(mark);
            }
        }
    }
}
