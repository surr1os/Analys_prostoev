using Npgsql;
using System.Collections.Generic;
using System.Data;
//using pree
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System;
using System.Windows.Controls.Primitives;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using System.Globalization;
using System.Drawing;
using System.Xml;
using System.Data.SqlClient;
using System.ComponentModel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using System.Windows.Forms;

namespace Analys_prostoev
{

    public partial class MainWindow : System.Windows.Window
    {
        private string connectionString = "Host=10.241.224.71;Port=5432;Database=analysis_user;Username=analysis_user;Password=71NfhRec";
        //private string connectionString = "Host=localhost;Database=myDb;Username=postgres;Password=iqdeadzoom1r";
        //private string connectionStringSecond = "Host=10.241.16.9:5432;Database=ParamASU;Username=postgres;Password=asutp2023";
        public MainWindow()
        {
            InitializeComponent();
        }
        //DataGridTable.ItemsSource

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT region FROM hpt_select";
                    using (NpgsqlCommand selectCommand = new NpgsqlCommand(selectQuery, connection))
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
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            //  SelectDataFromTrends();
            CreateSelectRowCB();
        }

        string queryString = "";

        public class Analysis
        {
            public DateTime date_start { get; set; }
            public DateTime date_finish { get; set; }
            public int id { get; set; }
            public string region { get; set; }
            public int period { get; set; }
            public string category_one { get; set; }
            public string category_two { get; set; }
            public string category_third { get; set; }
        }


        //private void SelectDataFromTrends()
        //{
        //    string selectQuery = "SELECT id, t, v FROM trends WHERE l = 0 and id = 2";
        //    using (NpgsqlConnection connection = new NpgsqlConnection(connectionStringSecond))
        //    {
        //        connection.Open();
        //        NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection);
        //        NpgsqlDataReader reader = command.ExecuteReader();
        //        DateTime previousT = DateTime.MinValue;
        //        // Список для хранения строк analysisTest
        //        List<AnalysisTest> analysisTests = new List<AnalysisTest>();
        //        int previousMilliseconds = -1; // Предыдущее значение миллисекунд                                           
        //        while (reader.Read())
        //        {
        //            // Чтение значений полей id t и v
        //            int id = reader.GetInt32(0);
        //            DateTime t = reader.GetDateTime(1);
        //            double v = reader.GetDouble(2);
        //            int milliseconds = t.Millisecond;

        //            if (v == 1)
        //            {
        //                previousMilliseconds = milliseconds;
        //                analysisTests.Add(new AnalysisTest { id = id, date_start = t });
        //                previousT = t;

        //            }

        //            else if (v == 0 && analysisTests.Count > 0 && milliseconds == previousMilliseconds)
        //            {

        //                // Записываем значение поля t в datefinish и добавляем строку analysisTest в таблицу
        //                analysisTests[analysisTests.Count - 1].date_finish = t;
        //                previousT = DateTime.MinValue;

        //                previousMilliseconds = -1; // Сброс предыдущего значения миллисекунд
        //            }

        //            else
        //            {
        //                continue;
        //            }

        //        }

        //        reader.Close();
        //        int addRowsCount = 0;
        //        int sameRowsCount = 0;
        //        int minutesRowsCount = 0;
        //        foreach (AnalysisTest analysisTest in analysisTests)
        //        {
        //            using (NpgsqlConnection connectionSecond = new NpgsqlConnection(connectionString))
        //            {

        //                if (analysisTest.date_finish != DateTime.MinValue)
        //                {
        //                    // Проверка на уже обработанные строки

        //                    connectionSecond.Open();

        //                    string selectHptQuery = "SELECT region FROM hpt_select_trends WHERE id = @id";

        //                    NpgsqlCommand hptCommand = new NpgsqlCommand(selectHptQuery, connectionSecond);
        //                    hptCommand.Parameters.AddWithValue("@id", analysisTest.id);

        //                    NpgsqlDataReader hptReader = hptCommand.ExecuteReader();

        //                    if (hptReader.Read())
        //                    {
        //                        // Чтение значения поля region
        //                        analysisTest.region = hptReader.GetString(0);
        //                    }

        //                    hptReader.Close();
        //                    NpgsqlCommand selectCommand = new NpgsqlCommand("SELECT COUNT(*) FROM analysisTest WHERE date_start = @date_start AND region = @region", connectionSecond);
        //                    selectCommand.Parameters.AddWithValue("@date_start", analysisTest.date_start);
        //                    selectCommand.Parameters.AddWithValue("@region", NpgsqlDbType.Text, analysisTest.region);

