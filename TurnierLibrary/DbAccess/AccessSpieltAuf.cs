using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using TurnierLibrary.TabelModel;

namespace TurnierLibrary.DbAccess
{
    public class AccessSpieltAuf : SqliteDataAccess
    {
        public static void StoreSpieltAuf(SpieltAuf spieltAuf)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute(
                    "INSERT INTO SpieltAuf (SpielerId, PositionId) " +
                    "VALUES (@SpielerId, @PositionId)",
                    spieltAuf
                );
            }
        }
    }
}
