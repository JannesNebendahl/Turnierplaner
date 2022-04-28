using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace TurnierLibrary.DbAccess
{
    public class AccessSpielplan : SqliteDataAccess
    {
        public static void LoadTableInDataGrid(System.Windows.Controls.DataGrid dataGrid, string sql)
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
