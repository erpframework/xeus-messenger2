using System ;
using System.Windows ;
using System.Windows.Controls ;
using agsXMPP.protocol.iq.register ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.XData
{
	/// <summary>
	/// Interaction logic for XDataFromBase.xaml
	/// </summary>
	public partial class XDataFromBase : UserControl
	{
		public XDataFromBase()
		{
			InitializeComponent() ;
		}

		private XDataContainer _xDataContainer = null ;
		private Service _service = null ;

		private Data _xData = null ;

		public bool IsValid
		{
			get
			{
				if ( XData == null )
				{
					/*if ( _register.Username != null
					     && _textUserName.GetResult().GetValue() == String.Empty )
					{
						return false ;
					}

					if ( _register.Password != null
					     && _textPassword.GetResult().GetValue() == String.Empty )
					{
						return false ;
					}

					if ( _register.Email != null
					     && _textEmail.GetResult().GetValue() == String.Empty )
					{
						return false ;
					}*/

					return true ;
				}
				else
				{
					return _xDataContainer.IsValid ;
				}
			}
		}

		private void SetupXDataRegistration( Data xData )
		{
			if ( string.IsNullOrEmpty( xData.Title ) )
			{
				_title.Visibility = Visibility.Collapsed ;
			}
			else
			{
				_title.Text = xData.Title ;
			}

			if ( string.IsNullOrEmpty( xData.Instructions ) )
			{
				_instructions.Visibility = Visibility.Collapsed ;
			}
			else
			{
				_instructions.Text = xData.Instructions ;
			}

			_xDataContainer = new XDataContainer() ;
			_container.Children.Add( _xDataContainer ) ;

			_xDataContainer.Data = xData ;
		}

		public Data GetResult()
		{
			if ( _xDataContainer != null )
			{
				return _xDataContainer.GetResult() ;
			}
			else
			{
				return null ;
			}
		}

		public Data XData
		{
			get
			{
				if ( _xDataContainer != null )
				{
					return _xDataContainer.Data ;
				}
				else
				{
					return null ;
				}
			}
		}

		internal Service Service
		{
			get
			{
				return _service ;
			}
		}
	}
}