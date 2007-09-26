using xeus2.xeus.Core;

namespace xeus2.xeus.Middle
{
    internal interface INotification
    {
        void RefreshStatus();
        void ItemAdded(Event @event);
    }
}