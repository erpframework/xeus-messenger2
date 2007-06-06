using agsXMPP.protocol.extensions.commands ;
using agsXMPP.protocol.x.data ;
using xeus2.xeus.UI ;

namespace xeus2.xeus.Core
{
	internal class ServiceCommandExecution
	{
		private readonly Service _service ;
		private readonly Command _command ;
		private UI.CommandExecute _commandExec ;

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

		public CommandExecute CommandExec
		{
			get
			{
				return _commandExec ;
			}
			set
			{
				_commandExec = value ;
			}
		}

		public Data GetResult()
		{
			return CommandExec.GetResult() ;
		}
	}
}