using Analys_prostoev.Data;
using AnalysisDowntimes;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Analys_prostoev
{
	/// <summary>
	/// Логика взаимодействия для CategoryHistory.xaml
	/// </summary>
	public partial class CategoryHistory : Window
	{
		public DateTime DateFrom { get; set; }
		public DateTime DateTo { get; set; }
		ObservableCollection<CategorysHistory> categoryHistories { get; set; }

		public CategoryHistory()
		{
			InitializeComponent();
			categoryHistories = new ObservableCollection<CategorysHistory>();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			startDatePicker.Value = DateTime.Today.Date.AddHours(0);
			AddTypes();
			GetAllHistory((string)TypesComboBox.SelectedValue, startDatePicker.Value);
		}

		private void AddTypes()
		{
			TypesComboBox.Items.Add(CategoryChangeTypes.All);
			TypesComboBox.Items.Add(CategoryChangeTypes.Created);
			TypesComboBox.Items.Add(CategoryChangeTypes.Changed);
			TypesComboBox.Items.Add(CategoryChangeTypes.Removed);
		}

		private void GetAllHistory(string type, DateTime? dateFrom, DateTime? dateTo = null)
		{
			var query = DBContext.GetAllCategoryHistory(type ,dateFrom, dateTo);

			if (query != null)
			{
				using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
				{
					connection.Open();

					var results = ExecuteQuery(query, connection);
					categoryHistories.Clear();

					foreach (var result in results)
					{
						categoryHistories.Add(result);
					}

					HistoryTable.ItemsSource = categoryHistories;
				}
			}

		}

		private IEnumerable<CategorysHistory> ExecuteQuery(string query, NpgsqlConnection connection)
		{
			var records = new List<CategorysHistory>();

			using (var command = new NpgsqlCommand(query, connection))
			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					records.Add(new CategorysHistory
					{
						Id = Guid.Parse(reader["id"].ToString()),
						Title = reader["title"].ToString(),
						CreatedDate = Convert.ToDateTime(reader["created_date"]),
						Type = reader["type"].ToString(),
					});
				}
			}

			return records;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void TypesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			GetAllHistory((string)TypesComboBox.SelectedValue, startDatePicker.Value, finishDatePicker.Value);
		}
	}
}
 