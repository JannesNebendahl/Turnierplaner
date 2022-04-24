using Dapper;
using DemoLibary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace TurnierLibrary
{
    public class AccessMannschaften : SqliteDataAccess
    {
        public static List<Mannschaft> LoadMannschaften()
        {
            string sql = "SELECT * " +
                         "FROM Mannschaften";
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<Mannschaft>(sql, new DynamicParameters());
                    return output.AsList();
                }
            }
            catch (Exception e)
            {
                Console.Error.Write(e.Message);
                return new List<Mannschaft>();
            }
        }

        public static void StoreMannschaft(Mannschaft mannschaft)
        {
            string sql = "INSERT INTO Mannschaften (Name, Kuerzel, Entstehungsjahr, Kapitan) " +
                         "VALUES (@Name, @Kuerzel, @Entstehungsjahr, @Kapitan);";

            try
            {
                using (var connection = new SQLiteConnection(LoadConnectionString()))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 0;
                        command.CommandText = sql;
                        command.Parameters.Add(new SQLiteParameter("@Name", mannschaft.Name));
                        command.Parameters.Add(new SQLiteParameter("@Kuerzel", mannschaft.Kuerzel));
                        command.Parameters.Add(new SQLiteParameter("@Entstehungsjahr", mannschaft.Entstehungsjahr));
                        command.Parameters.Add(new SQLiteParameter("@Kapitan", mannschaft.Kapitan));
                        var result = command.ExecuteNonQuery();
                        if (result <= 0)
                            throw new Exception("Can't store Mannschaft " + mannschaft.Name );
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.Write(e.Message);
            }
        }

        public static List<Mannschaft> LoadMannschaftenAlphabetical()
        {
            string sql = "SELECT * " +
                         "FROM Mannschaften " +
                         "ORDER BY Name ASC;";

            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<Mannschaft>(sql, new DynamicParameters());
                    return output.AsList();
                }
            }
            catch (Exception e)
            {
                Console.Error.Write(e.Message);
                return new List<Mannschaft>();
            }
        }

    }
}
