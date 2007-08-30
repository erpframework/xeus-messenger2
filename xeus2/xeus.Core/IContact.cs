using agsXMPP ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	public interface IContact
	{
		Jid Jid { get ; }
		Presence Presence { get ; }
		string DisplayName { get ; }
		string Group { get ; }
		string StatusText { get ; }

		string FullName { get ; }
		string NickName { get ; }

        string CustomName { get; }
	}
}
