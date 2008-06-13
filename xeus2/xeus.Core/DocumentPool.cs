using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    public class DocumentPool : Pool<FlowDocument>
    {
        static DocumentPool instance = new DocumentPool();

        public static DocumentPool Instance
        {
            get { return DocumentPool.instance; }
        }
    }
}
