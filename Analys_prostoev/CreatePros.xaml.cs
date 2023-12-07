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

        private enum AnalisisWasCreated
        {
            manually = 1,
            auto = 0
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
                            CB_Region.Items.Add(reader["region"].ToString());
                        }
                    }
                }
            }
        }

        private void CreateDownTime(object sender, RoutedEventArgs e)
        {

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO analysis (date_start, date_finish, period, region, status, created_at)" +
                    " VALUES (@dateStart, @dateFinish, @period, @region, @status, @created_at)";

                using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@dateStart", startDatePicker.Value);
                    insertCommand.Parameters.AddWithValue("@dateFinish", endDatePicker.Value);
                    insertCommand.Parameters.AddWithValue("@period", Convert.ToInt32(Period.Text));
                    insertCommand.Parameters.AddWithValue("@region", CB_Region.Text);

                    AnalysisStatus status = CB_Status.Text == "Согласован" ? AnalysisStatus.Approved : AnalysisStatus.NotApproved;
                    insertCommand.Parameters.AddWithValue("@status", (byte)status);
                    insertCommand.Parameters.AddWithValue("@created_at", 1);
                    insertCommand.ExecuteNonQuery();
                    Hide();
                }
            }
        }
    }
}
