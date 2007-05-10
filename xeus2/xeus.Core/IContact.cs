using agsXMPP ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	interface IContact
	{
		Jid Jid { get ; }
		Presence Presence { get ; }
		string DisplayName { get ; }
		string Group { get ; }
	}
}
