using System.Windows.Threading ;
using agsXMPP ;
using agsXMPP.protocol.iq.register ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Middle
{
	internal class Registration
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
			UI.Registration registration = new UI.Registration( register, service ) ;

			registration.Show() ;
		}

		public void DisplayInBandRegistration( Register register, Service service )
		{
			App.InvokeSafe( DispatcherPriority.Normal,
			                new DisplayCallback( InBandRegistration ), register, service ) ;
		}
	}
}