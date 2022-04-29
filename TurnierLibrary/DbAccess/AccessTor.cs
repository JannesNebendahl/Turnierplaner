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
        //TODO: Gibt die SpielId, die Heimmannschaft als Heim und die Gastmannschaft als Gast sortiert nach der Heimmannschaft aus der Tabelle spiel aus
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

        //TODO: Gibt den Vornamen und den Nachnamen sowie die Anzahl an Toren aller Spieler aus. Die Liste soll nach absteigender Toranzahl sortiert werden. Zudem soll eine Nummerierung als Platzierung durch SQLite erfolgen. Da ein Tor nicht zwingend einem Spieler zugeordnet werden muss, müssen die Tore mit SpielerId <null> entfernt werden.
        public static List<Tor> LoadTorschuetzenliste(bool limit, string condition)
        {
            string sql = "SELECT row_number()  over (ORDER BY Toranzahl DESC) as Platzierung, * FROM ( " +
                         "SELECT s.Vorname, s.Nachname as Nachname, Count(*) as Toranzahl " +
                         "From Tor t, Spieler s " +
                         "WHERE Spieler NOT NULL AND t.Spieler == s.Id AND t.Typ != 2 ";

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

        //TODO: Gibt die durchschnittlichen Tore pro Spiel aus
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

        //TODO: Speichert ein Tor in der Tabelle Tor ab
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