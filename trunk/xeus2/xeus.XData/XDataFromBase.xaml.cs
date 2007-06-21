using System ;
using System.Windows ;
using System.Windows.Controls ;
using System.Windows.Media;
using agsXMPP.protocol.iq.register ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.XData
{
	/// <summary>
	/// Interaction logic for XDataFromBase.xaml
	/// </summary>
	public abstract partial class XDataFormBase : UserControl
	{
		public XDataFormBase()
		{
			InitializeComponent() ;
		}

        public abstract DrawingBrush Icon
        { 
            get;
        }


		private XDataContainer _xDataContainer = null ;
		private Service _service = null ;

		protected Data _xData = null ;

		public virtual bool IsValid
		{
			get
			{
				return _xDataContainer.IsValid ;
			}
		}

		protected void ClearXForm()
		{
			_container.Children.Remove( _xDataContainer );
		}

		protected void SetupXData( Data xData )
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

			if ( _xDataContainer != null )
			{
				ClearXForm();
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

			set
			{
				_service = value ;
			}
		}
	}
}