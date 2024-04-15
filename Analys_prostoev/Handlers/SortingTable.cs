using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Xceed.Wpf.Toolkit;
using System.Windows.Controls;

namespace Analys_prostoev
{
	internal class SortingTable
	{
		#region Data
		INewColumnsNames _columnsNames;
		private object _selectRow {  get; set; }
		private object _selectComboBox {  get; set; }
		private DateTime? _startDate { get; set; }
		private DateTime? _endDate { get; set; }
		private DataGrid _source { get; set; }
		#endregion

		public SortingTable(DateTimePicker startDateTime,
							DateTimePicker endDateTime, 
							ComboBox selectComboBox, 
							ComboBox selectRowComboBox,
							DataGrid dataGridTable) 
		{
			_selectComboBox = selectComboBox.SelectedItem;
			_startDate = startDateTime.Value;
			_endDate = endDateTime.Value;
			_selectRow = selectRowComboBox.SelectedItem;
			_source = dataGridTable;
			_columnsNames = new NewColumnsNames();
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

				RegionFilter(queryBuilder, parameters);
				CategoryFilter(queryBuilder);
				DateTimeFilter(queryBuilder, parameters);

				string conclusion = queryBuilder.ToString();
				conclusion += " ORDER BY \"Id\" DESC";

				using (NpgsqlCommand command = new NpgsqlCommand(conclusion, connection))
				{
					command.Parameters.AddRange(parameters.ToArray());

					NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
					DataTable dataTable = new DataTable();
					adapter.Fill(dataTable);

					_source.ItemsSource = dataTable.DefaultView;

					_columnsNames.SetNewColumnNames(_source);
				}
				DBContext.queryString = conclusion;
			}
		}

		private void RegionFilter(StringBuilder queryBuilder, List<NpgsqlParameter> parameters)
		{
			if (_selectComboBox != null)
			{
				string selectedRegion = _selectComboBox.ToString();
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

		private void CategoryFilter(StringBuilder queryBuilder)
		{
			if (_selectRow != null)
			{
				string rowSelect = _selectRow.ToString();
				if (rowSelect == "Классифицированные строки")
				{
					queryBuilder.Append(" AND category_one IS NOT NULL AND category_one <> '' AND category_two IS NOT NULL AND category_two <> '' AND category_third IS NOT NULL AND category_third <> ''");
				}
				else if (rowSelect == "Неклассифицированные строки")
				{
					queryBuilder.Append(" AND category_one IS NULL AND category_two IS NULL AND category_third IS NULL");
				}
			}
		}
	}
}
