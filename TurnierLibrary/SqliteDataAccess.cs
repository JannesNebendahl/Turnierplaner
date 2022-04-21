using Dapper;
using DemoLibary;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace TurnierLibrary
{
    public class SqliteDataAccess
    {
        public static List<MannschaftModel> LoadMannschaften()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<MannschaftModel>("SELECT * FROM Mannschaften", new DynamicParameters());
                return output.AsList();
            }
        }

        public static void SaveMannschaft(MannschaftModel mannschaft)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("INSERT INTO Mannschaften (Name) VALUES (@Name)", mannschaft);
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
