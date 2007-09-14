using System ;
using System.Collections.Generic ;
using System.Data.Common ;
using System.Data.SQLite ;
using System.IO ;
using System.Text ;
using xeus2.xeus.Core ;

namespace xeus.Data
{
	internal class Database
	{
		private static string Path
		{
			get
			{
				return string.Format( "{0}\\{1}", Storage.GetDbFolder(), "xeus.db" ) ;
			}
		}

		private static SQLiteConnection _connection = null ;

		public static void OpenDatabase()
		{
			bool dbExisist = File.Exists( Path ) ;

			_connection = new SQLiteConnection( string.Format( "Data Source=\"{0}\"", Path ) );
			_connection.Open() ;

			if ( !dbExisist )
			{
				CreateDatabase() ;
			}
		}

		private static void CreateDatabase()
		{
			using ( SQLiteCommand cmd = _connection.CreateCommand() )
			{
				cmd.CommandText = "CREATE TABLE [Group] ([IsExpanded] INTEGER NOT NULL DEFAULT '0',"
				                  + "[Name] VARCHAR NOT NULL PRIMARY KEY UNIQUE);" ;
				cmd.ExecuteNonQuery() ;

				cmd.CommandText = "CREATE TABLE [Message] ([From] VARCHAR NOT NULL, "
				                  + "[To] VARCHAR NOT NULL, "
				                  + "[DateTime] INTEGER NOT NULL, "
				                  + "[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, "
				                  + "[Body] VARCHAR NOT NULL);" ;
				cmd.ExecuteNonQuery() ;

				cmd.CommandText = "CREATE TABLE [Contact] (Jid VARCHAR NOT NULL PRIMARY KEY UNIQUE, "
				                  + "[MetaId] VARCHAR, "
                                  + "[CustomName] VARCHAR);";
				cmd.ExecuteNonQuery() ;
            
                cmd.CommandText = "CREATE TABLE [MetaContact] (MetaId VARCHAR NOT NULL PRIMARY KEY UNIQUE, "
                                  + "[CustomName] VARCHAR);";
                cmd.ExecuteNonQuery();
            }
		}

		public static void CloseDatabase()
		{
			_connection.Close() ;
		}

        /*
        public static void LoadMucMarks()
        {
 			try
			{
                lock (MucMarks.Instance._syncObject)
                {
                    MucMarks.Instance.Clear();

                    using (SQLiteCommand command = _connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM [MucMark] ORDER BY [Name]";

                        SQLiteDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            MucMarks.Instance.Add(new MucMark(reader));
                        }

                        reader.Close();
                    }
                }
			}

			catch ( Exception e )
			{
                EventException eventError = new EventException("Error reading MUC Bookmarks", e);
			    Events.Instance.OnEvent( null, eventError ) ;
			}
        }*/

        /*
        public static void SaveMucMarks()
        {
            try
            {
                using (SQLiteTransaction transaction = _connection.BeginTransaction())
                {
                    lock (MucMarks.Instance._syncObject)
                    {
                        foreach (MucMark mucMark in MucMarks.Instance)
                        {
                            Dictionary<string, object> values = new Dictionary<string, object>();

                            values.Add("Id", mucMark.Id);
                            values.Add("Nick", mucMark.Nick);
                            values.Add("Jid", mucMark.Jid);
                            values.Add("Password", mucMark.Password);
                            values.Add("Name", mucMark.Name);
                            values.Add("Time", mucMark.Time.ToBinary());

                            mucMark.Id = SaveOrUpdate(values, "Id", "MucMark", true, _connection);
                        }

                        transaction.Commit();
                    }
                }
            }

            catch (Exception e)
            {
                Events.Instance.OnEvent(null, new EventError(e.Message, null));
            }
        }*/

