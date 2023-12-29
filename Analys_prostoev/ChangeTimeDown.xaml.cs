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
        readonly long _id;

        public ChangeTimeDown(long id, DateTime start, DateTime finish, int period, string region, string status, string shifts)
        {
            InitializeComponent();
            _id = id;
            startDatePicker.Value = start;
            endDatePicker.Value = finish;
            Period.Text = period.ToString();
            CB_Region.SelectedItem = region;

            CB_Status.SelectedItem = status == "Согласовано" ? Agreed : (object)NotAgreed;

            SelectShifts(shifts);
        }

        private void SelectShifts(string shifts)
        {
            switch (shifts)
            {
                case "":
                    Letter.SelectedIndex = 0;
                    break;
                case "А":
                    Letter.SelectedIndex = 1;
                    break;
                case "В":
                    Letter.SelectedIndex = 2;
                    break;
                case "С":
                    Letter.SelectedIndex = 3;
                    break;
                case "Д":
                    Letter.SelectedIndex = 4;
                    break;
            }
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

                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(DBContext.changeQuery, connection))
                    {
                        AnalysisStatus status = CB_Status.Text == "Согласован" ? AnalysisStatus.Approved : AnalysisStatus.NotApproved;
                        updateCommand.Parameters.AddWithValue("@status", (byte)status);
                        updateCommand.Parameters.AddWithValue("@id", _id);
                        updateCommand.Parameters.AddWithValue("@change_at", DateTime.Now);
                        updateCommand.Parameters.AddWithValue("@shifts", Letter.Text);
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
                        insertCommand.Parameters.AddWithValue("@id_pros", _id);
                        insertCommand.Parameters.AddWithValue("@modified_element", $"Статус изменён на \"{CB_Status.Text}\"");
                        insertCommand.ExecuteNonQuery();
                    }
                }
                Close();
            }
        }
    }
}
