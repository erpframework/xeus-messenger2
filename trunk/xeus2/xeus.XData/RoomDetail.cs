using System.Windows.Media;
using xeus2.xeus.Core ;
using xeus2.xeus.UI;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.XData
{
	public class RoomDetail : XDataFormBase
	{
		public RoomDetail()
		{
			InitializeComponent() ;
		}

	    public override DrawingBrush Icon
	    {
            get { return StyleManager.GetBrush("mucinfo_design"); }
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