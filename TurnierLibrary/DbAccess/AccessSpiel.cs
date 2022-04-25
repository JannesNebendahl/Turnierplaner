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
                        command.Parameters.Add(new SQLiteParameter("@Zuschaueranzahl", spiel.Zuschaueranzahl));
                        command.Parameters.Add(new SQLiteParameter("@HeimmannschaftsID", spiel.Heimmanschaft));
                        command.Parameters.Add(new SQLiteParameter("@AuswaertsmannschaftID", spiel.Auswaertsmannschaft));
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

    }
}
