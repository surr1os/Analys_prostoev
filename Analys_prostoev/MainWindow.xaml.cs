using Npgsql;
using System.Collections.Generic;
using System.Data;
//using pree
    using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System;

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
                    queryString += $" AND region = @selectedRegion";
                    parameters.Add(new NpgsqlParameter("selectedRegion", selectedRegion));
                }



                using (NpgsqlCommand command = new NpgsqlCommand(queryString, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    DataGridTable.ItemsSource = dataTable.DefaultView;
                }
            }
        }
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridCellInfo cellInfo = DataGridTable.CurrentCell;

            // Получаем столбец текущей ячейки
            DataGridColumn column = cellInfo.Column;
            var columnValue = column.Header.ToString();

            // Проверяем, что столбец имеет заголовок "category"
            if (columnValue == "category_one" || columnValue == "category_two" || columnValue == "category_third")
            {
                // Получаем объект данных
                DataRowView rowView = (DataRowView)cellInfo.Item;
                // Получаем значения ячеек выбранной строки
                string categoryOneValue = rowView["category_one"].ToString();
                string categoryTwoValue = rowView["category_two"].ToString();
                string categoryThirdValue = rowView["category_third"].ToString();

                // Создаем экземпляр окна CategoryHierarchy
                var newWindow = new CategoryHierarchy();
                newWindow.categoryOneTextB.Text = categoryOneValue;
                newWindow.categoryTwoTextB.Text = categoryTwoValue;
                newWindow.categoryThirdTextB.Text = categoryThirdValue;

                // Устанавливаем родительское окно
                newWindow.ParentWindow = this;

                newWindow.Show();
            }
        }
        public void UpdateSelectedRowValues(string categoryOneValue, string categoryTwoValue, string categoryThirdValue)
        {
            // Получаем выделенную строку в таблице
            DataRowView selectedItem = DataGridTable.SelectedItem as DataRowView;
            if (selectedItem != null)
            {
                // Обновляем значения ячеек выбранной строки
                selectedItem["category_one"] = categoryOneValue;
                selectedItem["category_two"] = categoryTwoValue;
                selectedItem["category_third"] = categoryThirdValue;

                // Обновляем строку в базе данных
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE analysis SET category_one = @categoryOne, category_two = @categoryTwo, category_third = @categoryThird WHERE \"Id\" = @Id";
                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("categoryOne", categoryOneValue);
                        updateCommand.Parameters.AddWithValue("categoryTwo", categoryTwoValue);
                        updateCommand.Parameters.AddWithValue("categoryThird", categoryThirdValue);

                        int id = Convert.ToInt32(selectedItem["Id"]);
                        updateCommand.Parameters.AddWithValue("Id", id);

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Обновление значения выполнено успешно");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка при обновлении значения");
                        }
                    }
                }
            }
        }

    }
    }


     


    