        //                    int count = Convert.ToInt32(selectCommand.ExecuteScalar());
        //                    if (count == 0)
        //                    {
        //                        TimeSpan period = analysisTest.date_finish - analysisTest.date_start;
        //                        int minutes = (int)period.TotalMinutes;
        //                        if (minutes >= 5)
        //                        {
        //                            string insertQuery = "INSERT INTO analysisTest (date_start, date_finish, region, period) VALUES (@date_start, @date_finish, @region, @period)";
        //                            NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, connectionSecond);
        //                            insertCommand.Parameters.AddWithValue("@date_start", analysisTest.date_start);
        //                            insertCommand.Parameters.AddWithValue("@date_finish", analysisTest.date_finish);
        //                            insertCommand.Parameters.AddWithValue("@region", NpgsqlDbType.Text, analysisTest.region);
        //                            insertCommand.Parameters.AddWithValue("@period", NpgsqlDbType.Integer, minutes);
        //                            insertCommand.ExecuteNonQuery();
        //                            addRowsCount++;
        //                        }
        //                        else
        //                        {
        //                            minutesRowsCount++;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        sameRowsCount++;
        //                    }
        //                }
        //            }

        //        }
        //        MessageBox.Show($"Добавлено строк: {addRowsCount}\nНе добавлено:\nПохожих строк: {sameRowsCount}\nС периодом < 5 минут: {minutesRowsCount}"); ;

        //    }

        //}

