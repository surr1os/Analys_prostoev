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

namespace Analys_prostoev
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string queryString = "SELECT * FROM analysis WHERE 1=1";
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
                    queryString += $" AND region LIKE @selectedRegion";
                    parameters.Add(new NpgsqlParameter("selectedRegion", selectedRegion + "%"));
                }



                using (NpgsqlCommand command = new NpgsqlCommand(queryString, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    DataGridTable.ItemsSource = dataTable.DefaultView;
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

                    DataGridTextColumn date_start = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "Date_start");
                    if (date_start != null)
                    {
                        date_start.Header = "Дата Начала";
                    }
                    DataGridTextColumn date_finish = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "Date_finish");
                    if (date_finish != null)
                    {
                        date_finish.Header = "Дата Финиша";
                    }
                    DataGridTextColumn period = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "period");
                    if (period != null)
                    {
                        period.Header = "Период";
                    }
                    DataGridTextColumn change_start = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "Change_start");
                    if (change_start != null)
                    {
                        change_start.Header = "Измененный Старт";
                    }
                    DataGridTextColumn change_finish = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "Change_finish");
                    if (change_finish != null)
                    {
                        change_finish.Header = "Измененный Финиш";
                    }
                    DataGridTextColumn condition = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "condition");
                    if (condition != null)
                    {
                        condition.Header = "Состояние";
                    }
                    DataGridTextColumn device = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "device");
                    if (period != null)
                    {
                        period.Header = "Устройство";
                    }
                    DataGridTextColumn coefficient = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "coefficient");
                    if (coefficient != null)
                    {
                        coefficient.Header = "Коэффициент";
                    }
                    DataGridTextColumn note = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "note");
                    if (note != null)
                    {
                        note.Header = "Комментарий";

                        DataGridTextColumn region = (DataGridTextColumn)DataGridTable.Columns.FirstOrDefault(c => c.Header.ToString() == "region");
                        if (region != null)
                        {
                            region.Header = "Участок";
                        }
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


                        // Устанавливаем родительское окно


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









