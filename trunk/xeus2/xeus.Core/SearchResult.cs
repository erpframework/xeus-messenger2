using System.Data ;
using agsXMPP.protocol.iq.search ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;
using xeus2.Properties ;

namespace xeus2.xeus.Core
{
	internal class SearchResult : DataTable
	{
		public SearchResult( FieldContainer fieldContainer, Item [] items )
		{
			foreach ( Node node in fieldContainer.ChildNodes )
			{
				Field field = node as Field ;

				if ( field != null )
				{
					DataColumn column = new DataColumn() ; 
					column.DataType = typeof ( string ) ; 
					column.Caption = field.Label ; 
					column.ColumnName = field.Var ; 

					Columns.Add( column ) ;
				}
			}

			foreach ( Item item in items )
			{
				DataRow row = NewRow() ;

				foreach ( Node node in item.ChildNodes )
				{
					Field field = node as Field ;

					if ( field != null )
					{
						row[ field.Var ] = field.GetValue() ;
					}
				}

				Rows.Add( row ) ;
			}
		}

		public SearchResult( SearchItem [] items )
		{
			DataColumn column = new DataColumn() ; 
			column.DataType = typeof ( string ) ; 
			column.Caption = Resources.Constant_Jid ; 
			column.ColumnName = "jid" ; 
			Columns.Add( column ) ;
			
			column = new DataColumn() ; 
			column.DataType = typeof ( string ) ; 
			column.Caption = Resources.Constant_FirstName ; 
			column.ColumnName = "first" ; 
			Columns.Add( column ) ;

			column = new DataColumn() ; 
			column.DataType = typeof ( string ) ; 
			column.Caption = Resources.Constant_LastName ; 
			column.ColumnName = "last" ; 
			Columns.Add( column ) ;

			column = new DataColumn() ; 
			column.DataType = typeof ( string ) ; 
			column.Caption = Resources.Constant_Nickname ; 
			column.ColumnName = "nick" ; 
			Columns.Add( column ) ;

			column = new DataColumn() ; 
			column.DataType = typeof ( string ) ; 
			column.Caption = Resources.Constant_Email ; 
			column.ColumnName = "email" ; 
			Columns.Add( column ) ;

			foreach ( SearchItem item in items )
			{
				DataRow row = NewRow() ;
				row[ "jid" ] = item.Jid ;
				row[ "first" ] = item.Firstname ;
				row[ "last" ] = item.Lastname ;
				row[ "nick" ] = item.Nickname ;
				row[ "email" ] = item.Email ;

				Rows.Add( row ) ;
			}
		}
	}
}