using Npgsql;
using System.Collections.Generic;
using System.Data;
//using pree
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Data.SqlClient;
using NpgsqlTypes;

namespace Analys_prostoev
{

    public partial class MainWindow : Window
    {
        private string connectionString = "Host=10.241.224.71;Port=5432;Database=analysisTestBD;Username=postgres;Password=BOuDxGVN2g";
        //private string connectionString = "Host=localhost;Database=myDb;Username=postgres;Password=iqdeadzoom1r";
        //private string connectionStringSecond = "Host=10.241.16.9:5432;Database=ParamASU;Username=postgres;Password=asutp2023";
        public MainWindow()
        {
            InitializeComponent();
        }
        //DataGridTable.ItemsSource

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
                connection.Open();

                 string queryString = "SELECT * FROM analysistest WHERE 1=1";

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                if (startDatePicker.SelectedDate != null)
                {
                    queryString += " AND TO_DATE(cast(date_start as TEXT), 'YYYY-MM-DD') >= @startDate";
                    parameters.Add(new NpgsqlParameter("startDate", NpgsqlTypes.NpgsqlDbType.Date));
                    parameters[parameters.Count - 1].Value = startDatePicker.SelectedDate;
                }

                if (endDatePicker.SelectedDate != null)
                {
                    queryString += " AND TO_DATE(cast(date_start as TEXT), 'YYYY-MM-DD') <= @endDate";
                    parameters.Add(new NpgsqlParameter("endDate", NpgsqlTypes.NpgsqlDbType.Date));
                    parameters[parameters.Count - 1].Value = endDatePicker.SelectedDate;
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
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                  
                    DataGridTable.ItemsSource = dataTable.DefaultView;
                 
                    SetNewColumnNames();
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
                reason.Width = new DataGridLength(300);
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
                    textColumn.HeaderStyle = new Style(typeof(DataGridColumnHeader));
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

                    string updateQuery = "UPDATE analysistest SET category_one = @categoryOne, category_two = @categoryTwo,category_third = @categoryThird, reason = @reason_new WHERE \"Id\" = @Id";
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

    }
}









