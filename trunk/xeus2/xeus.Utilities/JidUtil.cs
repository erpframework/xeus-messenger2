using agsXMPP ;
using agsXMPP.protocol.iq.disco ;

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

		public static bool CompareDiscoItem( DiscoItem discoItem, DiscoItem discoItem2 )
		{
			return Equals( discoItem.Jid, discoItem2.Jid )
				&& discoItem.Node == discoItem2.Node ;
		}
	}
}