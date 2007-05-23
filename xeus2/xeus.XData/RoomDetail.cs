using xeus2.xeus.Core ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.XData
{
	public class RoomDetail : XDataFormBase
	{
		public RoomDetail()
		{
			InitializeComponent() ;
		}

		internal void Setup( Service service )
		{
			Service = service ;

			_xData = ElementUtil.GetData( service.DiscoInfo ) ;

			if ( _xData != null )
			{
				SetupXData( _xData ) ;
			}
		}
	}
}