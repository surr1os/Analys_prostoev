﻿using Npgsql;
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

namespace Analys_prostoev
{
 
    public partial class MainWindow : Window
    {
        // private string connectionString = "Host=10.241.224.71;Port=5432;Database=planning_dept_db;Username=postgres_10_241_224_71;Password=feDoz5Xreh";
        private string connectionString = "Host=localhost;Database=myDb;Username=postgres;Password=iqdeadzoom1r";

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
            selectDataFromTrends();
            CreateSelectRowCB();
        }

        private void selectDataFromTrends()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                // Получаем значения из таблицы trends
                using (var cmd = new NpgsqlCommand("SELECT t, v FROM trends ORDER BY t", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    DateTime dateStart = DateTime.MinValue;
                    DateTime dateFinish = DateTime.MinValue;
                    double v1 = 1;

                    while (reader.Read())
                    {
                        DateTime t = reader.GetDateTime(0);
                        double v2 = reader.GetDouble(1);

                        if (v1 == 0 && v2 == 1 && t.Millisecond == dateStart.Millisecond)
                        {
                            dateFinish = t;
                            break;
                        }

                        if (v1 == 1 && v2 == 0)
                        {
                            dateStart = t;
                        }

                        v1 = v2;
                    }

                    // Закрываем предыдущий DataReader перед выполнением следующего запроса
                    reader.Close();

                    // Вытаскиваем значение из hpt_select_trends
                    int id = 2; // Идентификатор для сравнения
                    string region = "";
                    using (var cmd2 = new NpgsqlCommand("SELECT region FROM hpt_select_trends WHERE id = @id", connection))
                    {
                        cmd2.Parameters.AddWithValue("id", id);
                        region = (string)cmd2.ExecuteScalar();

                    }

                    // Вставляем значения в таблицу analysistest
                    using (var cmd3 = new NpgsqlCommand("INSERT INTO analysistest (date_start, date_finish, region) VALUES (@date_start, @date_finish, @region)", connection))
                    {
                        cmd3.Parameters.AddWithValue("@date_start", dateStart);
                        cmd3.Parameters.AddWithValue("@date_finish", dateFinish);
                        cmd3.Parameters.AddWithValue("@region", NpgsqlTypes.NpgsqlDbType.Text, region); // Устанавливаем тип параметра 'region'
                        cmd3.ExecuteNonQuery();
                    }
                }
            }
        }






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
                    queryString += " AND TO_DATE(\"Date_start\", 'DD-MM-YYYY') >= @startDate";
                    parameters.Add(new NpgsqlParameter("startDate", NpgsqlTypes.NpgsqlDbType.Date));
                    parameters[parameters.Count - 1].Value = startDatePicker.SelectedDate;
                }

                if (endDatePicker.SelectedDate != null)
                {
                    queryString += " AND TO_DATE(\"Date_start\", 'DD-MM-YYYY') <= @endDate";
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
                        queryString += " AND (category_one IS NULL OR category_one = '') OR (category_two IS NULL OR category_two = '') OR (category_third IS NULL OR category_third = '')";
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
                reason.Header = "Причина";
            }

            DataGridTextColumn date_start = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "date_start");
            if (date_start != null)
            {
                date_start.Header = "Дата Начала";
            }
            DataGridTextColumn date_finish = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "date_finish");
            if (date_finish != null)
            {
                date_finish.Header = "Дата Финиша";
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









