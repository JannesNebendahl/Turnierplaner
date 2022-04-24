using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using TurnierLibrary.TabelModel;

namespace TurnierLibrary.DbAccess
{
    public class AccessSchiedsrichter : SqliteDataAccess
    {
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
    }
}
