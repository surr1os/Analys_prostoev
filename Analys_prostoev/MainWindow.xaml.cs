using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


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

        private List<string> GetCategories()
        {
            List<string> categories = new List<string>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string queryString = "SELECT category_name FROM Category";

                using (NpgsqlCommand command = new NpgsqlCommand(queryString, connection))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string category = reader["category_name"].ToString();
                        categories.Add(category);
                    }

                    reader.Close();
                }
            }

            return categories;
        }

        private void InitializeDataGrid(DataTable dataTable)
        {
            // Добавьте остальной код инициализации таблицы (если нужно)

            // Удаляем столбец "category", если он уже существует
           

            // Создаем столбец с выпадающим списком "Категория" (если он еще не был создан)
            if (!dataTable.Columns.Contains("Категория"))
            {
                DataColumn categoryColumn = new DataColumn("Категория", typeof(string));
                dataTable.Columns.Add(categoryColumn);
            }

            // Заполняем столбец "Категория" данными из GetCategories()
            foreach (DataRow row in dataTable.Rows)
            {
                row["Категория"] = GetCategoryForRow(row); // Здесь используйте ваш метод GetCategoryForRow
            }
            if (dataTable.Columns.Contains("category"))
            {
                dataTable.Columns.Remove("category");
            }
        }
        private string GetCategoryForRow(DataRow row)
        {
            string categoryName = string.Empty;

            // Здесь вам нужно определить логику получения категории для строки (например, из столбца "Категория" в строке)
            // Для примера, предположим, что категория находится в столбце с индексом 2
            categoryName = row[9].ToString();

            return categoryName;
        }
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

                    InitializeDataGrid(dataTable); // Передаем dataTable в метод InitializeDataGrid

                    DataGridTable.ItemsSource = dataTable.DefaultView;
                }
            }
        }
    }



}

