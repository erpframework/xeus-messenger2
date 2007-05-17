using System.Windows.Threading ;
using agsXMPP ;
using agsXMPP.protocol.iq.register ;

namespace xeus2.xeus.Middle
{
	internal class Registration
	{
		private delegate void DisplayCallback( Register register, Jid jid ) ;

		private static Registration _instance = new Registration() ;

		public static Registration Instance
		{
			get
			{
				return _instance ;
			}
		}

		protected void InBandRegistration( Register register, Jid from )
		{
			UI.Registration registration = new UI.Registration( register, from ) ;

			registration.Show() ;
		}

		public void DisplayInBandRegistration( Register register, Jid from )
		{
			App.InvokeSafe( DispatcherPriority.Normal,
			                new DisplayCallback( InBandRegistration ), register, from ) ;
		}
	}
}