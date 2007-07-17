using System;
using System.Collections.Generic;
using System.Text;

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
            Add(new MucMark(service));
        }

        public void AddBookmark(Service service, string nick, string password)
        {
            Add(new MucMark(service, nick, password));
        }
    }
}
