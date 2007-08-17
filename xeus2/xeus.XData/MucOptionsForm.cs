using System.Windows.Media;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.data;
using agsXMPP.protocol.x.muc;
using xeus2.xeus.Core;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.XData
{
    internal class MucOptionsForm : XDataFormBase
    {
        private MucManager _manager;
        private MucRoom _mucRoom;

        private delegate void ResultCallback(object sender, IQ iq);

        public override DrawingBrush Icon
        {
            get
            {
                return (DrawingBrush) FindResource("muc_options_design");
            }
        }

        internal void Setup(MucRoom mucRoom)
        {
            _mucRoom = mucRoom;
            _manager = Account.Instance.GetMucManager();
            _manager.RequestConfigurationForm(mucRoom.Service.Jid, new IqCB(OnResultMucRoomOps));
        }

        private void OnResultMucRoomOps(object sender, IQ iq, object data)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new ResultCallback(OnResultMucRoomOpsInternal), sender, iq);
        }

        private void OnResultMucRoomOpsInternal(object sender, IQ iq)
        {
            if (iq.Error != null)
            {
                Services.Instance.OnServiceItemError(sender, iq);
            }
            else if (iq.Type == IqType.result)
            {
                _xData = ElementUtil.GetData(iq.Query);

                if (_xData != null)
                {
                    SetupXData(_xData);
                }
            }
        }

        public void Save()
        {
            Data data = GetResult();

            Account.Instance.DoSaveMucConfig(_mucRoom, data);
        }

        public void Reset()
        {
            _manager.AcceptDefaultConfiguration(_mucRoom.Service.Jid);
        }
    }
}