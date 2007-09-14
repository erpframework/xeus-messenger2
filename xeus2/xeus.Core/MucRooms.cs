using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    internal class MucRooms : ObservableCollectionDisp<Service>
    {
        public void AddMuc(Service service)
        {
            agsXMPP.protocol.x.data.Data xData = ElementUtil.GetData(service.DiscoInfo);

            if (xData != null)
            {
                service.XData = xData;
            }

            Add(service);
        }
    }
}