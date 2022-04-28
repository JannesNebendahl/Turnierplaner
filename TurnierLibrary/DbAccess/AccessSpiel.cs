using Dapper;
using DemoLibary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Windows.Controls;

namespace TurnierLibrary
{
    public class AccessSpiel : SqliteDataAccess
    {
        public static List<Spiel> LoadSpiele()
        {
            string sql = "SELECT * " +
                         "FROM Spiel";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Spiel>(sql, new DynamicParameters());
                return output.AsList();
            }
        }

        public static int? StoreSpiel(Spiel spiel)
        {
            int? id = null;
            string sql = "INSERT INTO Spiel(Datum, Spieltag, Zuschaueranzahl, HeimmannschaftsID, AuswaertsmannschaftsID) " +
                         "VALUES (@Datum, @Spieltag, @Zuschaueranzahl, @HeimmannschaftsID, @AuswaertsmannschaftsID);" +
                         "SELECT last_insert_rowid();";

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
                    command.Parameters.Add(new SQLiteParameter("@HeimmannschaftsID", spiel.HeimmannschaftsId));
                    command.Parameters.Add(new SQLiteParameter("@AuswaertsmannschaftsID", spiel.AuswaertsmannschaftsId));
                    var result = command.ExecuteScalar();
                    id = Convert.ToInt32(result);
                }
            }

            return id;
        }

        public static int? CountSpiele()
        {
            int? count = null;
            string sql = "SELECT COUNT(*) " +
                         "FROM Spiel;";

            using (var connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = sql;
                    var result = command.ExecuteScalar();
                    count = Convert.ToInt32(result);
                }
            }

            return count;
        }

        public static int? CleanSpiele()
        {
            int? count = null;
            string sql = "DELETE " +
                         "FROM Spiel;";

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

        public static bool? IdExist(int Id)
        {
            bool? ret = null;

            string sql = "SELECT COUNT(*) " +
                         "FROM Spiel;" +
                         "WHERE Id==@Id;";

            using (var connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = sql;
                    command.Parameters.Add(new SQLiteParameter("@Id", Id));
                    var result = command.ExecuteScalar();
                    if (Convert.ToInt32(result) > 0)
                        ret = true;
                    else if (Convert.ToInt32(result) == 0)
                    {
                        ret = false;
                    }
                }
            }

            if (ret == null)
                throw new Exception("Unexpected behavior: IdExist returns null");
            return ret;
        }

        public static List<Spiel> LoadGamesOfDate(DateTime date)
        {
            string sql = "SELECT s.Id, a.Name as Heim, b.Name as Gast, s.HeimmannschaftsID, s.AuswaertsmannschaftsID " +
                         "From Spiel s, Mannschaften a, Mannschaften b " +
                         "WHERE a.Id == s.HeimmannschaftsID AND s.AuswaertsmannschaftsID == b.Id AND strftime('%Y', Datum) IN ('" + date.Year + "') AND strftime('%m', Datum) IN ('" + date.Month.ToString("00") + "') AND strftime('%d', Datum) IN ('" + date.Day.ToString("00") + "') " +
                         "ORDER BY a.Name";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Spiel>(sql, new DynamicParameters());
                return output.AsList();
            }
        }
    }
}
