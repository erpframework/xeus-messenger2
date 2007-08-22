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

        void Login(string bare, string forceNick)
        {
            Jid jid = new Jid(bare);

            _mucMark = MucMarks.Instance.Find(bare);

            if (_mucMark == null)
            {
                _mucMark = new MucMark(bare);
            }

            if (!string.IsNullOrEmpty(forceNick))
            {
                _mucMark.Nick = forceNick;
            }

            Service service;

            lock (Services.Instance._syncObject)
            {
                service = Services.Instance.FindService(jid);
            }

            if (service == null)
            {
                if (_mucMark.Service != null)
                {
                    service = _mucMark.Service;
                }
                else
                {
                    // not on this server
                    service = new Service(new DiscoItem(), false);
                    service.DiscoItem.Jid = jid;
                }
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

        internal RoomLogin(MucMark mucMark, string forceNick)
        {
            Login(mucMark.Jid, forceNick);
        }

        internal RoomLogin(string jid, string forceNick)
        {
            Login(jid, forceNick);
        }

        /*
        internal RoomLogin(Service service, string forceNick)
		{
		    MucMark mucMark = MucMarks.Instance.Find(service);

            if (mucMark != null)
            {
                if (!string.IsNullOrEmpty(forceNick))
                {
                    mucMark.Nick = forceNick;
                }

                Login(mucMark);
                return;
            }

		    DataContext = service;

			InitializeComponent() ;

			if ( !string.IsNullOrEmpty( forceNick ) )
			{
				_nick.Text = forceNick ;
				_nick.IsEnabled = false ;
			}
            else
			{
			    _nick.Text = Account.Instance.MyJid.User;
			}

			if (!service.IsMucPasswordProtected)
			{
				_passwordPanel.Visibility = Visibility.Collapsed ;
			}
		}*/

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

                Database.SaveMucMarks();
            }

			Close() ;
		}
	}
}