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
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Mannschaft>("SELECT * FROM Mannschaften", new DynamicParameters());
                return output.AsList();
            }
        }

        public static void StoreMannschaft(Mannschaft mannschaft)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("INSERT INTO Mannschaften (Name, Kuerzel, Entstehungsjahr) VALUES (@Name, @Kuerzel, @Entstehungsjahr)", mannschaft);
            }
        }

    }
}
