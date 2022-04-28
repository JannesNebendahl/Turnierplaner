using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace TurnierLibrary
{
    public class AccessFairnesstabelle : SqliteDataAccess
    {
        public static int? StoreKarte(Fairnesstabelle fairnesstabelle)
        {
            int? id = null;
            string sql = "INSERT INTO Fairnesstabelle(SpielerId, Karte, SpielId) " +
                         "VALUES (@SpielerId, @Karte, @SpielId);";

            using (var connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = sql;
                    command.Parameters.Add(new SQLiteParameter("@SpielerId", fairnesstabelle.SpielerId));
                    command.Parameters.Add(new SQLiteParameter("@Karte", fairnesstabelle.Karte));
                    command.Parameters.Add(new SQLiteParameter("@SpielId", fairnesstabelle.SpielID));
                    var result = command.ExecuteScalar();
                    id = Convert.ToInt32(result);
                }
            }
            return id;
        }

        public static List<Fairnesstabelle> LoadFairnesstabelle()
        {
            string sql = "SELECT * " +
                         "From Fairnesstabelle;";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Fairnesstabelle>(sql, new DynamicParameters());
                return output.AsList();
            }
        }
    }
}
