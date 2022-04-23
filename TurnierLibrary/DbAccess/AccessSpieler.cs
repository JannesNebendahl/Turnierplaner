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
        public static void StoreSpieler(Spieler spieler)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("INSERT INTO Spieler (Vorname, Nachname, Trikotnummer) VALUES (@Vorname, @Nachname, @Trikotnummer)", spieler);
                }
            }
            catch (Exception e)
            {
                Console.Error.Write(e.Message);
            }
        }
    }
}
