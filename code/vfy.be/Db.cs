using System;
using System.Linq;
using System.Data;
using Mono.Data.Sqlite;
using vfy.be.Interfaces;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Dynamic;

namespace vfy.be
{	
	public class Db : IShortenerDb
	{
		private readonly String _dbUri;
		
		private const String ConnStringWithOutUri = @"URI=file:{0}";
		
		public Db() : this("./links.db") {}
		
		public Db(String dbUri)
		{
			if(String.IsNullOrEmpty(dbUri)) throw new ArgumentNullException("dbUri");
			_dbUri = dbUri;	
		}

		public Int32 InsertUrl (string url)
		{
			using(var db = OpenOrCreateDatabase())
			{
				var urlParam = new SqliteParameter("@Url", url);
				
				return (Int32)RunQueryAndReturnScalarValue<Int64>(SQL.InsertAndReturnId, db, new[] {urlParam});
			}
		}

		public Int32? GetIdForUrl (string url)
		{
			using(var db = OpenOrCreateDatabase())
			{
				var param = new SqliteParameter("@Url", url);
				
				var id = RunQueryAndReturnScalarValue<long>(SQL.SelectIdByUrl, db, new[] { param });
				return id == 0 ? null : new Nullable<Int32>((Int32)id);
			}	
		}

		public String GetUrlForId (int id)
		{
			using(var db = OpenOrCreateDatabase())
			{
				var param = new SqliteParameter("@Id", id);
				
				return RunQueryAndReturnScalarValue<String>(SQL.SelectUrlById, db, new[] { param });
			}
		}
		
		public void IncrementClickCountById (int id)
		{
			using(var db = OpenOrCreateDatabase())
			{
				var param = new SqliteParameter("@Id", id);
				
				using(var dbCmd = db.CreateCommand())
				{
					dbCmd.CommandText = "UPDATE links SET redirects = redirects + 1 WHERE id = @id";
					dbCmd.Parameters.Add(param);
					dbCmd.ExecuteNonQuery();
				}
				
			}
		}
		
		public dynamic GetDetailsFromId(int id)
		{
			var clicks = 0;
			using(var db = OpenOrCreateDatabase())
			{
				var param = new SqliteParameter("@Id", id);
				clicks = (int)RunQueryAndReturnScalarValue<long>(SQL.SelectClickCountById, db, new[] { param });
			}
			
			dynamic result =  new ExpandoObject();
			result.Url = GetUrlForId(id);
			result.Clicks = clicks;
			return result;
		}
			
		private IDbConnection OpenOrCreateDatabase()
		{
			var db = new SqliteConnection(String.Format(ConnStringWithOutUri, _dbUri));
			db.Open();
			InitDbIfRequired(db);
            return db;
		}

		
		private void InitDbIfRequired(IDbConnection db)
		{
			var tableCount = RunQueryAndReturnScalarValue<long>(SQL.CheckTableExists, db, new SqliteParameter[0]);
			if(tableCount == 0)
			{
				var cmd = db.CreateCommand();
				cmd.CommandText = SQL.CreateLinkTable;
				cmd.ExecuteNonQuery();
			}
		}
		
        private T RunQueryAndReturnScalarValue<T>(String sql, IDbConnection db, IEnumerable<SqliteParameter> sqlParams)
	    {
			
			IDbCommand dbCmd = null;
		
			try
			{
				dbCmd = db.CreateCommand();
				dbCmd.CommandText = sql;
			 	foreach(var param in sqlParams)
					dbCmd.Parameters.Add(param);
				
			    var retValue = dbCmd.ExecuteScalar();
				if(retValue != null && !(retValue is DBNull))
				{
					return (T)retValue;
				}
				else
				{
					return default(T);
				}
			}
			finally
			{
				if(dbCmd != null)
				{
					dbCmd.Dispose();
				}
				
			}
	    }
		 
	}
			                             
	static class SQL
	{
		public readonly static String CheckTableExists = @"SELECT COUNT(name) FROM sqlite_master WHERE type='table' AND name='links';";
		public readonly static String LinkTableName = @"links";
		public readonly static String CreateLinkTable = @"CREATE  TABLE ""links"" (""id"" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL , ""url"" TEXT NOT NULL  UNIQUE , ""redirects"" INTEGER NOT NULL )";
		public readonly static String InsertAndReturnId = @"INSERT INTO links (url, redirects) VALUES (@Url, 0); SELECT last_insert_rowid();";
		public readonly static String SelectUrlById = "SELECT url FROM links WHERE id=@Id";
		public readonly static String SelectIdByUrl = "SELECT id FROM links WHERE url=@Url";
		public readonly static String SelectClickCountById = "SELECT redirects FROM links WHERE id = @Id";
	}
			             
}

