using System.ComponentModel;
using agsXMPP.protocol.extensions.chatstates;

namespace xeus2.xeus.Core
{
    public interface IChatState : INotifyPropertyChanged
    {
        Chatstate ChatState
        {
            get;
            set;
        }
    }
}