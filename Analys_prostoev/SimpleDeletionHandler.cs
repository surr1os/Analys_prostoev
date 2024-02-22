using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace Analys_prostoev
{
    public class SimpleDeletionHandler
    {
        MainWindow main = Application.Current.MainWindow as MainWindow;
        public void Delete()
        {
            if (main.DataGridTable.SelectedItems.Count == 1)
            {
                DataRowView item = (DataRowView)main.DataGridTable.SelectedItem;
                long id = (long)item.Row["Id"];

                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите Удалить простой {id}?", "Удаление порстоя", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteLogic(id);
                    MessageBox.Show($"Запись {id}");
                }
                else
                    return;
            }
            else if (main.DataGridTable.SelectedItems.Count > 1)
            {
                List<long> listItem = new List<long>();
                foreach (var selectedItem in main.DataGridTable.SelectedItems)
                {
                    DataRowView oneItem = (DataRowView)selectedItem;
                    listItem.Add((long)oneItem.Row["Id"]);
                }
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите Удалить {main.DataGridTable.SelectedItems.Count} записей?", "удаление порстоя", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (var id in listItem)
                        DeleteLogic(id);
                    MessageBox.Show($"Удалено записей {listItem.Count()}");
                }
                else
                    return;
            }
        }

        private void DeleteLogic(long id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
            {
                connection.Open();

                DBContext.deleteQuery += $" \"Id\" = {id}";

                using (NpgsqlCommand deleteCommand = new NpgsqlCommand(DBContext.deleteQuery, connection))
                {
                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        DBContext.deleteQuery = "";
                        DBContext.deleteQuery = $"DELETE FROM analysis WHERE";
                    }
                }

                main.GetSortTable();
            }
        }
    }
}
