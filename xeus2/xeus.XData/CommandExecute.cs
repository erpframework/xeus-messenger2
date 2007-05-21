using agsXMPP.protocol.extensions.commands ;
using xeus2.xeus.Core ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.XData
{
	internal class CommandExecute : XDataFormBase
	{
		private Command _command = null ;

		public Command Command
		{
			get
			{
				return _command ;
			}
		}

		internal void Setup( Command command, Service service )
		{
			Service = service ;
			_command = command ;

			_xData = ElementUtil.GetData( _command ) ;

			if ( _xData == null )
			{
			}
			else
			{
				SetupXData( _xData ) ;
			}
		}
	}
}