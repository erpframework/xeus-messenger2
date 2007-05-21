using System.Windows.Controls ;
using System.Windows.Data ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;

namespace xeus2.xeus.XData
{
	public class XDataSearchResultHeader : GridView
	{
		public XDataSearchResultHeader( FieldContainer fieldContainer )
		{
			foreach ( Node node in fieldContainer.ChildNodes )
			{
				Field field = node as Field ;

				if ( field != null )
				{
					GridViewColumn column = new GridViewColumn() ;
					column.DisplayMemberBinding = new Binding( field.Var ) ;
					column.Header = field.Label ;

					Columns.Add( column ) ;
				}
			}
		}
	}
}