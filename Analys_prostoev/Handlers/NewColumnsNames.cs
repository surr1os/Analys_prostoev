using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Control = System.Windows.Controls.Control;
using DataGrid = System.Windows.Controls.DataGrid;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace AnalysisDowntimes
{
	/// <summary>
	/// Представляет собой класс для изменения названии столбцов таблицы Analysis.
	/// </summary>
	public class NewColumnsNames : INewColumnsNames
	{
		public void SetNewColumnNames(DataGrid dataGrid)
		{
			Dictionary<string, string> columnHeaders = new Dictionary<string, string>
			{
				{ "Id", "Номер" },
				{ "category_one", "Категория ур. 1" },
				{ "category_two", "Категория ур. 2" },
				{ "category_third", "Категория ур. 3" },
				{ "category_fourth", "Категория ур. 4" },
				{ "reason", "Причина" },
				{ "date_start", "Дата Начала" },
				{ "date_finish", "Дата Финиша" },
				{ "period", "Период" },
				{ "region", "Участок" },
				{ "status", "Статус" },
				{ "shifts", "Смена" },
				{ "created_at", "Создано" },
				{ "change_at", "Изменено" },
				{ "is_manual", " Создано вручную " }
			};

			foreach (DataGridColumn column in dataGrid.Columns)
			{
				DataGridTextColumn textColumn = column as DataGridTextColumn;
				if (textColumn != null)
				{
					textColumn.HeaderStyle = new Style(typeof(DataGridColumnHeader));
					textColumn.HeaderStyle.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
				}
			}

			DataGridCheckBoxColumn is_manual = (DataGridCheckBoxColumn)dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "is_manual");
			if (is_manual != null)
				is_manual.Header = " Создано вручную ";

			foreach (var kvp in columnHeaders)
			{
				string columnName = kvp.Key;
				string columnHeader = kvp.Value;

				ChangeColumnHeader(columnName, columnHeader, dataGrid);
				SetStyles(dataGrid);
			}

			foreach (DataRowView row in dataGrid.Items)
			{
				DataRow dataRow = row.Row;
				string statusValue = dataRow["status"].ToString();

				if (statusValue == "1")
				{
					dataRow["status"] = "Согласовано";
				}
				else if (statusValue == "0")
				{
					dataRow["status"] = "Не согласовано";
				}
				else
				{
					dataRow["status"] = "";
				}
			}

			CustomizeDataGridColumns(dataGrid);
		}

		private void ChangeColumnHeader(string nameHeader, string newHeader, DataGrid dataGrid)
		{
			DataGridTextColumn oldHeader = (DataGridTextColumn)dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == nameHeader);
			if (oldHeader != null)
				oldHeader.Header = newHeader;
		}
		private void CustomizeDataGridColumns(DataGrid dataGrid)
		{
			string[] dateHeaders = new string[4] { "Создано", "Изменено", "Дата Начала", "Дата Финиша" };
			foreach (var dateHeader in dateHeaders)
			{
				DataGridTextColumn createdAtColumn = (DataGridTextColumn)dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == dateHeader);
				if (createdAtColumn != null)
				{
					createdAtColumn.Binding.StringFormat = "dd-MM-yyyy HH:mm:ss";
				}
			}

			string[] headers = { "Категория ур. 1", "Категория ур. 2", "Категория ур. 3", "Категория ур. 4" };
			foreach (var header in headers)
			{
				DataGridColumn column = dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == header);
				if (column != null && (string)column.Header == headers[0])
					column.Width = new DataGridLength(225, DataGridLengthUnitType.Star);

				if (column != null && (string)column.Header == headers[1])
					column.Width = new DataGridLength(225, DataGridLengthUnitType.Star);

				if (column != null && (string)column.Header == headers[2])
					column.Width = new DataGridLength(225, DataGridLengthUnitType.Star);

				if (column != null && (string)column.Header == headers[3])
					column.Width = new DataGridLength(225, DataGridLengthUnitType.Star);

			}

			DataGridColumn reasonColumn = dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Причина");
			if (reasonColumn != null)
			{
				reasonColumn.Width = 150;
			}
		}

		public void SetStyles(DataGrid dataGrid)
		{
			dataGrid.Style = (Style)Application.Current.FindResource("DataGridStyle");

			foreach (DataGridColumn column in dataGrid.Columns)
			{
				if (column is DataGridTextColumn textColumn)
				{
					Style headerStyle = new Style(typeof(DataGridColumnHeader));
					headerStyle.BasedOn = (Style)Application.Current.FindResource("DataGridHeaderStyle");
					headerStyle.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
					

					textColumn.HeaderStyle = headerStyle;
				}
				else if (column is DataGridCheckBoxColumn checkBoxColumn)
				{
					Style headerStyle = new Style(typeof(DataGridColumnHeader));
					headerStyle.BasedOn = (Style)Application.Current.FindResource("DataGridHeaderStyle");
					headerStyle.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
					

					checkBoxColumn.HeaderStyle = headerStyle;

					Style elementStyle = new Style(typeof(CheckBox));
					elementStyle.Setters.Add(new Setter(Control.HorizontalAlignmentProperty, HorizontalAlignment.Center));
				}
			}
		}
	}
}
