using System;
using System.Collections.Generic;
using System.Text;
using System.Windows ;
using System.Windows.Threading ;
using agsXMPP.protocol.extensions.commands ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Middle
{
	internal class CommandExecutor : WindowManager< Service, UI.CommandExecute >
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
			UI.CommandExecute commandExecuteWindow = GetWindow( service ) ;

			if ( commandExecuteWindow == null )
			{
				commandExecuteWindow = new UI.CommandExecute( command, service ) ;
				commandExecuteWindow.DataContext = service ;
				AddWindow( service, commandExecuteWindow );
			}

			commandExecuteWindow.Closing += new System.ComponentModel.CancelEventHandler( commandExecuteWindow_Closing );
			commandExecuteWindow.Show() ;
		}

		void commandExecuteWindow_Closing( object sender, System.ComponentModel.CancelEventArgs e )
		{
			RemoveWindow( ( ( Window )sender ).DataContext as Service );
			( ( Window ) sender ).Closing -= commandExecuteWindow_Closing ;
		}

		public void DisplayQuestionaire( Command command, Service service )
		{
			App.InvokeSafe( DispatcherPriority.Normal,
			                new DisplayCallback( DisplayQuestionaireInternal ), command, service ) ;
		}

	}
}
