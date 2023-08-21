using Npgsql;
using System.Collections.Generic;
using System.Data;
//using pree
    using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

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

                if(selectComboBox.SelectedItem != null)
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

            // Проверяем, что столбец имеет заголовок "category"
            if (column.Header.ToString() == "category")
            {
                // Открываем новое окно
                var newWindow = new CategoryHierarchy();
                newWindow.Show();
            }
        }

    }
    }


     


    

