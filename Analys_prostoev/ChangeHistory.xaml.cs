using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
        }

        private void GetHistory(NpgsqlConnection connection)
        {
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

            if (selectComboBox.SelectedItem != null)
            {
                string selectedRegion = selectComboBox.SelectedItem.ToString();
                if (selectedRegion == "ХПТ" || selectedRegion == "ХПТР")
                {
                    DBContext.getHistoryString += $" AND region ILIKE @selectedRegion";
                }
                else
                {
                    DBContext.getHistoryString += $" AND region = @selectedRegionCurrent";
                    parameters.Add(new NpgsqlParameter("selectedRegionCurrent", selectedRegion));
                }
                parameters.Add(new NpgsqlParameter("selectedRegion", selectedRegion + " %"));
            }

            using (NpgsqlCommand command = new NpgsqlCommand(DBContext.getHistoryString, connection))
            {
                command.Parameters.AddRange(parameters.ToArray());

                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                NpgsqlDataReader reader = command.ExecuteReader();


                HistoryTable.ItemsSource = dataTable.DefaultView;
                SetNewColumnNames();
            }
        }
        private void SelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
            {
                connection.Open();
                GetHistory(connection);
            }
        }
        private void SetNewColumnNames()
        {
            DataGridTextColumn id_pros = (DataGridTextColumn)HistoryTable.Columns.FirstOrDefault(c => c.Header.ToString() == "id_pros");
            if (id_pros != null)
            {
                id_pros.Header = "Номер простоя";
            }
            DataGridTextColumn region = (DataGridTextColumn)HistoryTable.Columns.FirstOrDefault(c => c.Header.ToString() == "region");
            if (region != null)
            {
                region.Header = "Участок";
            }
            DataGridTextColumn date_change = (DataGridTextColumn)HistoryTable.Columns.FirstOrDefault(c => c.Header.ToString() == "date_change");
            if (date_change != null)
            {
                date_change.Header = "Дата изменения";
            }
            DataGridTextColumn modified_element = (DataGridTextColumn)HistoryTable.Columns.FirstOrDefault(c => c.Header.ToString() == "modified_element");
            if (modified_element != null)
            {

                modified_element.Header = "Изменения";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
