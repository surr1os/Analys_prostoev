using Analys_prostoev.Tables;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;


namespace Analys_prostoev
{

	public partial class MainWindow : System.Windows.Window
	{
		#region Data
		#region boolean
		static bool isStartImage = true;
		private bool isThreadRunning = false;
		private bool stopRequested = false;
		#endregion
		#region integer
		private int selectedInterval;
		#endregion
		IExportToExcel _excel;
		INewColumnsNames _columnsNames;
		ICancelMenuItemHendler _cancelMenuItemHendler;
		#endregion
		public MainWindow()
		{
			InitializeComponent();
			_excel = new ExportToExcel();
			_columnsNames = new NewColumnsNames();
			_cancelMenuItemHendler = new CancelMenuItemHehdler();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Edit_MenuItem.Visibility = Visibility.Collapsed;
			Delete_MenuItem.Visibility = Visibility.Collapsed;
			Cancel_MenuItem.Visibility = Visibility.Collapsed;

			startDatePicker.Value = DateTime.Today.Date.AddHours(0);
			GetRegionName();
			CreateSelectRowCB();
		}

		private void GetRegionName()
		{
			selectComboBox.Items.Add("Все участки");
			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();
				using (NpgsqlCommand selectCommand = new NpgsqlCommand(DBContext.selectQuery, connection))
				{
					using (NpgsqlDataReader reader = selectCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							selectComboBox.Items.Add(reader["region"].ToString());
						}
					}
				}
			}
		}

		private void CreateSelectRowCB()
		{
			selectRowComboBox.Items.Add("Все строки");
			selectRowComboBox.Items.Add("Классифицированные строки");
			selectRowComboBox.Items.Add("Неклассифицированные строки");
		}

		private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			DataRowView item = (DataRowView)DataGridTable.SelectedItem;
			if (item == null)
			{
				MessageBox.Show("Вы не выбрали простой!");
				return;
			}
			SimpleDeletionHandler simpleDeletion = new SimpleDeletionHandler();
			simpleDeletion.Delete();
		}

		public void CanselMenuItem_Click(object sender, RoutedEventArgs e)
		{
			_cancelMenuItemHendler.CancellationOfCategories(DataGridTable);
		}

		private void CreateMenuItem_Click(object sender, RoutedEventArgs e)
		{
			CreateTimeDown create = new CreateTimeDown();
			create.Show();
		}

		private void ChangeHistoryItem_Click(object sender, RoutedEventArgs e)
		{
			DataRowView item = (DataRowView)DataGridTable.SelectedItem;
			if (item == null)
			{
				MessageBox.Show("Вы не выбрали простой!");
				return;
			}
			else
			{
				ChangeHistory history = new ChangeHistory(Convert.ToInt32(item.Row.ItemArray[6]),
							   Convert.ToString(item.Row.ItemArray[5]), Convert.ToInt64(item.Row.ItemArray[0]));
				history.Show();
			}
		}

		private void ChangeMenuItem_Click(object sender, RoutedEventArgs e)
		{
			DataRowView item = (DataRowView)DataGridTable.SelectedItem;
			if (item == null)
			{
				MessageBox.Show("Вы не выбрали простой!");
				return;
			}
			else
			{
				ChangeTimeDown change = new ChangeTimeDown(Convert.ToInt64(item.Row.ItemArray[0]), Convert.ToDateTime(item.Row.ItemArray[1]),
							   Convert.ToDateTime(item.Row.ItemArray[2]), Convert.ToInt32(item.Row.ItemArray[6]),
							   Convert.ToString(item.Row.ItemArray[5]), Convert.ToString(item.Row.ItemArray[4]), Convert.ToString(item.Row.ItemArray[3]));

				change.Show();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			GetSortTable();
		}

		public void GetSortTable()
		{
			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				StringBuilder queryBuilder = new StringBuilder("SELECT \"Id\", date_start, date_finish, shifts, status, region, period," +
					" category_one, category_two, category_third, reason, created_at, change_at, is_manual FROM analysis WHERE 1=1 AND period >= 5");

				string original = queryBuilder.ToString();

				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

				if (selectComboBox.SelectedItem != null)
				{
					string selectedRegion = selectComboBox.SelectedItem.ToString();
					if (selectedRegion != "Все участки")
					{
						if (selectedRegion == "ХПТ" || selectedRegion == "ХПТР")
						{
							queryBuilder.Append(" AND region ILIKE @selectedRegion");
							parameters.Add(new NpgsqlParameter("selectedRegion", NpgsqlDbType.Varchar));
							parameters[parameters.Count - 1].Value = selectedRegion + " %";
						}
						else
						{
							queryBuilder.Append(" AND region = @selectedRegionCurrent");
							parameters.Add(new NpgsqlParameter("selectedRegionCurrent", NpgsqlDbType.Varchar));
							parameters[parameters.Count - 1].Value = selectedRegion;
						}
					}
				}

				if (selectRowComboBox.SelectedItem != null)
				{
					string rowSelect = selectRowComboBox.SelectedItem.ToString();
					if (rowSelect == "Классифицированные строки")
					{
						queryBuilder.Append(" AND category_one IS NOT NULL AND category_one <> '' AND category_two IS NOT NULL AND category_two <> '' AND category_third IS NOT NULL AND category_third <> ''");
					}
					else if (rowSelect == "Неклассифицированные строки")
					{
						queryBuilder.Append(" AND category_one IS NULL AND category_two IS NULL AND category_third IS NULL");
					}
				}

				if (startDatePicker.Value.HasValue)
				{
					queryBuilder.Append(" AND (date_start >= @startDate OR date_finish >= @startDate)");
					parameters.Add(new NpgsqlParameter("startDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
					parameters[parameters.Count - 1].Value = startDatePicker.Value.Value;
				}

				if (endDatePicker.Value.HasValue)
				{
					queryBuilder.Append(" AND (date_finish <= @endDate OR date_start <= @endDate)");
					parameters.Add(new NpgsqlParameter("endDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
					parameters[parameters.Count - 1].Value = endDatePicker.Value.Value;
				}

				string conclusion = queryBuilder.ToString();
				conclusion += " ORDER BY \"Id\" DESC";

				using (NpgsqlCommand command = new NpgsqlCommand(conclusion, connection))
				{
					command.Parameters.AddRange(parameters.ToArray());

					NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
					DataTable dataTable = new DataTable();
					adapter.Fill(dataTable);

					DataGridTable.ItemsSource = dataTable.DefaultView;

					_columnsNames.SetNewColumnNames(DataGridTable);
				}
				DBContext.queryString = conclusion;
			}
		}

		private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridCellInfo cellInfo = DataGridTable.CurrentCell;
			DataGridColumn column = cellInfo.Column;
			if (cellInfo.Column == null)
			{
				return;
			}
			else
			{
				var columnValue = column.Header.ToString();

				if (columnValue == "Категория Уровень 1" || columnValue == "Категория Уровень 2" || columnValue == "Категория Уровень 3" || columnValue == "Причина")
				{
					DataRowView rowView = (DataRowView)cellInfo.Item;
					long id = Convert.ToInt64(rowView["Id"]);
					string reason = rowView["reason"].ToString();
					string regionValue = rowView["region"].ToString();
					string categoryOneValue = rowView["category_one"].ToString();
					string categoryTwoValue = rowView["category_two"].ToString();
					string categoryThirdValue = rowView["category_third"].ToString();

					var newWindow = new CategoryHierarchy(regionValue, id, categoryOneValue, categoryTwoValue, categoryThirdValue);
					newWindow.ParentWindow = this;
					newWindow.Show();

					newWindow.categoryOneTextB.Text = categoryOneValue;
					newWindow.categoryTwoTextB.Text = categoryTwoValue;
					newWindow.categoryThirdTextB.Text = categoryThirdValue;
					newWindow.reasonTextB.Text = reason;

				}
			}
		}

		public void UpdateSelectedRowValues(string categoryOneValue, string categoryTwoValue, string categoryThirdValue, string reasonValue)
		{
			DataRowView selectedItem = DataGridTable.SelectedItem as DataRowView;
			if (selectedItem != null)
			{

				selectedItem["category_one"] = categoryOneValue;
				selectedItem["category_two"] = categoryTwoValue;
				selectedItem["category_third"] = categoryThirdValue;
				selectedItem["reason"] = reasonValue;

				using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
				{
					connection.Open();
					using (NpgsqlCommand updateCommand = new NpgsqlCommand(DBContext.updateQuery, connection))
					{
						updateCommand.Parameters.AddWithValue("categoryOne", categoryOneValue);
						updateCommand.Parameters.AddWithValue("categoryTwo", categoryTwoValue);
						updateCommand.Parameters.AddWithValue("categoryThird", categoryThirdValue);
						updateCommand.Parameters.AddWithValue("reason_new", reasonValue);

						long id = Convert.ToInt64(selectedItem["Id"]);
						updateCommand.Parameters.AddWithValue("Id", id);

						int rowsAffected = updateCommand.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							MessageBox.Show("Обновление значения выполнено успешно");
						}
						else
						{
							MessageBox.Show("Ошибка при обновлении значения");
						}
					}
				}
			}
		}

		public void ExportToExcel(string queryString)
		{
			List<Analysis> analysisList = _excel.GetAnalysisList(queryString);
			_excel.CreateExcelFile(analysisList);
		}

		private void Button_Click_Excel(object sender, RoutedEventArgs e)
		{
			if (DataGridTable.Items.Count == 0)
			{
				MessageBox.Show("Нельзя выгрузить в эксель пустые данные. Загрузите данные в таблицу.");
			}
			else
			{
				ExportToExcel(DBContext.queryString);
			}
		}

		private void DataGridTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			DataRowView item = (DataRowView)DataGridTable.SelectedItem;
			if (item != null)
			{
				var status = item.Row.ItemArray.Length > 4 && item.Row.ItemArray[4] != DBNull.Value ? (string)item.Row.ItemArray[4] : null;
				var categoryOne = item.Row.ItemArray.Length > 8 && item.Row.ItemArray[8] != DBNull.Value ? (string)item.Row.ItemArray[8] : null;
				var categoryTwo = item.Row.ItemArray.Length > 9 && item.Row.ItemArray[9] != DBNull.Value ? (string)item.Row.ItemArray[9] : null;
				var categoryThird = item.Row.ItemArray.Length > 10 && item.Row.ItemArray[10] != DBNull.Value ? (string)item.Row.ItemArray[10] : null;
				var manual = item.Row.ItemArray.Length > 13 && item.Row.ItemArray[13] != DBNull.Value ? (bool)item.Row.ItemArray[13] : (bool?)null;

				if (categoryOne == null || categoryTwo == null || categoryThird == null)
				{
					Cancel_MenuItem.Visibility = Visibility.Collapsed;
					Edit_MenuItem.Visibility = Visibility.Collapsed;
				}
				if (manual == false)
				{
					Delete_MenuItem.Visibility = Visibility.Collapsed;
				}
				if (status == "Не согласованно" && manual == false)
				{
					Edit_MenuItem.Visibility = Visibility.Visible;
				}
				if (manual == true)
				{
					Delete_MenuItem.Visibility = Visibility.Visible;
				}
				if (categoryOne != null || categoryTwo != null || categoryThird != null)
				{
					Edit_MenuItem.Visibility = Visibility.Visible;
					Cancel_MenuItem.Visibility = Visibility.Visible;
				}
				else
				{
					Edit_MenuItem.Visibility = Visibility.Collapsed;
				}
			}
			else
			{
				Edit_MenuItem.Visibility = Visibility.Visible;
			}
		}

		private void toggle_Click(object sender, RoutedEventArgs e)
		{
			ToggleLogic(ref isStartImage);
		}

		private void ToggleLogic(ref bool isStartImage)
		{
			switch (isStartImage)
			{
				case false:
					isStartImage = true;
					isThreadRunning = false;
					stopRequested = true;
					toggleButton.Content = FindResource("Play");
					break;

				case true:
					isStartImage = false;
					if (!isThreadRunning)
					{
						toggleButton.Content = FindResource("Stop");
						isThreadRunning = true;
						stopRequested = false;
						selectedInterval = GetSelectedInterval();
						Thread thread = new Thread(Timer);
						thread.IsBackground = true;
						thread.Start();
					}
					break;
			}
		}

		void Timer()
		{
			while (isThreadRunning)
			{
				Dispatcher.Invoke(GetSortTable);
				Thread.Sleep(selectedInterval);
			}
		}

		private int GetSelectedInterval()
		{
			switch (TimerValue.SelectedIndex)
			{
				case 0: return 5 * 60 * 1000;
				case 1: return 10 * 60 * 1000;
				case 2: return 15 * 60 * 1000;
				case 3: return 30 * 60 * 1000;
				case 4: return 60 * 60 * 1000;
				default: return 5 * 60 * 1000;
			}
		}
	}
}