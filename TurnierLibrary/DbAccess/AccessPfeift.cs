using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace TurnierLibrary.DbAccess
{
    public class AccessPfeift : SqliteDataAccess
    {
        public static void AddRelation(int spielId, int schiedsrichterId)
        {
            string sql = "INSERT INTO Pfeift(SchiedsrichterId, SpielId)" +
                         "VALUES (@SchiedsrichterId, @SpielId);";

            using (var connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = sql;
                    command.Parameters.Add(new SQLiteParameter("@SchiedsrichterId", schiedsrichterId));
                    command.Parameters.Add(new SQLiteParameter("@SpielId", spielId));
                    var result = command.ExecuteNonQuery();
                    if (result <= 0)
                        throw new Exception("Can't store Pfeift Relation " + spielId + "<->" + schiedsrichterId);
                }
            }
        }

        public static int? CleanPfeift()
        {
            int? count = null;
            string sql = "DELETE " +
                         "FROM Pfeift;";

            using (var connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = sql;
                    var result = command.ExecuteNonQuery();
                    count = Convert.ToInt32(result);
                }
            }

            return count;
        }
    }
}
