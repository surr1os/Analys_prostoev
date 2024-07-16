using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Analys_prostoev
{
	internal class SortingTable
	{
		#region Data
		INewColumnsNames _columnsNames;
		private object[] _selectedRegions { get; set; }
		private DateTime? _startDate { get; set; }
		private DateTime? _endDate { get; set; }
		private DataGrid _source { get; set; }
		private ComboBox _selectedRow { get; set; }
		#endregion

		public SortingTable(DateTimePicker startDateTime,
					DateTimePicker endDateTime,
					ListBox selectListBox,
					ComboBox selectRowComboBox,
					DataGrid dataGridTable)
		{
			_selectedRegions = selectListBox.SelectedItems.Cast<string>().ToArray();
			_startDate = startDateTime.Value;
			_endDate = endDateTime.Value;
			_source = dataGridTable;
			_columnsNames = new NewColumnsNames();
			_selectedRow = selectRowComboBox;

			List<string> selectedItemsList = new List<string>();

			foreach (var item in selectListBox.SelectedItems)
			{
				selectedItemsList.Add(item.ToString());
			}

			_selectedRegions = selectedItemsList.ToArray();
		}

		public void GetSortTable()
		{

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				//StringBuilder queryBuilder = new StringBuilder("SELECT \"Id\", date_start, date_finish, shifts, status, region, period," +
				//	" category_one, category_two, category_third, reason, created_at, change_at, is_manual FROM analysis WHERE 1=1 AND period >= 5");

				StringBuilder queryBuilder = new StringBuilder("SELECT \"Id\", date_start, date_finish, shifts, status, region, period," +
					" category_one, category_two, category_third, category_fourth, reason, created_at, change_at, is_manual FROM analysis WHERE 1=1 AND period >= 5");

				string original = queryBuilder.ToString();

				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

				RegionFilter(queryBuilder, parameters);
				DateTimeFilter(queryBuilder, parameters);
				CategoryFilter(queryBuilder, _selectedRow);

				string conclusion = queryBuilder.ToString();
				conclusion += " ORDER BY \"Id\" DESC";

				using (NpgsqlCommand command = new NpgsqlCommand(conclusion, connection))
				{
					command.Parameters.AddRange(parameters.ToArray());

					NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
					DataTable dataTable = new DataTable();
					adapter.Fill(dataTable);

					_source.ItemsSource = null;
					_source.ItemsSource = dataTable.DefaultView;
					_source.Items.Refresh();

					_columnsNames.SetNewColumnNames(_source);
				}
				DBContext.queryString = conclusion;
			}
		}

		private void RegionFilter(StringBuilder queryBuilder, List<NpgsqlParameter> parameters)
		{
			if (_selectedRegions != null && _selectedRegions.Length > 0)
			{
				if (_selectedRegions.Length == 1)
				{
					string selectedRegion = _selectedRegions[0].ToString();

					queryBuilder.Append(" AND region ILIKE @selectedRegion");
					parameters.Add(new NpgsqlParameter("selectedRegion", NpgsqlDbType.Varchar));
					parameters[parameters.Count - 1].Value = selectedRegion;

				}
				else
				{
					queryBuilder.Append(" AND (");
					for (int i = 0; i < _selectedRegions.Length; i++)
					{
						string selectedRegion = _selectedRegions[i].ToString();
						if (i > 0)
							queryBuilder.Append(" OR ");

						queryBuilder.Append("region ILIKE @selectedRegion" + i);
						parameters.Add(new NpgsqlParameter($"selectedRegion{i}", NpgsqlDbType.Varchar));
						parameters[parameters.Count - 1].Value = selectedRegion;

					}
					queryBuilder.Append(")");
				}
			}
		}

		private void DateTimeFilter(StringBuilder queryBuilder, List<NpgsqlParameter> parameters)
		{
			if (_startDate.HasValue)
			{
				queryBuilder.Append(" AND (date_start >= @startDate OR date_finish >= @startDate)");
				parameters.Add(new NpgsqlParameter("startDate", NpgsqlDbType.Timestamp));
				parameters[parameters.Count - 1].Value = _startDate.Value;
			}

			if (_endDate.HasValue)
			{
				queryBuilder.Append(" AND (date_finish <= @endDate OR date_start <= @endDate)");
				parameters.Add(new NpgsqlParameter("endDate", NpgsqlDbType.Timestamp));
				parameters[parameters.Count - 1].Value = _endDate.Value;
			}
		}

		private void CategoryFilter(StringBuilder queryBuilder, ComboBox selectedRow)
		{
			if (selectedRow != null && selectedRow.SelectedItem != null)
			{
				string rowSelect = selectedRow.SelectedItem.ToString();
				if (rowSelect == "Классифицированные строки")
				{
					queryBuilder.Append(" AND category_one IS NOT NULL AND category_two IS NOT NULL AND category_third IS NOT NULL");
				}
				else if (rowSelect == "Неклассифицированные строки")
				{
					queryBuilder.Append(" AND (category_one IS NULL OR category_two IS NULL OR category_third IS NULL)");
				}
			}
		}
	}
}
