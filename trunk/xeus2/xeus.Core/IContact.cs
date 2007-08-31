using System.Windows.Media.Imaging;
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
        string XStatusText { get; }

		string FullName { get ; }
		string NickName { get ; }
        BitmapImage Image { get; }

        string CustomName { get; }

        bool IsService { get; }
	}
}
