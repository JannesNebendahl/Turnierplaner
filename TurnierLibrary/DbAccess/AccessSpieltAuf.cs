using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace TurnierLibrary.DbAccess
{
    public class AccessSpieltAuf : SqliteDataAccess
    {
        public static void AddRelation(int spielerId, int positionId)
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

        public static List<Position> GetPositions(Spieler spieler)
        {
            string sql = "SELECT P.*" +
                         "FROM Positionen P " +
                         "LEFT JOIN SpieltAuf SA on P.Id = SA.PositionId " +
                         "WHERE SA.SpielerId == @Id; ";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Id", spieler.Id);
                var output = cnn.Query<Position>(sql, dynamicParameters).AsList();
                return output;
            }
        }

        public static void DeleteRelation(int spielerId, int positionId)
        {
            string sql = "DELETE FROM SpieltAuf " +
                         "WHERE (SpielerId == @SpielerId AND PositionId == @PositionId);";

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
                        throw new Exception("Can't delete Relation " + spielerId + "<->" + positionId);
                }
            }
        }
    }
}
