using Npgsql;
using System;
using System.Windows;

namespace Analys_prostoev
{
    /// <summary>
    /// Логика взаимодействия для ChangeTimeDown.xaml
    /// </summary>
    public partial class ChangeTimeDown : Window
    {
        public DateTime start;
        public DateTime finish;
        public int period;
        public string region;
        public string status;
        readonly long id;

        public ChangeTimeDown(long id, DateTime start, DateTime finish, int period, string region, string status)
        {
            InitializeComponent();
            this.id = id;
            this.start = start;
            this.finish = finish;
            this.status = status;
            this.period = period;
            this.region = region;
            startDatePicker.Value = start;
            endDatePicker.Value = finish;
            Period.Text = period.ToString();
            CB_Region.SelectedItem = region;
            if (status == "Согласовано")
                CB_Status.SelectedItem = Agreed;
            else
                CB_Status.SelectedItem = NotAgreed;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startDatePicker.IsEnabled = false;
            endDatePicker.IsEnabled = false;
            Period.IsEnabled = false;
            CB_Region.IsEnabled = false;

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


        private void ChangeDownTime(object sender, RoutedEventArgs e)
        {
            if (CB_Region.Text == string.Empty)
            {
                Close();
            }
            else
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
                {
                    connection.Open();
                    MainWindow main = Application.Current.MainWindow as MainWindow;
                    long id = this.id;

                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(DBContext.changeQuery, connection))
                    {
                        AnalysisStatus status = CB_Status.Text == "Согласован" ? AnalysisStatus.Approved : AnalysisStatus.NotApproved;
                        updateCommand.Parameters.AddWithValue("@status", (byte)status);
                        updateCommand.Parameters.AddWithValue("@id", id);
                        updateCommand.Parameters.AddWithValue("@change_at", DateTime.Now);
                        updateCommand.ExecuteNonQuery();

                        main.GetSortTable();
                    }
                }

                using (NpgsqlConnection connection2 = new NpgsqlConnection(DBContext.connectionString)) // Open a new connection
                {
                    connection2.Open();
                    using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertHistory, connection2))
                    {
                        insertCommand.Parameters.AddWithValue("@region", CB_Region.Text);
                        insertCommand.Parameters.AddWithValue("@date_change", DateTime.Now);
                        insertCommand.Parameters.AddWithValue("@id_pros", id);
                        insertCommand.Parameters.AddWithValue("@modified_element", $"Статус изменён на \"{CB_Status.Text}\"");

                        insertCommand.ExecuteNonQuery();
                    }
                }
                Close();
            }
        }
    }
}
