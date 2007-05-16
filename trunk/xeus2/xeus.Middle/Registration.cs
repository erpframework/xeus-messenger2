using System.Windows.Threading ;
using agsXMPP.protocol.iq.register ;

namespace xeus2.xeus.Middle
{
	internal class Registration
	{
		private delegate void DisplayCallback( Register register ) ;

		private static Registration _instance = new Registration() ;

		public static Registration Instance
		{
			get
			{
				return _instance ;
			}
		}

		protected void InBandRegistration( Register register )
		{
			UI.Registration registration = new UI.Registration( register ) ;

			registration.Show() ;
		}

		public void DisplayInBandRegistration( Register register )
		{
			App.InvokeSafe( DispatcherPriority.Normal,
			                new DisplayCallback( InBandRegistration ), register ) ;
		}
	}
}