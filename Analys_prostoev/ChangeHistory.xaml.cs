using Npgsql;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace AnalysisDowntimes
{
	/// <summary>
	/// Логика взаимодействия для ChangeHistory.xaml
	/// </summary>
	public partial class ChangeHistory : Window
	{
		private readonly int _period;
		private readonly string _region;
		private readonly long _id;

		public ChangeHistory(int period, string region, long id)
		{
			InitializeComponent();
			_id = id;
			_period = period;
			_region = region;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			namePros.Content = $"Простой: {_id}  Период: {_period}  Усчасток: {_region}";
			HistorySearch();
		}

		private void HistorySearch()
		{
			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				using (NpgsqlCommand getHistory = new NpgsqlCommand(DBContext.getHistoryString, connection))
				{
					getHistory.Parameters.AddWithValue("@id", _id);

					NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(getHistory);
					DataTable dataTable = new DataTable();
					adapter.Fill(dataTable);
					NpgsqlDataReader reader = getHistory.ExecuteReader();


					HistoryTable.ItemsSource = dataTable.DefaultView;
				}
			}

			SetNewColumnNames(HistoryTable);
		}

		private void SetNewColumnNames(System.Windows.Controls.DataGrid dataGrid)
		{
			DataGridTextColumn date_change = (DataGridTextColumn)HistoryTable.Columns.FirstOrDefault(c => c.Header.ToString() == "date_change");
			if (date_change != null)
			{
				date_change.Header = "Дата изменения";
				date_change.Binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
				date_change.Width = 120;
			}
			DataGridTextColumn modified_element = (DataGridTextColumn)HistoryTable.Columns.FirstOrDefault(c => c.Header.ToString() == "modified_element");
			if (modified_element != null)
			{
				modified_element.Header = "Изменения";
			}

			foreach (DataGridColumn column in dataGrid.Columns)
			{
				if (column is DataGridTextColumn textColumn)
				{
					Style headerStyle = new Style(typeof(DataGridColumnHeader));
					headerStyle.BasedOn = (Style)System.Windows.Application.Current.FindResource("DataGridHeaderStyle");

					textColumn.HeaderStyle = headerStyle;
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
