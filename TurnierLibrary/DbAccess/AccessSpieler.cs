using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Windows.Controls;

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

            return id;
        }

        public static List<Spieler> LoadSpielerAlphabetical()
        {
            string sql = "SELECT * " +
                         "FROM Spieler " +
                         "ORDER BY Vorname ASC;";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Spieler>(sql, new DynamicParameters());
                return output.AsList();
            }
        }

        public static List<Spieler> LoadSpielerNameAlphabetical()
        {
            string sql = "SELECT Vorname, Nachname, ID " +
                         "FROM Spieler;";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Spieler>(sql, new DynamicParameters());
                return output.AsList();
            }
        }

        public static List<Spieler> LoadSpielerAlphabeticalFromMannschaft(int? ID)
        {
            string sql = "SELECT Vorname, Nachname,  Id " +
                         "FROM Spieler " +
                         "WHERE MannschaftsID == " + ID +
                         " ORDER BY Vorname ASC;";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Spieler>(sql, new DynamicParameters());
                return output.AsList();
            }
        }

        public static void ChangeMannschaft(int spielerId, int newMannschaftsId)
        {
            string sql = "UPDATE Spieler " +
                         "SET MannschaftsId = @newMannschaftsId " +
                         "WHERE Id == @SpielerId; ";

            using (var connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = sql;
                    command.Parameters.Add(new SQLiteParameter("@newMannschaftsId", newMannschaftsId));
                    command.Parameters.Add(new SQLiteParameter("@SpielerId", spielerId));
                    var result = command.ExecuteNonQuery();
                    if (result <= 0)
                        throw new Exception("Can't change MannschaftsId of Mannschaft (Id=" + spielerId + ") to MannschaftsId:" + newMannschaftsId);
                }
            }
        }

        
    }
}
