using System.Windows ;
using agsXMPP;
using agsXMPP.protocol.iq.disco;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for RoomLogin.xaml
	/// </summary>
	public partial class RoomLogin : Window
	{
        internal RoomLogin(MucMark mucMark)
        {
            Jid jid = new Jid(mucMark.Jid);

            Service service;

            lock (Services.Instance._syncObject)
            {
                service = Services.Instance.FindService(jid);
            }

            if (service == null)
            {
                if (mucMark.Service != null)
                {
                    service = mucMark.Service;
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

            if (!string.IsNullOrEmpty(mucMark.Nick))
            {
                _nick.Text = mucMark.Nick;
            }
            else
            {
                _nick.Text = Account.Instance.MyJid.User;
            }

            if (!service.IsMucPasswordProtected)
            {
                _passwordPanel.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(mucMark.Password))
            {
                _password.Password = mucMark.Password;
            }
        }

		internal RoomLogin( Service service, string forceNick )
		{
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
		}

		protected void OnJoin( object sender, RoutedEventArgs eventArgs )
		{
			Middle.Muc.Instance.DisplayMuc( DataContext as Service, _nick.Text, _password.Password );
			Close() ;
		}
	}
}