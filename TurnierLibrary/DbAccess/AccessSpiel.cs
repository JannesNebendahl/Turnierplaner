using Dapper;
using DemoLibary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace TurnierLibrary
{
    public class AccessSpiel : SqliteDataAccess
    {
        public static List<Spiel> LoadSpiele()
        {
            string sql = "SELECT * " +
                         "FROM Spiel";
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<Spiel>(sql, new DynamicParameters());
                    return output.AsList();
                }
            }
            catch (Exception e)
            {
                Console.Error.Write(e.Message);
                return new List<Spiel>();
            }
        }

        public static void StoreSpiel(Spiel spiel)
        {
            string sql = "INSERT INTO Spiel(Datum, Spieltag, Zuschaueranzahl, HeimmannschaftsID, AuswaertsmannschaftID) " +
                         "VALUES (@Datum, @Spieltag, @Zuschaueranzahl, @HeimmannschaftsID, @AuswaertsmannschaftID);" +
                         "SELECT last_insert_rowid();";

            try
            {
                using (var connection = new SQLiteConnection(LoadConnectionString()))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 0;
                        command.CommandText = sql;
                        command.Parameters.Add(new SQLiteParameter("@Datum", spiel.Datum));
                        command.Parameters.Add(new SQLiteParameter("@Spieltag", spiel.Spieltag));
                        command.Parameters.Add(new SQLiteParameter("@Zuschaueranzahl", spiel.Zuschauerzahl));
                        command.Parameters.Add(new SQLiteParameter("@HeimmannschaftsID", spiel.HeimmannschaftsID));
                        command.Parameters.Add(new SQLiteParameter("@AuswaertsmannschaftID", spiel.AuswaertsmannschaftsID));
                        var result = command.ExecuteNonQuery();
                        if (result <= 0)
                            throw new Exception("Can't store Spiel ");
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.Write(e.Message);
            }
        }
        public static List<Spiel> LoadGamesOfDate(DateTime date)
        {
            string sql = "SELECT s.Id, a.Name as Heim, b.Name as Gast, s.HeimmannschaftsId, s.AuswaertsmannschaftsId " +
                         "From Spiel s, Mannschaften a, Mannschaften b " +
                         "WHERE a.Id == s.HeimmannschaftsId AND s.AuswaertsmannschaftsId == b.Id AND strftime('%Y', Datum) IN ('" + date.Year + "') AND strftime('%m', Datum) IN ('" + date.Month.ToString("00") + "') AND strftime('%d', Datum) IN ('" + date.Day.ToString("00") + "') " +
                         "ORDER BY a.Name";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Spiel>(sql, new DynamicParameters());
                return output.AsList();
            }
        }

    }
}
