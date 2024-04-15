using Analys_prostoev.Tables;
using Npgsql;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace Analys_prostoev
{
	public class ExportToExcel : IExportToExcel
	{
		#region Get
		public List<Analysis> GetAnalysisList(string queryString, 
												Xceed.Wpf.Toolkit.DateTimePicker startDateTime,
												Xceed.Wpf.Toolkit.DateTimePicker endDateTime,
												System.Windows.Controls.ComboBox selectComboBox)
		{
			List<Analysis> analysisList = new List<Analysis>();

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();
				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

				if (startDateTime.Value != null)
				{
					parameters.Add(new NpgsqlParameter("startDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
					parameters[parameters.Count - 1].Value = startDateTime.Value;
				}

				if (endDateTime.Value != null)
				{
					parameters.Add(new NpgsqlParameter("endDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
					parameters[parameters.Count - 1].Value = endDateTime.Value;
				}

				if (selectComboBox.SelectedItem != null)
				{
					string selectedRegion = selectComboBox.SelectedItem.ToString();
					parameters.Add(new NpgsqlParameter("selectedRegionCurrent", selectedRegion));
					parameters.Add(new NpgsqlParameter("selectedRegion", selectedRegion + " %"));
				}
				ExecuteSQLAndFetchData(queryString, analysisList, connection, parameters);
			}
			return analysisList;
		}
		private static void GetHeaders(ExcelWorksheet worksheet)
		{
			string[] headers = { "ID", "Дата начала", "Дата окончания", "Период", "Участок", "Категория 1 ур", "Категория 2 ур", "Категория 3 ур", "Причина", "Смена" };
			for (int i = 0; i < headers.Length; i++)
			{
				worksheet.Cells[1, i + 1].Value = headers[i];
			}
		}
		#endregion

		private static void ExecuteSQLAndFetchData(string queryString, List<Analysis> analysisList, NpgsqlConnection connection, List<NpgsqlParameter> parameters)
		{
			using (NpgsqlCommand command = new NpgsqlCommand(queryString, connection))
			{
				command.Parameters.AddRange(parameters.ToArray());
				using (NpgsqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Analysis analysis = new Analysis
						{
							DateStart = reader.GetDateTime(reader.GetOrdinal("date_start")),
							DateFinish = reader.GetDateTime(reader.GetOrdinal("date_finish")),
							Id = reader.GetInt32(reader.GetOrdinal("id")),
							Region = reader.IsDBNull(reader.GetOrdinal("region")) ? string.Empty : reader.GetString(reader.GetOrdinal("region")),
							Period = reader.GetInt32(reader.GetOrdinal("period")),
							CategoryOne = reader.IsDBNull(reader.GetOrdinal("category_one")) ? string.Empty : reader.GetString(reader.GetOrdinal("category_one")),
							CategoryTwo = reader.IsDBNull(reader.GetOrdinal("category_two")) ? string.Empty : reader.GetString(reader.GetOrdinal("category_two")),
							CategoryThird = reader.IsDBNull(reader.GetOrdinal("category_third")) ? string.Empty : reader.GetString(reader.GetOrdinal("category_third")),
							Reason = reader.IsDBNull(reader.GetOrdinal("reason")) ? string.Empty : reader.GetString(reader.GetOrdinal("reason")),
							Shifts = reader.IsDBNull(reader.GetOrdinal("shifts")) ? string.Empty : reader.GetString(reader.GetOrdinal("shifts"))
						};
						analysisList.Add(analysis);
					}
				}
			}
		}

		public void CreateExcelFile(List<Analysis> analysisList)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
			saveFileDialog.FilterIndex = 1;
			saveFileDialog.RestoreDirectory = true;

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = saveFileDialog.FileName;

				using (ExcelPackage package = new ExcelPackage())
				{
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Analysis");
					GetHeaders(worksheet);
					FillData(analysisList, worksheet);
					worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

					// Сохранение файла Excel
					FileInfo file = new FileInfo(fileName);
					package.SaveAs(file);
					System.Windows.MessageBox.Show("Экспорт завершён!", "Окончание работы", MessageBoxButton.OK);
				}
			}
		}

		private static void FillData(List<Analysis> analysisList, ExcelWorksheet worksheet)
		{
			for (int i = 0; i < analysisList.Count; i++)
			{
				Analysis analysis = analysisList[i];
				int row = i + 2;

				worksheet.Cells[row, 1].Value = analysis.Id;
				worksheet.Cells[row, 2].Value = analysis.DateStart;
				worksheet.Cells[row, 2].Style.Numberformat.Format = "dd-mm-yyyy HH:MM:SS";
				worksheet.Cells[row, 3].Value = analysis.DateFinish;
				worksheet.Cells[row, 3].Style.Numberformat.Format = "dd-mm-yyyy HH:MM:SS";
				worksheet.Cells[row, 4].Value = analysis.Period;
				worksheet.Cells[row, 5].Value = analysis.Region;
				worksheet.Cells[row, 6].Value = analysis.CategoryOne;
				worksheet.Cells[row, 7].Value = analysis.CategoryTwo;
				worksheet.Cells[row, 8].Value = analysis.CategoryThird;
				worksheet.Cells[row, 9].Value = analysis.Reason;
				worksheet.Cells[row, 10].Value = analysis.Shifts;
			}
		}

		
	}
}
