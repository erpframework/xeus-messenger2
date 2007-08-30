using System.Windows ;
using agsXMPP;
using agsXMPP.protocol.iq.disco;
using xeus.Data;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for RoomLogin.xaml
	/// </summary>
	public partial class RoomLogin : Window
	{
	    private MucMark _mucMark;

        internal RoomLogin(Service service, string forceNick)
        {
            _mucMark = MucMarks.Instance.Find(service);

            if (_mucMark == null)
            {
                _mucMark = new MucMark(service);
            }

            if (!string.IsNullOrEmpty(forceNick))
            {
                _mucMark.Nick = forceNick;
            }

            DataContext = service;

            InitializeComponent();

            if (!string.IsNullOrEmpty(_mucMark.Nick))
            {
                _nick.Text = _mucMark.Nick;
            }
            else
            {
                _nick.Text = Account.Instance.MyJid.User;
            }

            if (!service.IsMucPasswordProtected)
            {
                _passwordPanel.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(_mucMark.Password))
            {
                _password.Password = _mucMark.Password;
            }            
        }

        internal RoomLogin(MucMark mucMark, string forceNick):this(mucMark.Service, forceNick)
        {
        }

		protected void OnJoin( object sender, RoutedEventArgs eventArgs )
		{
			Middle.Muc.Instance.DisplayMuc( DataContext as Service, _nick.Text, _password.Password );

            if (_mucMark != null)
            {
                if (!string.IsNullOrEmpty(_nick.Text))
                {
                    _mucMark.Nick = _nick.Text;
                }

                if (!string.IsNullOrEmpty(_password.Password))
                {
                    _mucMark.Password = _password.Password;
                }

                Account.Instance.MucMarkManager.SaveBookmarks();
            }

			Close() ;
		}
	}
}