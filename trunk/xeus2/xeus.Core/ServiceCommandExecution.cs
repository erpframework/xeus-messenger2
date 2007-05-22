using agsXMPP.protocol.extensions.commands ;

namespace xeus2.xeus.Core
{
	internal class ServiceCommandExecution
	{
		private readonly Service _service ;
		private readonly Command _command ;

		public ServiceCommandExecution( Command command, Service service )
		{
			_command = command ;
			_service = service ;
		}

		public Service Service
		{
			get
			{
				return _service ;
			}
		}

		public Command Command
		{
			get
			{
				return _command ;
			}
		}
	}
}