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
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
           
          //  SelectDataFromTrends();
            CreateSelectRowCB();
        }

        public class AnalysisTest
        {
            public DateTime date_start { get; set; }
            public DateTime date_finish { get; set; }
            public int id { get; set; }
            public string region { get; set; }
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

                    string queryString = "SELECT * FROM analysis WHERE 1=1 AND period >= 5";

                    List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                    if (startDatePicker.Value.HasValue)
                    {
                        queryString += " AND date_start >= @startDate";
                        parameters.Add(new NpgsqlParameter("startDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
                        parameters[parameters.Count - 1].Value = startDatePicker.Value.Value;
                    }

                    if (endDatePicker.Value.HasValue)
                    {
                        queryString += " AND date_start <= @endDate";
                        parameters.Add(new NpgsqlParameter("endDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
                        parameters[parameters.Count - 1].Value = endDatePicker.Value.Value;
                    }

                    if (selectComboBox.SelectedItem != null)
                    {
                        string selectedRegion = selectComboBox.SelectedItem.ToString();
                        if (selectedRegion == "ХПТ" || selectedRegion == "ХПТР")
                        {
                            queryString += $" AND region ILIKE @selectedRegion";
                        }
                        else
                        {
                            queryString += $" AND region = @selectedRegionCurrent";
                            parameters.Add(new NpgsqlParameter("selectedRegionCurrent", selectedRegion));
                        }
                        parameters.Add(new NpgsqlParameter("selectedRegion", selectedRegion + " %"));
                    }

                    if (selectRowComboBox.SelectedItem != null)
                    {
                        string rowSelect = selectRowComboBox.SelectedItem.ToString();
                        if (rowSelect == "Все строки")
                        {
                            queryString += "";
                        }
                        else if (rowSelect == "Классифицированные строки")
                        {
                            queryString += " AND category_one IS NOT NULL AND category_one <> '' AND category_two IS NOT NULL AND category_two <> '' AND category_third IS NOT NULL AND category_third <> ''";
                        }
                        else if (rowSelect == "Неклассифицированные строки")
                        {
                            queryString += " AND category_one IS NULL AND category_two IS NULL AND category_third IS NULL";
                        }
                    }


                    using (NpgsqlCommand command = new NpgsqlCommand(queryString, connection))
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
                    textColumn.HeaderStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
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
        public void ExportToExcel(DataGrid dg)
        {
            // Создание нового экземпляра Excel.
            var excelApp = new Microsoft.Office.Interop.Excel.Application();

            // Настройка свойств новой книги Excel.
            excelApp.Visible = true;
            var workbook = excelApp.Workbooks.Add();
            var worksheet = workbook.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;

            // Установка заголовков столбцов.
            for (int i = 0; i < dg.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1] = dg.Columns[i].Header;
            }

            // Открытие диалогового окна сохранения файла.
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "Excel файлы (*.xlsx)|*.xlsx";
            saveFileDialog.DefaultExt = "xlsx";

            // Если пользователь выбрал место сохранения файла и нажал "ОК", продолжаем сохранение файла.
           
            

                // Создание стилей для заголовков и данных.
                var headerStyle = workbook.Styles.Add("HeaderStyle");
                headerStyle.Font.Bold = true;
                headerStyle.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                headerStyle.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);

                var dataStyle = workbook.Styles.Add("DataStyle");
                dataStyle.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                dataStyle.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightYellow);

                // Применение стилей к заголовкам столбцов.
               // worksheet.get_Range("A1:I1", $"A{dg.Columns.Count}").Style = "HeaderStyle";

                // Заполнение ячеек данными из DataGrid и применение стилей.
                for (int row = 0; row < dg.Items.Count; row++)
                {
                    for (int col = 0; col < dg.Columns.Count; col++)
                    {
                        var cellContent = dg.Columns[col].GetCellContent(dg.Items[row]);
                        if (cellContent is TextBlock textBlock)
                        {
                            string columnName = dg.Columns[col].Header.ToString();
                            switch (columnName)
                            {
                                case "Дата Начала":
                                case "Дата Финиша":
                                    //DateTime dateTime = DateTime.ParseExact(textBlock.Text, "yyyy-MM-dd H:mm:ss", CultureInfo.InvariantCulture);
                                    //worksheet.Cells[row + 2, col + 1] = dateTime.ToString("dd.MM.yyyy H:mm");
                                    //worksheet.Columns[col + 1].NumberFormat = "dd.MM.yyyy H:mm";
                                    worksheet.Columns[col + 1].ColumnWidth = 15;
                                    worksheet.Cells[row + 2, col + 1] = textBlock.Text;
                                    break;
                                case "Id":
                                case "Период":
                                    worksheet.Cells[row + 2, col + 1] = textBlock.Text;
                                    //worksheet.Columns[col + 1].NumberFormat = "0";
                                    worksheet.Columns[col + 1].ColumnWidth = 15;
                                    break;
                                default:
                                    worksheet.Cells[row + 2, col + 1] = textBlock.Text;
                                    worksheet.Columns[col + 1].ColumnWidth = 25;
                                    break;
                            }
                        }
                        else
                        {
                            worksheet.Cells[row + 2, col + 1] = (cellContent != null) ? cellContent.ToString() : "";
                        }

                        // Применение стилей к данным.
                       // worksheet.Cells[row + 2, col + 1].Style = "DataStyle";
                    }
                }

                // Применение стилей к строкам данных.
                var range = worksheet.Range[$"A2:I{dg.Items.Count + 1}"]; // Замените "Z" на последнюю колонку, которую вы заполняете данными.
               // range.Style = "DataStyle";
            }

                

                // Сохранение файла Excel.
    
        

        private void Button_Click_Excel(object sender, RoutedEventArgs e)
        {
            ExportToExcel(DataGridTable);
        }
    }
}









