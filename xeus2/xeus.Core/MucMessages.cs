using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class MucMessages : ObservableCollectionDisp< MucMessage >
	{
		public void OnMessage( Message message, MucContact sender )
		{
			Add( new MucMessage( message, sender ) ) ;
		}
	}
}