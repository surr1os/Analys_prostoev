using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Analys_prostoev
{
    /// <summary>
    /// Логика взаимодействия для ChangeHistory.xaml
    /// </summary>
    public partial class ChangeHistory : Window
    {
        public ChangeHistory()
        {
            InitializeComponent();
            selectComboBox.SelectionChanged += SelectComboBox_SelectionChanged;
        }

        

        //private string connectionString = "Host=10.241.224.71;Port=5432;Database=analysis_user;Username=analysis_user;Password=71NfhRec";
        private string connectionString = "Host=localhost;Database=Prostoi_Test;Username=postgres;Password=431Id008";

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
                //GetHistory(connection);
            }
        }

        private void GetHistory(NpgsqlConnection connection)
        {
            string queryString = "SELECT id_pros, region, date_change FROM change_history WHERE 1=1 ";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

            //if (startDatePicker.Value.HasValue)
            //{
            //    this.queryString += " AND date_start >= @startDate";
            //    parameters.Add(new NpgsqlParameter("startDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
            //    parameters[parameters.Count - 1].Value = startDatePicker.Value.Value;
            //}

            //if (endDatePicker.Value.HasValue)
            //{
            //    this.queryString += " AND date_start <= @endDate";
            //    parameters.Add(new NpgsqlParameter("endDate", NpgsqlTypes.NpgsqlDbType.Timestamp));
            //    parameters[parameters.Count - 1].Value = endDatePicker.Value.Value;
            //}  на случай если понадобится поиск по времени

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

            using (NpgsqlCommand command = new NpgsqlCommand(queryString, connection))
            {
                command.Parameters.AddRange(parameters.ToArray());

                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                NpgsqlDataReader reader = command.ExecuteReader();


                HistoryTable.ItemsSource = dataTable.DefaultView;
            }
        }
        private void SelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Вызов метода GetHistory
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                GetHistory(connection);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
