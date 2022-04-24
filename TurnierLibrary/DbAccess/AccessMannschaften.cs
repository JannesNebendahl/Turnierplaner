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
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<Mannschaft>("SELECT * FROM Mannschaften", new DynamicParameters());
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
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("INSERT INTO Mannschaften (Name, Kuerzel, Entstehungsjahr) VALUES (@Name, @Kuerzel, @Entstehungsjahr)", mannschaft);
                }
            }
            catch(Exception e) 
            {
                Console.Error.Write(e.Message);
            }
        }

    }
}