		/*
		public List< RosterItem > ReadRosterItems()
		{
			List< RosterItem > rosterItems = new List< RosterItem >() ;

			try
			{
				using ( SQLiteCommand command = _connection.CreateCommand() )
				{
					command.CommandText = "SELECT * FROM [RosterItem]" ;

					SQLiteDataReader reader = command.ExecuteReader() ;

					while ( reader.Read() )
					{
						RosterItem rosterItem = new RosterItem( reader ) ;
						rosterItems.Add( rosterItem ) ;
					}

					reader.Close() ;
				}
			}

			catch ( Exception e )
			{
				Client.Instance.Log( "Error reading Roster items: {0}", e.Message ) ;
			}

			return rosterItems ;
		}

		public List< ChatMessage > ReadMessages( RosterItem rosterItem )
		{
			List< ChatMessage > messages = new List< ChatMessage >() ;

			int maxMessages = Settings.Default.Roster_MaximumMessagesToLoad ;

			try
			{
				using ( SQLiteCommand command = _connection.CreateCommand() )
				{
					command.CommandText =
						string.Format( "SELECT * FROM [Message] WHERE [Key]=@key ORDER BY [Id] DESC LIMIT {0}", maxMessages ) ;

					command.Parameters.Add( new SQLiteParameter( "key", rosterItem.Key ) ) ;

					SQLiteDataReader reader = command.ExecuteReader() ;

					while ( reader.Read() )
					{
						messages.Insert( 0, new ChatMessage( reader, rosterItem ) ) ;
					}

					reader.Close() ;
				}
			}

			catch ( Exception e )
			{
				Client.Instance.Log( "Error reading messages: {0}", e.Message ) ;
			}

			return messages ;
		}

		public ChatMessage GetChatMessage( Int64 id, RosterItem rosterItem )
		{
			ChatMessage chatMessage = null ;

			try
			{
				using ( SQLiteCommand command = _connection.CreateCommand() )
				{
					command.CommandText = "SELECT * FROM [Message] WHERE [Id]=@id" ;

					command.Parameters.Add( new SQLiteParameter( "Id", id ) ) ;

					SQLiteDataReader reader = command.ExecuteReader() ;

					while ( reader.Read() )
					{
						chatMessage = new ChatMessage( reader, rosterItem ) ;
					}

					reader.Close() ;
				}
			}

			catch ( Exception e )
			{
				Client.Instance.Log( "Error reading messages: {0}", e.Message ) ;
			}

			return chatMessage ;
		}

		public int InsertMessage( ChatMessage message )
		{
			int id = 0 ;

			try
			{
				Dictionary< string, object > values = message.GetData() ;

				id = Insert( values, "Message", true, _connection ) ;
			}

			catch ( Exception e )
			{
				Client.Instance.Log( "Error writing groups: {0}", e.Message ) ;
			}

			return id ;
		}
		*/
		public void StoreGroups( Dictionary< string, bool > expanderStates )
		{
			try
			{
				using ( SQLiteTransaction transaction = _connection.BeginTransaction() )
				{
					foreach ( KeyValuePair< string, bool > state in expanderStates )
					{
						Dictionary< string, object > values = new Dictionary< string, object >() ;

						values.Add( "Name", state.Key ) ;
						values.Add( "IsExpander", ( state.Value ) ? 1 : 0 ) ;

						SaveOrUpdate( values, "Name", "Group", false, _connection ) ;
					}

					transaction.Commit();
				}
			}

			catch ( Exception e )
			{
				Events.Instance.OnEvent( this, new EventError( e.Message, null ) ) ;
			}
		}

		public Dictionary< string, bool > ReadGroups()
		{
			Dictionary< string, bool > expanderStates = new Dictionary< string, bool >() ;

			try
			{
				using ( SQLiteCommand command = _connection.CreateCommand() )
				{
					command.CommandText = "SELECT * FROM [Group]" ;

					SQLiteDataReader reader = command.ExecuteReader() ;

					while ( reader.Read() )
					{
						expanderStates.Add( ( string ) reader[ "Name" ], ( ( Int64 ) reader[ "IsExpander" ] ) == 1 ) ;
					}

					reader.Close() ;
				}
			}

			catch ( Exception e )
			{
				Events.Instance.OnEvent( this, new EventError( e.Message, null ) ) ;
			}

			return expanderStates ;
		}
		/*
		public void SaveRosterItem( RosterItem item )
		{
			if ( item.IsService )
			{
				return ;
			}

			try
			{
				if ( !item.IsInDatabase )
				{
					Insert( item.GetData(), "RosterItem", false, _connection ) ;
					item.IsInDatabase = true ;
					item.IsDirty = false ;
				}
				else if ( item.IsDirty )
				{
					Update( item.GetData(), "Key", "RosterItem", _connection ) ;
					item.IsDirty = false ;
				}
			}

			catch ( Exception e )
			{
				Client.Instance.Log( "Error writing roster items: {0}", e.Message ) ;
			}
		}

		public void StoreRosterItems( ObservableCollectionDisp< RosterItem > rosterItems )
		{
			lock ( rosterItems._syncObject )
			{
				using ( SQLiteTransaction transaction = _connection.BeginTransaction() )
				{
					foreach ( RosterItem item in rosterItems )
					{
						SaveRosterItem( item ) ;
					}

					transaction.Commit();
				}
			}
		}

		public void DeleteRosterItem( RosterItem rosterItem )
		{
			Delete( "RosterItem", "Key", rosterItem.Key, _connection ) ;
		}

		void Delete( string table, string keyField, object keyValue, SQLiteConnection connection )
		{
			using ( SQLiteCommand commandDelete = connection.CreateCommand() )
			{
				StringBuilder queryDelete = new StringBuilder() ;

				queryDelete.AppendFormat( "DELETE FROM [{0}] WHERE [{1}]=@{1}", table, keyField ) ;
				
				commandDelete.Parameters.Add( new SQLiteParameter( keyField, keyValue ) ) ;

				commandDelete.CommandText = queryDelete.ToString() ;
				commandDelete.ExecuteNonQuery() ;
			}
		}
		*/
		private static Int32 Insert( Dictionary< string, object > values, string table, bool readAutoId,
		                      SQLiteConnection connection )
		{
			using ( SQLiteCommand commandUpdate = connection.CreateCommand() )
			{
				StringBuilder queryUpdate = new StringBuilder() ;

				queryUpdate.AppendFormat( "INSERT INTO [{0}] (", table ) ;

				bool isFirst = true ;

				foreach ( KeyValuePair< string, object > pair in values )
				{
                    if (pair.Key == "Id")
                    {
                        //don't insert auto id
                        continue;
                    }

					if ( !isFirst )
					{
						queryUpdate.Append( "," ) ;
					}

					isFirst = false ;

					queryUpdate.AppendFormat( "[{0}]", pair.Key ) ;
				}

				queryUpdate.Append( ") VALUES (" ) ;

				isFirst = true ;

				foreach ( KeyValuePair< string, object > pair in values )
				{
                    if (pair.Key == "Id")
                    {
                        //don't insert auto id
                        continue;
                    }

					if ( !isFirst )
					{
						queryUpdate.Append( "," ) ;
					}

					isFirst = false ;

					queryUpdate.AppendFormat( "@{0}", pair.Key ) ;

					commandUpdate.Parameters.Add( new SQLiteParameter( pair.Key, pair.Value ) ) ;
				}

				queryUpdate.Append( ")" ) ;

				commandUpdate.CommandText = queryUpdate.ToString() ;
				commandUpdate.ExecuteNonQuery() ;
			}

			if ( readAutoId )
			{
				return GetlastRowId( connection ) ;
			}
			else
			{
				return 0 ;
			}
		}

