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
            string sql = "SELECT * " +
                         "FROM Positionen";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Position>(sql, new DynamicParameters());
                return output.AsList();
            }


        }
    }
}
