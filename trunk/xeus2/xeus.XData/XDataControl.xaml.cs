using System ;
using System.Windows ;
using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;

namespace xeus2.xeus.XData
{
	/// <summary>
	/// Interaction logic for XDataControl.xaml
	/// </summary>
	public partial class XDataControl : UserControl
	{
		private Field _field = null ;

		public XDataControl()
		{
			InitializeComponent() ;
		}

		public Field Field
		{
			get
			{
				return _field ;
			}

			set
			{
				_field = value ;
				
				OnFieldIsSet() ;
			}
		}

		virtual protected void OnFieldIsSet()
		{
			_container.Children.Clear();

			if ( !String.IsNullOrEmpty( Field.Description ) )
			{
				_description.Text = Field.Label ;
				_description.ToolTip = Field.Description ;

				_container.Children.Add( _description ) ;
			}

			if ( Field.IsRequired )
			{
				_required.Visibility = Visibility.Visible ;
			}
		}

		public virtual bool Validate()
		{
			return false ;
		}
	}
}