using System.Data ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;

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

				Rows.Add() ;
			}
		}
	}
}