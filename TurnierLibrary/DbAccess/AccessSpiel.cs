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
            string sql = "INSERT INTO Spiel(Datum, Spieltag, Zuschaueranzahl, HeimmannschaftsId, AuswaertsmannschaftsId) " +
                         "VALUES (@Datum, @Spieltag, @Zuschaueranzahl, @HeimmannschaftsId, @AuswaertsmannschaftsId);" +
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
                    command.Parameters.Add(new SQLiteParameter("@HeimmannschaftsId", spiel.Heimmanschaft));
                    command.Parameters.Add(new SQLiteParameter("@AuswaertsmannschaftsId", spiel.Auswaertsmannschaft));
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
                    else if(Convert.ToInt32(result) == 0)
                    {
                        ret = false;
                    }
                }
            }

            if(ret == null)
                throw new Exception("Unexpected behavior: IdExist returns null");
            return ret;
        }
    }
}
