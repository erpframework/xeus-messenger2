using System.Windows ;
using System.Windows.Threading ;
using agsXMPP.protocol.iq.register ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Middle
{
	internal class Registration : WindowManager< Service, UI.Registration >
	{
		private delegate void DisplayCallback( Register register, Service service ) ;

		private static Registration _instance = new Registration() ;

		public static Registration Instance
		{
			get
			{
				return _instance ;
			}
		}

		protected void InBandRegistration( Register register, Service service )
		{
			UI.Registration registration = GetWindow( service ) ;

			if ( registration == null )
			{
				registration = new UI.Registration( register, service ) ;
				registration.Closing += new System.ComponentModel.CancelEventHandler( registration_Closing );
				registration.DataContext = service ;
				AddWindow( service, registration );
			}

			registration.Show() ;
		}

		void registration_Closing( object sender, System.ComponentModel.CancelEventArgs e )
		{
			RemoveWindow( ( ( Window )sender ).DataContext as Service );
			( ( Window ) sender ).Closing -= registration_Closing ;
		}

		public void DisplayInBandRegistration( Register register, Service service )
		{
			App.InvokeSafe( DispatcherPriority.Normal,
			                new DisplayCallback( InBandRegistration ), register, service ) ;
		}
	}
}