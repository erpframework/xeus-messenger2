using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class MucMessages : ObservableCollectionDisp< MucMessage >
	{
        public void OnMessage(agsXMPP.protocol.client.Message message, MucContact sender)
		{
			Add( new MucMessage( message, sender ) ) ;
		}
	}
}