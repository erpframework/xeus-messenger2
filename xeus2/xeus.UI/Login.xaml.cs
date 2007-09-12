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

		public string Password
		{
			get
			{
				return _password.Password ;
			}
		}

		public string Server
		{
			get
			{
				return _server.Text ;
			}
		}

		public string User
		{
			get
			{
				return _user.Text ;
			}
		}

		protected void OnLogin( object sender, RoutedEventArgs eventArgs )
		{
			DialogResult = true;
		}
	}
}