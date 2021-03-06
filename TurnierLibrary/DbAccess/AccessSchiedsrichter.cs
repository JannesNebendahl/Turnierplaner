using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using TurnierLibrary.TabelModel;

namespace TurnierLibrary.DbAccess
{
    public class AccessSchiedsrichter : SqliteDataAccess
    {
        //TODO: Speichert einen Schiedsrichter ab.
        public static void StoreSchiedsrichter(Schiedsrichter schiedsrichter)
        {
            string sql = "INSERT INTO Schiedsrichter (Vorname, Nachname) " +
                         "VALUES (@Vorname, @Nachname);";

            using (var connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = sql;
                    command.Parameters.Add(new SQLiteParameter("@Vorname", schiedsrichter.Vorname));
                    command.Parameters.Add(new SQLiteParameter("@Nachname", schiedsrichter.Nachname));
                    var result = command.ExecuteNonQuery();
                    if (result <= 0)
                        throw new Exception("Can't store Schiedsrichter " + schiedsrichter.Name);
                }
            }
        }

        //TODO: Gibt Schiedsrichter in alphabetischer Reihenfolge aus
        public static List<Schiedsrichter> LoadAlphabetical()
        {
            string sql = "SELECT * " +
                         "FROM Schiedsrichter " +
                         "ORDER BY Vorname ASC;";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Schiedsrichter>(sql, new DynamicParameters());
                return output.AsList();
            }
        }
    }
}
