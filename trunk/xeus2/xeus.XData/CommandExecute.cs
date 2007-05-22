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

		internal void Setup( ServiceCommandExecution serviceCommandExecution )
		{
			Service = serviceCommandExecution.Service ;
			_command = serviceCommandExecution.Command ;

			_xData = ElementUtil.GetData( _command ) ;

			if ( _xData == null )
			{
				_title.Text = Service.Name ;
				_instructions.Visibility = System.Windows.Visibility.Visible ;

				ClearXForm();

				if ( !string.IsNullOrEmpty( _command.Node ) )
				{
					_instructions.Text = _command.Note.Value ;
				}
				else
				{
					_instructions.Text = Properties.Resources.Constant_IncompleteData ;
				}
			}
			else
			{
				SetupXData( _xData ) ;
			}
		}
	}
}