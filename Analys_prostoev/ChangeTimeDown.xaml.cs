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
        private string RegionValue { get; set; }
        private string originalStatus;
        private DateTime? originalStartDate;
        private DateTime? originalEndDate;
        private string originalPeriod;

        public ChangeTimeDown(long id, DateTime start, DateTime finish, int period, string region, string status, string shifts)
        {
            InitializeComponent();
            _id = id;
            startDatePicker.Value = start;
            endDatePicker.Value = finish;
            Period.Text = period.ToString();
            CB_Region.SelectedItem = region;
            RegionValue = region;

            CB_Status.SelectedItem = status == "Согласовано" ? Agreed : (object)NotAgreed;

            SelectShifts(shifts);

            originalStatus = status;
            originalStartDate = start;
            originalEndDate = finish;
            originalPeriod = period.ToString();
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
            Nothing = 2,
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
            Period.IsEnabled = false;
            CB_Region.IsEnabled = false;

            info.Content = $"Простой номер: {_id}";

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

        private void ChangeDownTime(object sender, RoutedEventArgs e)
        {
            IGetHistory changeHistory = new GlobalChangeHistory(RegionValue, _id);

            if (CB_Region.Text == string.Empty)
            {
                Close();
            }
            else
            {
                bool isStatusChanged = CB_Status.Text != originalStatus;
                bool isStartDateChanged = startDatePicker.Value != originalStartDate;
                bool isEndDateChanged = endDatePicker.Value != originalEndDate;
                bool isPeriodChanged = Period.Text != originalPeriod;

                if (!isStatusChanged && !isStartDateChanged && !isEndDateChanged && !isPeriodChanged)
                {
                    Close(); // Закрываем окно, если изменений не было
                    return;
                }

                using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
                {
                    connection.Open();
                    MainWindow main = Application.Current.MainWindow as MainWindow;

                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(DBContext.changeQuery, connection))
                    {
                        AnalysisStatus status = CB_Status.Text == "Согласован" ? AnalysisStatus.Approved : AnalysisStatus.NotApproved;

                        if (isStatusChanged)
                        {
                            updateCommand.Parameters.AddWithValue("@status", (byte)status);
                        }
                        else updateCommand.Parameters.AddWithValue("@status", originalStatus);

                        if (isStartDateChanged)
                        {
                            updateCommand.Parameters.AddWithValue("@dateStart", startDatePicker.Value);
                        }
                        else updateCommand.Parameters.AddWithValue("@dateStart", originalStartDate);

                        if (isEndDateChanged)
                        {
                            updateCommand.Parameters.AddWithValue("@dateFinish", endDatePicker.Value);
                        }
                        else updateCommand.Parameters.AddWithValue("@dateFinish", originalEndDate);

                        if (isPeriodChanged)
                        {
                            updateCommand.Parameters.AddWithValue("@period", Convert.ToInt32(Period.Text));
                        }
                        else updateCommand.Parameters.AddWithValue("@period", Convert.ToInt32(originalPeriod));

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

                    if (isStatusChanged)
                    {
                        using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertHistory, connection2))
                        {
                            changeHistory.HistoryForAnalysis(insertCommand, $"Статус изменён на \"{CB_Status.Text}\"");
                        }
                    }
                    if (isStartDateChanged)
                    {
                        using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertHistory, connection2))
                        {
                            changeHistory.HistoryForAnalysis(insertCommand, $"Дата окончания изменена c \"{originalStartDate}\" на \"{startDatePicker.Value}\"");
                        }
                    }
                    if (isEndDateChanged)
                    {
                        using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertHistory, connection2))
                        {
                            changeHistory.HistoryForAnalysis(insertCommand, $"Дата окончания изменена c \"{originalEndDate}\" на \"{endDatePicker.Value}\"");
                        }
                    }
                    if (isPeriodChanged)
                    {
                        using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertHistory, connection2))
                        {
                            changeHistory.HistoryForAnalysis(insertCommand, $"Период изменён на \"{Period.Text}\"");
                        }
                    }

                }

                Close();
            }
        }
    }
}