		private static void Update( Dictionary< string, object > values, string keyField, string table, SQLiteConnection connection )
		{
			using ( SQLiteCommand commandUpdate = connection.CreateCommand() )
			{
				StringBuilder queryUpdate = new StringBuilder() ;

				queryUpdate.AppendFormat( "UPDATE [{0}] SET ", table ) ;

				bool isFirst = true ;

				foreach ( KeyValuePair< string, object > pair in values )
				{
					if ( string.Compare( keyField, pair.Key, true ) == 0 )
					{
						continue ;
					}

					if ( !isFirst )
					{
						queryUpdate.Append( "," ) ;
					}

					isFirst = false ;

					queryUpdate.AppendFormat( "[{0}]=@{0}", pair.Key ) ;

					commandUpdate.Parameters.Add( new SQLiteParameter( pair.Key, pair.Value ) ) ;
				}

				queryUpdate.AppendFormat( " WHERE [{0}]=@{0}", keyField ) ;
				commandUpdate.Parameters.Add( new SQLiteParameter( keyField, values[ keyField ] ) ) ;

				commandUpdate.CommandText = queryUpdate.ToString() ;
				commandUpdate.ExecuteNonQuery() ;
			}
		}

		private static Int32 SaveOrUpdate( Dictionary< string, object > values, string keyField, string table, bool readAutoId,
		                            SQLiteConnection connection )
		{
			bool exists = false ;

			int id = 0 ;

			if ( keyField != null )
			{
				StringBuilder query = new StringBuilder() ;

				query.AppendFormat( "SELECT * FROM [{0}] WHERE [{1}]=@keyparam", table, keyField ) ;

				using ( SQLiteCommand command = connection.CreateCommand() )
				{
					command.CommandText = query.ToString() ;

					command.Parameters.Add( new SQLiteParameter( "keyparam", values[ keyField ] ) ) ;

				    id = (int)values[keyField];

					SQLiteDataReader reader = command.ExecuteReader() ;

					exists = reader.HasRows ;

					reader.Close() ;
				}
			}

			if ( exists )
			{
				Update( values, keyField, table, connection ) ;
			}
			else
			{
				id = Insert( values, table, readAutoId, connection ) ;
			}

			return id ;
		}

		private static int GetlastRowId( SQLiteConnection connection )
		{
			int id = 0 ;

			using ( SQLiteCommand command = connection.CreateCommand() )
			{
				command.CommandText = "SELECT last_insert_rowid()" ;

				SQLiteDataReader reader = command.ExecuteReader() ;

				if ( reader.Read() )
				{
					id = ( Int32 ) ( Int64 ) reader[ 0 ] ;
				}

				reader.Close() ;
			}

			return id ;
		}

        /*
	    public static void DeleteMucMark(MucMark mucMark)
	    {
            try
            {
                using (SQLiteTransaction transaction = _connection.BeginTransaction())
                {
                    using (SQLiteCommand command = _connection.CreateCommand())
                    {
                        command.CommandText = string.Format("DELETE FROM [MucMark] WHERE [Id]={0}", mucMark.Id);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            catch (Exception e)
            {
                EventException eventError = new EventException("Error deleting MUC Bookmarks", e);
                Events.Instance.OnEvent(null, eventError);
            }
        }*/
	}
}