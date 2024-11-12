using Npgsql;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Analys_prostoev
{
	/// <summary>
	/// Представляет собой класс для отмены категорий простоя.
	/// </summary>
	public class CanselMenuItemHehdler : ICancelMenuItemHendler
	{
		MainWindow main = Application.Current.MainWindow as MainWindow;
		public void CancellationOfCategories(DataGrid dataGrid)
		{
			DataRowView item = (DataRowView)dataGrid.SelectedItem;
			if (item != null)
			{
				CancellationCategoriesLogic(item);
			}
			else
			{
				MessageBox.Show("Вы не выбрали простой!");
				return;
			}
		}

		private void CancellationCategoriesLogic(DataRowView item)
		{
			long id = Convert.ToInt64(item["Id"]);
			string regionValue = item["region"].ToString();

			MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите Аннулировать простой {id}?", "Аннулирование порстоя", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				IGetHistory changeHistory = new GlobalChangeHistory(regionValue, id);

				using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
				{
					connection.Open();
					using (NpgsqlCommand cancellationCommand = new NpgsqlCommand(DBContext.cancellationQuery, connection))
					{
						cancellationCommand.Parameters.AddWithValue("@Id", id);
						cancellationCommand.ExecuteNonQuery();

						using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertHistory, connection))
						{
							changeHistory.AddHistory(insertCommand, $"Простой аннулирован");
						}
					}
				}
				main.GetTable();
			}
			else
				return;
		}
	}
}
