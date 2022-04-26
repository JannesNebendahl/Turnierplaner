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

        public static void StoreSpiel(Spiel spiel)
        {
            string sql = "INSERT INTO Spiel(Datum, Spieltag, Zuschauerzahl, HeimmannschaftsId, AuswaertsmannschaftsId) " +
                         "VALUES (@Datum, @Spieltag, @Zuschauerzahl, @HeimmannschaftsId, @AuswaertsmannschaftsId);" +
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
                    command.Parameters.Add(new SQLiteParameter("@Zuschauerzahl", spiel.Zuschauerzahl));
                    command.Parameters.Add(new SQLiteParameter("@HeimmannschaftsId", spiel.Heimmanschaft));
                    command.Parameters.Add(new SQLiteParameter("@AuswaertsmannschaftsId", spiel.Auswaertsmannschaft));
                    var result = command.ExecuteNonQuery();
                    if (result <= 0)
                        throw new Exception("Can't store Spiel ");
                }
            }
        }

    }
}
