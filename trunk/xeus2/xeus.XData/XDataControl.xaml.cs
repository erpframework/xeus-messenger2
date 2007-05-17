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

		public virtual Field GetResult()
		{
			throw new NotImplementedException() ;
		}

		protected virtual void OnFieldIsSet()
		{
			if ( !String.IsNullOrEmpty( Field.Label ) )
			{
				_description.Text = Field.Label ;
				_description.ToolTip = Field.Description ;

				_description.Visibility = Visibility.Visible ;
			}
			else if ( !String.IsNullOrEmpty( Field.Description ) )
			{
				_description.Text = Field.Description ;

				_description.Visibility = Visibility.Visible ;
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