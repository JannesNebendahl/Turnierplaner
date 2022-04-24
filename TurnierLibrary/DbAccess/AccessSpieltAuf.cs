using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace TurnierLibrary.DbAccess
{
    public class AccessSpieltAuf : SqliteDataAccess
    {
        public static void StoreSpieltAuf(int spielerId, int positionId)
        {
            string sql = "INSERT INTO SpieltAuf(SpielerId, PositionId)" +
                         "VALUES (@SpielerId, @PositionId);";

            using (var connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = sql;
                    command.Parameters.Add(new SQLiteParameter("@SpielerId", spielerId));
                    command.Parameters.Add(new SQLiteParameter("@PositionId", positionId));
                    var result = command.ExecuteNonQuery();
                    if (result <= 0)
                        throw new Exception("Can't store SpieltAuf Relation " + spielerId + "<->" + positionId);
                }
            }
        }
    }
}
