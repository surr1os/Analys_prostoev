using Npgsql;
using System;
using System.Windows;
using static Analys_prostoev.ChangeTimeDown;

namespace Analys_prostoev
{
    /// <summary>
    /// Логика взаимодействия для CreateTimeDown.xaml
    /// </summary>
    public partial class CreateTimeDown : Window
    {
        public CreateTimeDown()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Period.IsEnabled = false;
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
        private void GetPeriod(object sender, RoutedEventArgs e)
        {
            if (startDatePicker.Value != null && endDatePicker.Value != null)
            {
                DateTime? start = startDatePicker.Value;
                DateTime? end = endDatePicker.Value;

                TimeSpan difference = end.Value - start.Value;
                int periodInMinutes = (int)difference.TotalMinutes;

                Period.Text = periodInMinutes.ToString();
            }
            else return;
        }

        private void CreateDownTime(object sender, RoutedEventArgs e)
        {
            DateTime? start = startDatePicker.Value;
            DateTime? end = endDatePicker.Value;

            TimeSpan difference = end.Value - start.Value;
            int periodInMinutes = (int)difference.TotalMinutes;

            using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
            {
                connection.Open();
                MainWindow main = Application.Current.MainWindow as MainWindow;



                using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@dateStart", startDatePicker.Value);
                    insertCommand.Parameters.AddWithValue("@dateFinish", endDatePicker.Value);
                    insertCommand.Parameters.AddWithValue("@period", periodInMinutes);
                    insertCommand.Parameters.AddWithValue("@region", CB_Region.Text);

                    if (CB_Status.Text == "Согласован")
                    {
                        AnalysisStatus status = AnalysisStatus.Approved;
                        insertCommand.Parameters.AddWithValue("@status", status.ToString());
                    }
                    else if (CB_Status.Text == "Не согласован")
                    {
                        AnalysisStatus status = AnalysisStatus.NotApproved;
                        insertCommand.Parameters.AddWithValue("@status", status.ToString());
                    }
                    else
                    {
                        AnalysisStatus status = AnalysisStatus.Nothing;
                        insertCommand.Parameters.AddWithValue("@status", status.ToString());
                    }

                    insertCommand.Parameters.AddWithValue("@created_at", DateTime.Now);
                    insertCommand.Parameters.AddWithValue("@change_at", DateTime.Now);
                    insertCommand.Parameters.AddWithValue("@is_manual", true);
                    string selectedShift = CalculateShiftForDateTime(Convert.ToDateTime(startDatePicker.Value), Convert.ToDateTime(endDatePicker.Value), connection);
                    insertCommand.Parameters.AddWithValue("@shift", selectedShift);
                    insertCommand.ExecuteNonQuery();
                    main.GetSortTable();
                    Hide();
                }
            }
        }

        // Метод для определения подходящей смены на основе даты
        private string CalculateShiftForDateTime(DateTime start, DateTime end, NpgsqlConnection connection)
        {
            SetShifts setShifts = new SetShifts();
            var listShifts = setShifts.GetShiftsList(connection);
            foreach (var time in listShifts)
            {
                if (start.Date == time.Day)
                {
                    if (time.TimeShiftId == 1
                        && start.TimeOfDay >= TimeSpan.Parse("08:00:00")
                        && end.TimeOfDay <= TimeSpan.Parse("20:00:00"))
                    {
                        return time.Letter;
                    }

                    if (time.TimeShiftId == 2
                        && ((start.TimeOfDay >= TimeSpan.Parse("20:00:00")
                        && start.TimeOfDay <= TimeSpan.Parse("23:59:59"))
                        || (start.TimeOfDay >= TimeSpan.Parse("00:00:00")
                        && start.TimeOfDay <= TimeSpan.Parse("08:00:00"))))
                    {
                        return time.Letter;
                    }
                }
            }
            return null; // Если не найдена подходящая смена, возвращаем null.
        }
    }
}
