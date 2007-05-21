using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using agsXMPP.protocol.extensions.commands ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for CommandExecute.xaml
	/// </summary>

	public partial class CommandExecute : System.Windows.Window
	{
		internal CommandExecute( Command command, Service service )
		{
			InitializeComponent() ;

			_execute.Setup( command, service ) ;
		}

		protected void OnExecute( object sender, RoutedEventArgs eventArgs )
		{
			if ( !_execute.IsValid )
			{
				return ;
			}

			if ( _execute.XData != null )
			{
				// Account.Instance.DoSearchService( _execute.Service, _search.GetResult() ) ;
			}
			else
			{
			}
		}
	}
}