using Dapper;
using DemoLibary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace TurnierLibrary
{
    public class AccessTor : SqliteDataAccess
    {
        public static List<Spiel> LoadSpieltag(DateTime date)
        {
            string sql = "SELECT s.Id, a.Name as Heim, b.Name as Gast " +
                         "From Spiel s, Mannschaften a, Mannschaften b " +
                         "WHERE a.Id == s.HeimmannschaftsId AND s.AuswaertsmannschaftsId == b.Id AND strftime('%Y', Datum) IN ('" + date.Year + "') AND strftime('%m', Datum) IN ('" + date.Month.ToString("00") + "') AND strftime('%d', Datum) IN ('" + date.Day.ToString("00") + "') " +
                         "ORDER BY a.Name";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Spiel>(sql, new DynamicParameters());
                return output.AsList();
            }
        }

        public static List<Tor> LoadTorschuetzenliste(bool limit, string condition)
        {
            string sql = "SELECT row_number()  over (ORDER BY Toranzahl DESC) as Platzierung, * FROM ( " +
                         "SELECT s.Vorname, s.Nachname as Nachname, Count(*) as Toranzahl " +
                         "From Tor t, Spieler s " +
                         "WHERE Spieler NOT NULL AND t.Spieler == s.Id AND t.Typ != 'Eigentor' ";

            switch (condition)
            {
                case "Elfmeter":
                    sql = sql + " AND t.Typ == 3 ";
                    break;
                default:
                    break;
            }
            sql = sql + "GROUP BY t.Spieler " +
                        "ORDER BY t.Spieler desc )"; ;

            if (limit)
            {
                sql = sql + " limit 1";
            }
                         

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Tor>(sql, new DynamicParameters());
                return output.AsList();
            }
        }

        public static List<Tor> LoadavgToreproSpiel()
        {
            string sql = "SELECT avg(Anzahl) as avgSpiel " +
                         "From (SELECT Count(Tor.SpielID) as Anzahl " +
                         "FROM Tor " +
                         "GROUP BY SpielID)";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Tor>(sql, new DynamicParameters());
                return output.AsList();
            }
        }

        public static void StoreTor(Tor tor)
        {
            string sql = "INSERT INTO Tor (TorID, Zeitstempel, Spieler, Mannschaft, Typ, SpielID) " +
                         "VALUES (@TorID, @Zeitstempel, @Spieler, @Mannschaft, @Typ, @SpielID);";

            using (var connection = new SQLiteConnection(LoadConnectionString()))
            {
                
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = sql;
                    command.Parameters.Add(new SQLiteParameter("@TorID", tor.TorId));
                    command.Parameters.Add(new SQLiteParameter("@Zeitstempel", tor.Zeitstempel));
                    command.Parameters.Add(new SQLiteParameter("@Spieler", tor.Spieler));
                    command.Parameters.Add(new SQLiteParameter("@Mannschaft", tor.Mannschaft));
                    command.Parameters.Add(new SQLiteParameter("@Typ", tor.Typ));
                    command.Parameters.Add(new SQLiteParameter("@SpielID", tor.SpielID));
                    var result = command.ExecuteNonQuery();
                }
            }
        }
    }
}