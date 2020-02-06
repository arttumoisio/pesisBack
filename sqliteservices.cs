using System;
using Microsoft.Data.Sqlite;

namespace pesisBackend
{
  public class SqliteService
  {
      public SqliteConnection connectorF(){
        var connectionStringBuilder = new SqliteConnectionStringBuilder();

        //Use DB in project directory.  If it does not exist, create it:
        connectionStringBuilder.DataSource = "./sqlite/pesisKanta.db";

        return new SqliteConnection(connectionStringBuilder.ConnectionString);
        
        
      }
  }
    
}