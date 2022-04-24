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
            int? id;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                using(var cmd = cnn.CreateCommand())
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandText =   "INSERT INTO Spieler (Vorname, Nachname, Trikotnummer)" +
                                        "VALUES (@Vorname, @Nachname, @Trikotnummer);" +
                                        "SELECT last_insert_rowid();";
                    cmd.Parameters.Add(new SQLiteParameter("@Vorname", spieler.Vorname));
                    cmd.Parameters.Add(new SQLiteParameter("@Nachname", spieler.Nachname));
                    cmd.Parameters.Add(new SQLiteParameter("@Trikotnummer", spieler.Trikotnummer));
                    var result = cmd.ExecuteScalar();
                    id = Convert.ToInt32(result);
                }
            }
            return id;
        }
    }
}
