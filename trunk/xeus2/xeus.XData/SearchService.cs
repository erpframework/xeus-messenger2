using System ;
using agsXMPP.protocol.iq.search ;
using agsXMPP.protocol.x.data ;
using xeus2.xeus.Core ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.XData
{
	public class SearchService : XDataFormBase
	{
		private Search _search = null ;

		private XDataTextBox _textFirst ;
		private XDataTextBox _textLast ;
		private XDataTextBox _textNick ;
		private XDataTextBox _textEmail ;

		public Search Search
		{
			get
			{
				return _search ;
			}
		}

		internal void Setup( Search search, Service service )
		{
			Service = service ;
			_search = search ;

			_xData = ElementUtil.GetData( _search ) ;

			if ( _xData == null )
			{
				SetupSimpleSearch() ;
			}
			else
			{
				SetupXData( _xData ) ;
			}
		}

		private void SetupSimpleSearch()
		{
			_instructions.Text = _search.Instructions ;

			// first name
			if ( _search.Firstname != null )
			{
				_textFirst = new XDataTextBox() ;

				Field fieldUserName = new Field( "firstname", Properties.Resources.Constant_FirstName, FieldType.Text_Single ) ;
				fieldUserName.IsRequired = true ;
				fieldUserName.AddValue( _search.Firstname ) ;
				fieldUserName.Description = Properties.Resources.Constant_EnterFirstNameForSearch ;

				_textFirst.Field = fieldUserName ;

				_container.Children.Add( _textFirst ) ;
			}

			// last name
			if ( _search.Lastname != null )
			{
				_textLast = new XDataTextBox() ;

				Field fieldUserName = new Field( "lastname", Properties.Resources.Constant_LastName, FieldType.Text_Single ) ;
				fieldUserName.IsRequired = true ;
				fieldUserName.AddValue( _search.Lastname ) ;
				fieldUserName.Description = Properties.Resources.Constant_EnterLastNameForSearch ;

				_textLast.Field = fieldUserName ;

				_container.Children.Add( _textLast ) ;
			}

			// nickname
			if ( _search.Nickname != null )
			{
				_textNick = new XDataTextBox() ;

				Field fieldUserName = new Field( "nickname", Properties.Resources.Constant_Nickname, FieldType.Text_Single ) ;
				fieldUserName.IsRequired = true ;
				fieldUserName.AddValue( _search.Nickname ) ;
				fieldUserName.Description = Properties.Resources.Constant_EnterNicknameForSearch ;

				_textNick.Field = fieldUserName ;

				_container.Children.Add( _textNick ) ;
			}

			// email
			if ( _search.Email != null )
			{
				_textEmail = new XDataTextBox() ;

				Field fieldEmail = new Field( "email", Properties.Resources.Constant_Email, FieldType.Text_Single ) ;
				fieldEmail.IsRequired = true ;
				fieldEmail.AddValue( _search.Email ) ;
				fieldEmail.Description = Properties.Resources.Constant_EnterEmailForSearch ;

				_textEmail.Field = fieldEmail ;

				_container.Children.Add( _textEmail ) ;
			}
		}

		public override bool IsValid
		{
			get
			{
				if ( XData == null )
				{
					if ( _search.Firstname != null
					     && _textFirst.GetResult().GetValue() == String.Empty )
					{
						return false ;
					}

					if ( _search.Lastname != null
					     && _textLast.GetResult().GetValue() == String.Empty )
					{
						return false ;
					}

					if ( _search.Nickname != null
					     && _textNick.GetResult().GetValue() == String.Empty )
					{
						return false ;
					}

					if ( _search.Email != null
					     && _textEmail.GetResult().GetValue() == String.Empty )
					{
						return false ;
					}

					return true ;
				}
				else
				{
					return base.IsValid ;
				}
			}
		}

		public void UpdateData()
		{
			if ( _textFirst != null )
			{
				_search.Firstname = _textFirst.GetResult().GetValue() ;
			}

			if ( _textLast != null )
			{
				_search.Lastname = _textLast.GetResult().GetValue() ;
			}

			if ( _textNick != null )
			{
				_search.Nickname = _textNick.GetResult().GetValue() ;
			}

			if ( _textEmail != null )
			{
				_search.Email = _textEmail.GetResult().GetValue() ;
			}
		}

		public string FirstName
		{
			get
			{
				return _search.Firstname ;
			}
		}

		public string LastName
		{
			get
			{
				return _search.Lastname ;
			}
		}

		public string Nickname
		{
			get
			{
				return _search.Nickname ;
			}
		}

		public string Email
		{
			get
			{
				return _search.Email ;
			}
		}
	}
}