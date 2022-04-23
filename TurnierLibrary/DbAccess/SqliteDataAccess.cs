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
        protected static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
