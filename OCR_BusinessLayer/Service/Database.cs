using System;
using System.Data.Odbc;
using System.Data.SqlClient;
using static OCR_BusinessLayer.CONSTANTS;

namespace OCR_BusinessLayer.Service
{
	public class Database
	{

		private SqlConnection _conn;
		public Database()
		{
			_conn = new SqlConnection();
			_conn.ConnectionString = @"Data Source = (LocalDb)\MSSQLLocalDB; 
									   Initial Catalog = OCR_2018; 
									   Integrated Security = True";
		   
		}
		/// <summary>
		/// Connect
		/// </summary>
		private bool Connect()
		{
			if (_conn.State != System.Data.ConnectionState.Open)
			{
				_conn.Open();
			}
			return true;

		}


		public object Execute(string SQL,Operation op )
		{
			if (Connect())
			{
				SqlCommand command = CreateCommand(SQL);
				switch (op)
				{
					case Operation.SELECT:
						return Select(command);
					case Operation.INSERT:
						return ExecuteNonQuery(command);                          
					case Operation.UPDATE:
						return ExecuteNonQuery(command);                        
					case Operation.DELETE:
						return ExecuteNonQuery(command);                        
				}
				
			}
			return -1;
		}

		/// <summary>
		/// Insert a record encapulated in the command.
		/// </summary>
		private int ExecuteNonQuery(SqlCommand command)
		{
			int rowNumber = 0;
			try
			{
				rowNumber = command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				throw e;
			}
			return rowNumber;
		}

		/// <summary>
		/// Create command
		/// </summary>
		private SqlCommand CreateCommand(string strCommand)
		{
			SqlCommand command = new SqlCommand(strCommand, _conn);            
			return command;
		}

		public bool CheckTableExists(string table)
		{
			bool exists;
			try
			{
				// ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL.  
				var cmd = new OdbcCommand(
				  $"SELECT CASE WHEN EXISTS((SELECT * FROM information_schema.tables where table_name = '{table}')) THEN 1 ELSE 0 END");

				exists = (int)cmd.ExecuteScalar() == 1;
			}
			catch
			{
				try
				{
					// Other RDBMS.  Graceful degradation
					exists = true;
					var cmdOthers = new OdbcCommand($"SELECT 1 FROM dbo.{table} WHERE 1 = 0");
					cmdOthers.ExecuteNonQuery();
				}
				catch
				{
					exists = false;
				}
			}
			return exists;
		}

		public void CreateTableIfNotExists(string table)
		{
				string create = $@"CREATE TABLE {table}(
									Word_ID int Primary key identity NOT NULL,
									Word_Key VARCHAR(50) NOT NULL,
									Word_Value VARCHAR(50) NOT NULL,
									K_X int NOT NULL,
									K_Y int NOT NULL,
									K_Width int NOT NULL,
									K_Height int NOT NULL,
									V_X int NOT NULL,
									V_Y int NOT NULL,
									V_Width int NOT NULL,
									V_Height int NOT NULL
								);";
			
			ExecuteNonQuery(CreateCommand(create));
		}

		/// <summary>
		/// Select encapulated in the command.
		/// </summary>
		public SqlDataReader Select(SqlCommand command)
		{
			SqlDataReader sqlReader = command.ExecuteReader();
			return sqlReader;
		}


		/// <summary>
		/// Close
		/// </summary>
		public void Close()
		{
			_conn.Close();
		}

	}
}
