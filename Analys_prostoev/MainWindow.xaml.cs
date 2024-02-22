﻿using Analys_prostoev.Tables;
using Npgsql;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;


namespace Analys_prostoev
{

    public partial class MainWindow : System.Windows.Window
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Edit_MenuItem.Visibility = Visibility.Collapsed;
            Delete_MenuItem.Visibility = Visibility.Collapsed;
            Cancel_MenuItem.Visibility = Visibility.Collapsed;

            selectComboBox.Items.Add("Все участки");

            startDatePicker.Value = DateTime.Today.Date.AddHours(0);

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

            CreateSelectRowCB();
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
            DataRowView item = (DataRowView)DataGridTable.SelectedItem;
            if (item != null)
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
                        GetSortTable();
                    }
                }
                else
                    return;
            }
            else
            {
                MessageBox.Show("Вы не выбрали простой!");
                return;
            }
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

                string conclusion = "SELECT \"Id\", date_start, date_finish, shifts, status, region, period," +
                    " category_one, category_two, category_third, reason, created_at, change_at, is_manual FROM analysis WHERE 1=1 AND period >= 5";
                DBContext.queryString = conclusion;
                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                if (selectComboBox.SelectedItem != null)
                {
                    string selectedRegion = selectComboBox.SelectedItem.ToString();
                    if (selectedRegion == "Все участки")
                    {
                        // Do nothing
                    }
                    else if (selectedRegion == "ХПТ" || selectedRegion == "ХПТР")
                    {
                        DBContext.queryString += $" AND region ILIKE @selectedRegion";
                        parameters.Add(new NpgsqlParameter("selectedRegion", selectedRegion + " %"));
                    }
                    else
                    {
                        DBContext.queryString += $" AND region = @selectedRegionCurrent";
                        parameters.Add(new NpgsqlParameter("selectedRegionCurrent", selectedRegion));
                    }
                }

                if (selectRowComboBox.SelectedItem != null)
                {
                    string rowSelect = selectRowComboBox.SelectedItem.ToString();
                    if (rowSelect == "Все строки")
                    {
                        // Do nothing
                    }
                    else if (rowSelect == "Классифицированные строки")
                    {
                        DBContext.queryString += " AND category_one IS NOT NULL AND category_one <> '' AND category_two IS NOT NULL AND category_two <> '' AND category_third IS NOT NULL AND category_third <> ''";
                    }
                    else if (rowSelect == "Неклассифицированные строки")
                    {
                        DBContext.queryString += " AND category_one IS NULL AND category_two IS NULL AND category_third IS NULL";
                    }
                }

                GetSortDate(parameters);

                using (NpgsqlCommand command = new NpgsqlCommand(DBContext.queryString, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);
                    NpgsqlDataReader reader = command.ExecuteReader();

                    DataGridTable.ItemsSource = dataTable.DefaultView;

                    SetNewColumnNames();
                }
            }
        }

        private void GetSortDate(List<NpgsqlParameter> parameters)
        {
            if (startDatePicker.Value.HasValue)
            {
                DBContext.queryString += " AND (date_start >= @startDate OR date_finish >= @startDate)";
                parameters.Add(new NpgsqlParameter("startDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
                parameters[parameters.Count - 1].Value = startDatePicker.Value.Value;
            }

            if (endDatePicker.Value.HasValue)
            {
                DBContext.queryString += " AND (date_finish <= @endDate OR date_start <= @endDate)";
                parameters.Add(new NpgsqlParameter("endDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
                parameters[parameters.Count - 1].Value = endDatePicker.Value.Value;
            }
        }

        private void SetNewColumnNames()
        {
            DataGridTextColumn category_level_one = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "category_one");
            if (category_level_one != null)
            {
                category_level_one.Width = new DataGridLength(300);
                category_level_one.Header = "Категория Уровень 1";
            }
            DataGridTextColumn Id = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "Id");
            if (Id != null)
            {
                Id.Header = "Номер";
            }
            DataGridTextColumn category_level_two = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "category_two");
            if (category_level_two != null)
            {
                category_level_two.Width = new DataGridLength(300);
                category_level_two.Header = "Категория Уровень 2";
            }
            DataGridTextColumn category_level_third = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "category_third");
            if (category_level_third != null)
            {
                category_level_third.Width = new DataGridLength(300);
                category_level_third.Header = "Категория Уровень 3";
            }
            DataGridTextColumn reason = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "reason");
            if (reason != null)
            {
                reason.Width = new DataGridLength(600);
                reason.Header = "Причина";
            }

            DataGridTextColumn date_start = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "date_start");
            if (date_start != null)
            {
                date_start.Header = "Дата Начала";
                date_start.Binding.StringFormat = "dd-MM-yyyy HH:mm:ss";
            }
            DataGridTextColumn date_finish = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "date_finish");
            if (date_finish != null)
            {
                date_finish.Header = "Дата Финиша";
                date_finish.Binding.StringFormat = "dd-MM-yyyy HH:mm:ss";
            }
            DataGridTextColumn period = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "period");
            if (period != null)
            {
                period.Header = "Период";
            }
            DataGridTextColumn region = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "region");
            if (region != null)
            {
                region.Header = "Участок";
            }
            DataGridTextColumn status = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "status");
            if (status != null)
            {
                status.Header = "Статус";
            }
            DataGridTextColumn shifts = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "shifts");
            if (shifts != null)
            {
                shifts.Header = "Смена";
            }
            foreach (DataRowView row in DataGridTable.Items)
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

            DataGridTextColumn created_at = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "created_at");
            if (created_at != null)
            {
                created_at.Header = "Создано";
                created_at.Binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
            }
            DataGridTextColumn change_at = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "change_at");
            if (change_at != null)
            {
                change_at.Header = "Изменено";
                change_at.Binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
            }
            DataGridCheckBoxColumn is_manual = (DataGridCheckBoxColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "is_manual");
            if (is_manual != null)
            {
                is_manual.Header = " Создано вручную ";
            }
            foreach (DataGridColumn column in DataGridTable.Columns)
            {
                DataGridTextColumn textColumn = column as DataGridTextColumn;
                if (textColumn != null)
                {
                    textColumn.HeaderStyle = new System.Windows.Style(typeof(DataGridColumnHeader));
                    textColumn.HeaderStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, System.Windows.HorizontalAlignment.Center));
                }
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

                // Проверяем, что столбец имеет заголовок "category"
                if (columnValue == "Категория Уровень 1" || columnValue == "Категория Уровень 2" || columnValue == "Категория Уровень 3" || columnValue == "Причина")
                {
                    // Получаем объект данных
                    DataRowView rowView = (DataRowView)cellInfo.Item;
                    // Получаем значения ячеек выбранной строки
                    long id = Convert.ToInt64(rowView["Id"]);
                    string reason = rowView["reason"].ToString();
                    string regionValue = rowView["region"].ToString();
                    string categoryOneValue = rowView["category_one"].ToString();
                    string categoryTwoValue = rowView["category_two"].ToString();
                    string categoryThirdValue = rowView["category_third"].ToString();

                    // Создаем экземпляр окна CategoryHierarchy
                    var newWindow = new CategoryHierarchy(regionValue, id, categoryOneValue, categoryTwoValue, categoryThirdValue);
                    newWindow.ParentWindow = this;
                    // Открываем окно CategoryHierarchy
                    newWindow.Show();

                    // Установка значений других полей в окне CategoryHierarchy
                    newWindow.categoryOneTextB.Text = categoryOneValue;
                    newWindow.categoryTwoTextB.Text = categoryTwoValue;
                    newWindow.categoryThirdTextB.Text = categoryThirdValue;
                    newWindow.reasonTextB.Text = reason;

                }
            }
        }

        public void UpdateSelectedRowValues(string categoryOneValue, string categoryTwoValue, string categoryThirdValue, string reasonValue)
        {
            // Получаем выделенную строку в таблице
            DataRowView selectedItem = DataGridTable.SelectedItem as DataRowView;
            if (selectedItem != null)
            {
                // Обновляем значения ячеек выбранной строки

                selectedItem["category_one"] = categoryOneValue;
                selectedItem["category_two"] = categoryTwoValue;
                selectedItem["category_third"] = categoryThirdValue;
                selectedItem["reason"] = reasonValue;

                // Обновляем строку в базе данных
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

        public List<Analysis> GetAnalysisList(string queryString)
        {
            List<Analysis> analysisList = new List<Analysis>();

            using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
            {
                connection.Open();
                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                if (startDatePicker.Value.HasValue)
                {
                    DBContext.queryString += " AND date_start >= @startDate";
                    parameters.Add(new NpgsqlParameter("startDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
                    parameters[parameters.Count - 1].Value = startDatePicker.Value.Value;
                }

                if (endDatePicker.Value.HasValue)
                {
                    DBContext.queryString += " AND date_start <= @endDate";
                    parameters.Add(new NpgsqlParameter("endDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
                    parameters[parameters.Count - 1].Value = endDatePicker.Value.Value;
                }

                if (selectComboBox.SelectedItem != null)
                {
                    string selectedRegion = selectComboBox.SelectedItem.ToString();
                    DBContext.queryString += $" AND region = @selectedRegionCurrent";
                    parameters.Add(new NpgsqlParameter("selectedRegionCurrent", selectedRegion));
                    parameters.Add(new NpgsqlParameter("selectedRegion", selectedRegion + " %"));
                }


                // Execute SQL Query and fetch data
                using (NpgsqlCommand command = new NpgsqlCommand(queryString, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {

                        // Fetch data and populate the list
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
                            };

                            analysisList.Add(analysis);
                        }
                    }
                }
            }

            return analysisList;
        }

        public void ExportToExcel(string queryString)
        {
            // Заполнение списка
            List<Analysis> analysisList = GetAnalysisList(queryString);

            // Создание Excel файла
            CreateExcelFile(analysisList);
        }

        public void CreateExcelFile(List<Analysis> analysisList)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Открытие диалогового окна для сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) // Используйте DialogResult.OK
            {
                string fileName = saveFileDialog.FileName;

                // Создание нового Excel пакета
                using (ExcelPackage package = new ExcelPackage())
                {
                    // Создание нового листа
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Analysis");

                    // Установка заголовков столбцов
                    string[] headers = { "ID", "Дата начала", "Дата окончания", "Период", "Участок", "Категория 1 ур", "Категория 2 ур", "Категория 3 ур" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                    }

                    // Заполнение данных из списка анализов
                    for (int i = 0; i < analysisList.Count; i++)
                    {
                        Analysis analysis = analysisList[i];
                        int row = i + 2; // Начинаем заполнение с 2 строки

                        worksheet.Cells[row, 1].Value = analysis.Id;

                        worksheet.Cells[row, 2].Value = analysis.DateStart;
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "yyyy-mm-dd HH:MM:SS";

                        worksheet.Cells[row, 3].Value = analysis.DateFinish;
                        worksheet.Cells[row, 3].Style.Numberformat.Format = "yyyy-mm-dd HH:MM:SS";

                        worksheet.Cells[row, 4].Value = analysis.Period;
                        worksheet.Cells[row, 5].Value = analysis.Region;
                        worksheet.Cells[row, 6].Value = analysis.CategoryOne;
                        worksheet.Cells[row, 7].Value = analysis.CategoryTwo;
                        worksheet.Cells[row, 8].Value = analysis.CategoryThird;
                    }

                    // Автоматическое подгонка ширины столбцов
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Сохранение файла Excel
                    FileInfo file = new FileInfo(fileName);
                    package.SaveAs(file);
                }
            }
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
                    Delete_MenuItem.Visibility = Visibility.Collapsed; // скрываем кнопку удаления у автоматических простоев
                }
                if (status == "Не согласованно" && manual == false)
                {
                    Edit_MenuItem.Visibility = Visibility.Visible; // показываем кнопку изменения у не автоматических простоев
                }
                if (manual == true)
                {
                    Delete_MenuItem.Visibility = Visibility.Visible;// показываем кнопку изменения и уаления
                }
                if (categoryOne != null || categoryTwo != null || categoryThird != null)
                {
                    Edit_MenuItem.Visibility = Visibility.Visible;
                    Cancel_MenuItem.Visibility = Visibility.Visible;
                }
                else
                {
                    Edit_MenuItem.Visibility = Visibility.Collapsed; // скрываем кнопку изменения
                }
            }
            else
            {
                Edit_MenuItem.Visibility = Visibility.Visible; // показываем кнопку изменения
            }
        }
    }
}