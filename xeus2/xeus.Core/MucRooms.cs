using agsXMPP.protocol.x.data;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    internal class MucRooms : ObservableCollectionDisp<Service>
    {
        public void AddMuc(Service service)
        {
            Data xData = ElementUtil.GetData(service.DiscoInfo);

            if (xData != null)
            {
                service.XData = xData;
            }

            base.Add(service);
        }
    }
}