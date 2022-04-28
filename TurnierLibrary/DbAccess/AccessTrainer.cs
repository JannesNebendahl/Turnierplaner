using Dapper;
using DemoLibary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;


namespace TurnierLibrary.DbAccess
{
    public class AccessTrainer : SqliteDataAccess
    {
        public static List<Trainer> LoadTrainer()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Trainer>("SELECT * FROM Trainer", new DynamicParameters());
                return output.AsList();
            }
        }

        public static void StoreTrainer(Trainer trainer)
        {
            string sql = "INSERT INTO Trainer(Vorname, Nachname, Amtsantritt, Mannschaft) " +
                         "VALUES (@Vorname, @Nachname, @Amtsantritt, @Mannschaft);" +
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
                        command.Parameters.Add(new SQLiteParameter("@Vorname", trainer.Vorname));
                        command.Parameters.Add(new SQLiteParameter("@Nachname", trainer.Nachname));
                        command.Parameters.Add(new SQLiteParameter("@Amtsantritt", trainer.Amtsantritt));
                        command.Parameters.Add(new SQLiteParameter("@Mannschaft", trainer.Mannschaft));
                        var result = command.ExecuteNonQuery();
                        if (result <= 0)
                            throw new Exception("Can't store Trainer");
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
