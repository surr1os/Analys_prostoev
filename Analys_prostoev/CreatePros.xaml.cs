using Npgsql;
using System;
using System.Windows;

namespace Analys_prostoev
{
    /// <summary>
    /// Логика взаимодействия для CreatePros.xaml
    /// </summary>
    public partial class CreatePros : Window
    {
        public CreatePros()
        {
            InitializeComponent();
        }

        public enum AnalysisStatus
        {
            Approved = 1,
            NotApproved = 0
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
                            CB_Region.Items.Add(reader["region"].ToString());
                        }
                    }
                }
            }
        }

        private void CreateDownTime(object sender, RoutedEventArgs e)
        {

            using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
            {
                connection.Open();
                MainWindow main = Application.Current.MainWindow as MainWindow;

                using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@dateStart", startDatePicker.Value);
                    insertCommand.Parameters.AddWithValue("@dateFinish", endDatePicker.Value);
                    insertCommand.Parameters.AddWithValue("@period", Convert.ToInt32(Period.Text));
                    insertCommand.Parameters.AddWithValue("@region", CB_Region.Text);

                    AnalysisStatus status = CB_Status.Text == "Согласован" ? AnalysisStatus.Approved : AnalysisStatus.NotApproved;
                    insertCommand.Parameters.AddWithValue("@status", (byte)status);
                    insertCommand.Parameters.AddWithValue("@is_manual", true);
                    insertCommand.ExecuteNonQuery();
                    main.GetSortTable();
                    Hide();
                }
            }
        }
    }
}
