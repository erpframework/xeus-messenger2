using System ;
using System.Text ;
using System.Text.RegularExpressions ;
using FastDynamicPropertyAccessor ;
using xeus2.Properties ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Utilities
{
	internal class DisplayNameBuilder
	{
		public string GetDisplayName( IContact contact )
		{
			string[] properties = GetPropertiesFromString( Settings.Default.UI_DisplayFormat ) ;

			object[] values = GetValues( properties, contact ) ;
	
			return string.Format( PrepareFormatString( Settings.Default.UI_DisplayFormat, properties ),
									values ) ;
		}

		public Regex _regex = new Regex(
			"\\{[A-Za-z]*\\}",
			RegexOptions.IgnoreCase
			| RegexOptions.CultureInvariant
			| RegexOptions.IgnorePatternWhitespace
			| RegexOptions.Compiled
			) ;

		private string[] GetPropertiesFromString( string text )
		{
			MatchCollection ms = _regex.Matches( text ) ;

			string[] properties = new string[ ms.Count ];

			for ( int i = 0; i < ms.Count; i++ )
			{
				properties[ i ] = ms[ i ].ToString() ;
			}

			return properties ;
		}

		string PrepareFormatString( string format, string [] properties )
		{
			StringBuilder result = new StringBuilder( format );

			for ( int i = 0; i < properties.Length; i++ )
			{
				result.Replace( properties[ i ], string.Format( "{{{0}}}", i ) ) ;
			}

			return result.ToString() ;
		}

		object [] GetValues( string [] properties, IContact contact )
		{
			object [] values = new object[ properties.Length ];

			for ( int i = 0; i < properties.Length; i++ )
			{
				if ( properties[ i ].Length >= 3 )
				{
					string propertyName = properties[ i ].Substring( 1, properties[ i ].Length - 2 ) ;

					if ( propertyName != "DisplayName" )
					{
						try
						{
							PropertyAccessor propertyAccessor =
								new PropertyAccessor( typeof ( IContact ), propertyName ) ;

							values[ i ] = propertyAccessor.Get( contact ) ;
						}

						catch ( PropertyAccessorException )
						{
							values[ i ] = string.Format( Resources.Error_InvalidContactProp, propertyName ) ;
						}
					}
					else
					{
						values[ i ] = Resources.Error_InvDisplayName ;
					}
				}
				else
				{
					values[ i ] = null ;
				}
			}

			return values ;
		}
	}
}