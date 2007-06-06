using System.ComponentModel ;
using System.Windows ;
using System.Windows.Threading ;
using agsXMPP.protocol.extensions.commands ;
using xeus2.Properties ;
using xeus2.xeus.Core ;
using xeus2.xeus.UI ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.Middle
{
	internal class CommandExecutor : WindowManager< string, CommandExecute >
	{
		private static CommandExecutor _instance = new CommandExecutor() ;

		private delegate void DisplayCallback( Command command, Service service ) ;

		public static CommandExecutor Instance
		{
			get
			{
				return _instance ;
			}
		}

		protected void DisplayQuestionaireInternal( Command command, Service service )
		{
			CommandExecute commandExecuteWindow = GetWindow( service.Jid.ToString() ) ;

			ServiceCommandExecution serviceCommandExecution = new ServiceCommandExecution( command, service ) ;

			if ( commandExecuteWindow == null )
			{
				commandExecuteWindow = new CommandExecute( serviceCommandExecution ) ;
				commandExecuteWindow.Closing += new CancelEventHandler( commandExecuteWindow_Closing ) ;
				AddWindow( service.Jid.ToString(), commandExecuteWindow ) ;
			}
			else
			{
				commandExecuteWindow.Redisplay( serviceCommandExecution ) ;
			}

			if ( command.Status == Status.canceled )
			{
				EventInfo eventinfo = new EventInfo( string.Format( Resources.Event_CommandCancelled, service.Name ) ) ;
				Events.Instance.OnEvent( this, eventinfo ) ;

				commandExecuteWindow.Close() ;
			}
			else
			{
				commandExecuteWindow.DataContext = serviceCommandExecution ;
				commandExecuteWindow.Show() ;
			}
		}

		private void commandExecuteWindow_Closing( object sender, CancelEventArgs e )
		{
			ServiceCommandExecution execution = ( ( Window ) sender ).DataContext as ServiceCommandExecution ;
			RemoveWindow( execution.Service.Jid.ToString() ) ;

			( ( Window ) sender ).Closing -= commandExecuteWindow_Closing ;
		}

		public void DisplayQuestionaire( Command command, Service service )
		{
			App.InvokeSafe( DispatcherPriority.ApplicationIdle,
			                new DisplayCallback( DisplayQuestionaireInternal ), command, service ) ;
		}

		public Service ChooseCommand( Service service )
		{
			if ( service.Commands.Count == 0 )
			{
				return service ;
			}

			if ( service.Commands.Count == 1 )
			{
				return service.Commands[ 0 ] ;
			}

			foreach ( Service command in service.Commands )
			{
				if ( JidUtil.CompareDiscoItem( command.DiscoItem, service.DiscoItem ) )
				{
					return command ;
				}
			}

			ChooseCommand chooseCommand = new ChooseCommand() ;
			chooseCommand.DataContext = service ;

			chooseCommand.ShowDialog() ;

			if ( ( bool )chooseCommand.DialogResult )
			{
				return chooseCommand.Service ;
			}
			else
			{
				return null ;
			}
		}
	}
}