using System.Windows ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : BaseWindow
	{
	    public const string _keyBase = "Login";

		public Login() : base(_keyBase, string.Empty)
		{
			InitializeComponent() ;
		}
	}
}