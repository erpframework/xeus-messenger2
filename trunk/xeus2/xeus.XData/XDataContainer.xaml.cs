using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;

namespace xeus2.xeus.XData
{
	/// <summary>
	/// Interaction logic for XDataContainer.xaml
	/// </summary>
	public partial class XDataContainer : UserControl
	{
		private Data _data = null ;

		public XDataContainer()
		{
			InitializeComponent() ;
		}

		public Data Data
		{
			get
			{
				return _data ;
			}

			set
			{
				_data = value ;

				BuildForm() ;
			}
		}

		public Data GetResult()
		{
			Data data = new Data( XDataFormType.submit ) ;

			foreach ( XDataControl control in _container.Children )
			{
				data.AddChild( control.GetResult() ) ;
			}

			return data ;
		}

		void BuildForm()
		{
			// fields
			foreach ( Node node in _data.ChildNodes )
			{
				Field field = node as Field ;

				if ( field != null )
				{
					CreateFieldControl( field ) ;
				}
			}
		}

		private void CreateFieldControl( Field field )
		{
			XDataControl control = null ;

			switch ( field.Type )
			{
				case FieldType.Boolean:
					{
						control = new XDataCheckBox() ;
						break ;
					}
				case FieldType.Fixed:
					{
						control = new XDataFixed() ;
						break ;
					}
				case FieldType.Hidden:
					{
						control = new XDataHidden() ;
						break ;
					}
				case FieldType.List_Multi:
					{
						control = new XDataListMulti() ;
						break ;
					}
				case FieldType.List_Single:
					{
						control = new XDataListSingle() ;
						break ;
					}
				case FieldType.Text_Private:
					{
						control = new XDataSecret() ;
						break ;
					}
				case FieldType.Jid_Multi:
				case FieldType.Jid_Single:
				case FieldType.Text_Multi:
				case FieldType.Text_Single:
					{
						control = new XDataTextBox() ;
						break ;
					}
				default:
					{
						break ;
					}
			}

			control.Field = field ;
			_container.Children.Add( control ) ;
		}
	}
}