        private void CreateSelectRowCB()
        {
            selectRowComboBox.Items.Add("Все строки");
            selectRowComboBox.Items.Add("Классифицированные строки");
            selectRowComboBox.Items.Add("Неклассифицированные строки");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    this.queryString = "SELECT * FROM analysis WHERE 1=1 AND period >= 5";

                    List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                    if (startDatePicker.Value.HasValue)
                    {
                        this.queryString += " AND date_start >= @startDate";
                        parameters.Add(new NpgsqlParameter("startDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
                        parameters[parameters.Count - 1].Value = startDatePicker.Value.Value;
                    }

                    if (endDatePicker.Value.HasValue)
                    {
                        this.queryString += " AND date_start <= @endDate";
                        parameters.Add(new NpgsqlParameter("endDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
                        parameters[parameters.Count - 1].Value = endDatePicker.Value.Value;
                    }

                    if (selectComboBox.SelectedItem != null)
                    {
                        string selectedRegion = selectComboBox.SelectedItem.ToString();
                        if (selectedRegion == "ХПТ" || selectedRegion == "ХПТР")
                        {
                            this.queryString += $" AND region ILIKE @selectedRegion";
                        }
                        else
                        {
                            this.queryString += $" AND region = @selectedRegionCurrent";
                            parameters.Add(new NpgsqlParameter("selectedRegionCurrent", selectedRegion));
                        }
                        parameters.Add(new NpgsqlParameter("selectedRegion", selectedRegion + " %"));
                    }

                    if (selectRowComboBox.SelectedItem != null)
                    {
                        string rowSelect = selectRowComboBox.SelectedItem.ToString();
                        if (rowSelect == "Все строки")
                        {
                            this.queryString += "";
                        }
                        else if (rowSelect == "Классифицированные строки")
                        {
                            this.queryString += " AND category_one IS NOT NULL AND category_one <> '' AND category_two IS NOT NULL AND category_two <> '' AND category_third IS NOT NULL AND category_third <> ''";
                        }
                        else if (rowSelect == "Неклассифицированные строки")
                        {
                            this.queryString += " AND category_one IS NULL AND category_two IS NULL AND category_third IS NULL";
                        }
                    }


                    using (NpgsqlCommand command = new NpgsqlCommand(this.queryString, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());

                        NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                        System.Data.DataTable dataTable = new System.Data.DataTable();
                        adapter.Fill(dataTable);

                        DataGridTable.ItemsSource = dataTable.DefaultView;

                        SetNewColumnNames();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error");
                }

            }
        }

        private void SetNewColumnNames()
        {
            DataGridTextColumn category_level_one = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "category_one");
            if (category_level_one != null)
            {
                category_level_one.Width = new DataGridLength(300); // Введите нужную вам ширину
                category_level_one.Header = "Категория Уровень 1";

            }

            DataGridTextColumn category_level_two = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "category_two");
            if (category_level_two != null)
            {
                category_level_two.Width = new DataGridLength(300); // Введите нужную вам ширину
                category_level_two.Header = "Категория Уровень 2";
            }
            DataGridTextColumn category_level_third = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "category_third");
            if (category_level_third != null)
            {
                category_level_third.Width = new DataGridLength(300); // Введите нужную вам ширину
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
                date_start.Binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
            }
            DataGridTextColumn date_finish = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "date_finish");
            if (date_finish != null)
            {
                date_finish.Header = "Дата Финиша";
                date_finish.Binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
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
                    string reason = rowView["reason"].ToString();
                    string regionValue = rowView["region"].ToString();
                    string categoryOneValue = rowView["category_one"].ToString();
                    string categoryTwoValue = rowView["category_two"].ToString();
                    string categoryThirdValue = rowView["category_third"].ToString();

                    // Создаем экземпляр окна CategoryHierarchy
                    var newWindow = new CategoryHierarchy(regionValue);
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
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE analysis SET category_one = @categoryOne, category_two = @categoryTwo,category_third = @categoryThird, reason = @reason_new WHERE \"Id\" = @Id";
                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("categoryOne", categoryOneValue);
                        updateCommand.Parameters.AddWithValue("categoryTwo", categoryTwoValue);
                        updateCommand.Parameters.AddWithValue("categoryThird", categoryThirdValue);
                        updateCommand.Parameters.AddWithValue("reason_new", reasonValue);

                        int id = Convert.ToInt32(selectedItem["Id"]);
                        updateCommand.Parameters.AddWithValue("Id", id);

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            System.Windows.MessageBox.Show("Обновление значения выполнено успешно");
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Ошибка при обновлении значения");
                        }
                    }
                }
            }
        }

        public List<Analysis> GetAnalysisList(string queryString)
        {
            List<Analysis> analysisList = new List<Analysis>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                if (startDatePicker.Value.HasValue)
                {
                    this.queryString += " AND date_start >= @startDate";
                    parameters.Add(new NpgsqlParameter("startDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
                    parameters[parameters.Count - 1].Value = startDatePicker.Value.Value;
                }

                if (endDatePicker.Value.HasValue)
                {
                    this.queryString += " AND date_start <= @endDate";
                    parameters.Add(new NpgsqlParameter("endDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
                    parameters[parameters.Count - 1].Value = endDatePicker.Value.Value;
                }

                if (selectComboBox.SelectedItem != null)
                {
                    string selectedRegion = selectComboBox.SelectedItem.ToString();
                    this.queryString += $" AND region = @selectedRegionCurrent";
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
                                date_start = reader.GetDateTime(reader.GetOrdinal("date_start")),
                                date_finish = reader.GetDateTime(reader.GetOrdinal("date_finish")),
                                id = reader.GetInt32(reader.GetOrdinal("id")),
                                region = reader.GetString(reader.GetOrdinal("region")),
                                period = reader.GetInt32(reader.GetOrdinal("period")),
                                category_one = reader.IsDBNull(reader.GetOrdinal("category_one")) ? string.Empty : reader.GetString(reader.GetOrdinal("category_one")),
                                category_two = reader.IsDBNull(reader.GetOrdinal("category_two")) ? string.Empty : reader.GetString(reader.GetOrdinal("category_two")),
                                category_third = reader.IsDBNull(reader.GetOrdinal("category_third")) ? string.Empty : reader.GetString(reader.GetOrdinal("category_third")),
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
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

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

                        worksheet.Cells[row, 1].Value = analysis.id;

                        worksheet.Cells[row, 2].Value = analysis.date_start;
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "yyyy-mm-dd HH:MM:SS";

                        worksheet.Cells[row, 3].Value = analysis.date_finish;
                        worksheet.Cells[row, 3].Style.Numberformat.Format = "yyyy-mm-dd HH:MM:SS";

                        worksheet.Cells[row, 4].Value = analysis.period;
                        worksheet.Cells[row, 5].Value = analysis.region;
                        worksheet.Cells[row, 6].Value = analysis.category_one;
                        worksheet.Cells[row, 7].Value = analysis.category_two;
                        worksheet.Cells[row, 8].Value = analysis.category_third;

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
            ExportToExcel(queryString);
        }
    }
}









