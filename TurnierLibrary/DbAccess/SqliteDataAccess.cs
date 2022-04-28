using Dapper;
using DemoLibary;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Windows.Controls;

namespace TurnierLibrary
{
    public class SqliteDataAccess
    {
        protected static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public static void LoadTableInDataGrid(DataGrid dataGrid, string sql)
        {
            using (var connection = new SQLiteConnection(LoadConnectionString()))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();

                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable("Tabelle");
                dataAdapter.Fill(dt);
                dataGrid.ItemsSource = dt.DefaultView;
                dataAdapter.Update(dt);
                connection.Close();
            }
        }
    }
}
