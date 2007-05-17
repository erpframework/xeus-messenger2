using System.Windows ;
using agsXMPP.protocol.x.data ;

namespace xeus2.xeus.XData
{
	internal class XDataHidden : XDataTextBox
	{
		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			Visibility = Visibility.Collapsed ;
		}

		public override bool Validate()
		{
			// hidden field can't be wrong
			return true ;
		}

		public override Field GetResult()
		{
			Field field = new Field( Field.Var, null, Field.Type );
			field.SetValue( Field.GetValue() );

			return field ;
		}
	}
}