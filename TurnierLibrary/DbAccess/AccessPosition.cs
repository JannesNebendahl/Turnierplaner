using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace TurnierLibrary.DbAccess
{
    public class AccessPosition : SqliteDataAccess
    {
        public static List<Position> LoadPositionen()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Position>("SELECT * FROM Positionen", new DynamicParameters());
                return output.AsList();
            }
        }
    }
}
