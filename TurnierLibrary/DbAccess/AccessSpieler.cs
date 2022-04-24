using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace TurnierLibrary
{
    public class AccessSpieler : SqliteDataAccess
    {
        public static int? StoreSpieler(Spieler spieler)
        {
            int? id = null;
            string sql = "INSERT INTO Spieler(Vorname, Nachname, Trikotnummer, MannschaftsId) " +
                         "VALUES (@Vorname, @Nachname, @Trikotnummer, @MannschaftsId);" +
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
                        command.Parameters.Add(new SQLiteParameter("@Vorname", spieler.Vorname));
                        command.Parameters.Add(new SQLiteParameter("@Nachname", spieler.Nachname));
                        command.Parameters.Add(new SQLiteParameter("@Trikotnummer", spieler.Trikotnummer));
                        command.Parameters.Add(new SQLiteParameter("@MannschaftsId", spieler.MannschaftsId));
                        var result = command.ExecuteScalar();
                        id = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.Write(e.Message);
            }
            return id;
        }

        public static List<Spieler> LoadSpielerAlphabetical()
        {
            string sql = "SELECT * " +
                         "FROM Spieler " +
                         "ORDER BY Vorname ASC;";

            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<Spieler>(sql, new DynamicParameters());
                    return output.AsList();
                }
            }
            catch (Exception e)
            {
                Console.Error.Write(e.Message);
                return new List<Spieler>();
            }
        }
    }
}
