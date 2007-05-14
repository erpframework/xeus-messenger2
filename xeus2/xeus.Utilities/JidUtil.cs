using agsXMPP ;

namespace xeus2.xeus.Utilities
{
	internal static class JidUtil
	{
		public static bool Equals( Jid jid, Jid jid2 )
		{
			return jid.Equals( jid2 ) ;
		}

		public static bool BareEquals( Jid jid, Jid jid2 )
		{
			return ( string.Compare( jid.Bare, jid2.Bare, true ) == 0 ) ;
		}
	}
}