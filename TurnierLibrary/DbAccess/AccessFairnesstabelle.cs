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
            string sql = "SELECT row_number()  over (ORDER BY Punkte DESC) as Platzierung, * " +
                         "FROM ( " +
                         "SELECT Mannschaft, Gelb, Rot, Gelb + Rot * 3 as Punkte " +
                         "FROM ( " +
                         "SELECT M.Name AS Mannschaft, " +
                         "sum(F.Karte == 'Gelbe Karte' AND F.SpielerId == S.Id AND S.MannschaftsId == M.Id) AS Gelb, sum(F.Karte == 'Rote Karte' AND F.SpielerId == S.Id AND S.MannschaftsId == M.Id)  AS Rot " +
                         "FROM Fairnesstabelle F, Spieler S, Mannschaften M " +
                         "GROUP BY m.Name))";

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Fairnesstabelle>(sql, new DynamicParameters());
                return output.AsList();
            }
        }
    }
}
