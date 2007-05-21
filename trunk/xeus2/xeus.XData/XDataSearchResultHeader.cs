using System.Windows.Controls ;
using System.Windows.Data ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;
using xeus2.Properties ;

namespace xeus2.xeus.XData
{
	public class XDataSearchResultHeader : GridView
	{
		public XDataSearchResultHeader()
		{
			GridViewColumn column = new GridViewColumn() ;
			column.DisplayMemberBinding = new Binding( "jid" ) ;
			column.Header = Resources.Constant_Jid ;
			Columns.Add( column ) ;
			
			column = new GridViewColumn() ;
			column.DisplayMemberBinding = new Binding( "first" ) ;
			column.Header = Resources.Constant_FirstName ;
			Columns.Add( column ) ;

			column = new GridViewColumn() ;
			column.DisplayMemberBinding = new Binding( "last" ) ;
			column.Header = Resources.Constant_LastName ;
			Columns.Add( column ) ;

			column = new GridViewColumn() ;
			column.DisplayMemberBinding = new Binding( "nick" ) ;
			column.Header = Resources.Constant_Nickname ;
			Columns.Add( column ) ;

			column = new GridViewColumn() ;
			column.DisplayMemberBinding = new Binding( "email" ) ;
			column.Header = Resources.Constant_Email ;
			Columns.Add( column ) ;
		}

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