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
using xeus2.xeus.Commands ;
using xeus2.xeus.Core ;

namespace xeus2
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class RosterWindow : System.Windows.Window
	{
		public RosterWindow()
		{
			InitializeComponent();
		}

		public override void EndInit()
		{
			base.EndInit();

			ServiceCommands.RegisterCommands( this );
			AccountCommands.RegisterCommands( this );

			Account.Instance.Open();
		}

		protected override void OnClosed( EventArgs e )
		{
			Account.Instance.Close();

			base.OnClosed( e );
		}

		void RosterMouseDoubleClick( object sender, RoutedEventArgs args )
		{
		}
	}
}