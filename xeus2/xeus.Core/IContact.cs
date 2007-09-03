using System.Windows.Media.Imaging;
using agsXMPP ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	public interface IContact
	{
		Jid Jid { get ; }
		Presence Presence { get ; }
        string Resource { get; }
		
        string DisplayName { get ; }
		string Group { get ; }
        bool IsAvailable { get; }
		string StatusText { get ; }
        string XStatusText { get; }

		string FullName { get ; }
		string NickName { get ; }
        BitmapImage Image { get; }
        bool IsImageTransparent { get; }

        string CustomName { get; }

        bool IsService { get; }
	}
}
