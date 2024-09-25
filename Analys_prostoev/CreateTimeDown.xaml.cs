using Npgsql;
using System;
using System.Windows;
using static AnalysisDowntimes.ChangeTimeDown;

namespace AnalysisDowntimes
{
    /// <summary>
    /// Логика взаимодействия для CreateTimeDown.xaml
    /// </summary>
    public partial class CreateTimeDown : Window
    {
        private DateTime start_first_shift;
        private DateTime end_first_shift;

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
                            if (reader["region"] != null 
                                && reader["region"].ToString() != "ХПТ" 
                                && reader["region"].ToString() != "ХПТР")
                            {
                                CB_Region.Items.Add(reader["region"].ToString());
                            }
                        }
                    }
                }
            }
        }

        private void CreateDownTime(object sender, RoutedEventArgs e)
        {
            DateTime? start = startDatePicker.Value;
            DateTime? end = endDatePicker.Value;


            using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
            {
                connection.Open();
                ShiftRecordHandler.InitializeShifts(connection);
                string status = GetStatusFromComboBox(CB_Status.Text);

                if (startDatePicker.Value != null && endDatePicker.Value != null && CB_Region.SelectedItem != null)
                {
                    CheckingForDivision(start, end, connection, status);
                    MainWindow main = Application.Current.MainWindow as MainWindow;
                    main.GetTable();
                    Hide();
                }
                else
                {
                    MessageBox.Show("Основные параметры не заполнены!");
                    return;
                }
               
            }
        }

        # region Division
        private void CheckingForDivision(DateTime? start, DateTime? end, NpgsqlConnection connection, string status)
        {
            // добавляем простой если:
            if ((start.Value > start.Value.Date.AddHours(8) && start.Value < start.Value.Date.AddHours(20)
            && end.Value < start.Value.Date.AddHours(20)) //к концу утренней смены ничего не добавляем
            || (start.Value > start.Value.Date.AddHours(20) && start.Value < start.Value.Date.AddDays(1).AddHours(8)
            && end.Value < start.Value.Date.AddDays(1).AddHours(8))
            || (start.Value > start.Value.Date && start.Value < start.Value.Date.AddHours(8)
            && end.Value < start.Value.Date.AddHours(8))) //к концу ночной смены прибавляем 1 день.
            {
                ShiftRecordHandler.InsertShiftRecord(start.Value, end.Value, CB_Region.Text, GetPeriodForDivision(start, end), status, connection);
            }
            else
            {
                Division(start, end, connection, status);
            }
        }
        private void Division(DateTime? start, DateTime? end, NpgsqlConnection connection, string status)
        {
            if (start.Value > start.Value.Date.AddHours(8) && start.Value < start.Value.Date.AddHours(20)
                && end.Value > start.Value.Date.AddHours(20))
            {
                start_first_shift = start.Value.Date.AddHours(8);
                end_first_shift = start_first_shift.AddHours(12);

                DivisionСycle(start, end, connection, status);
            }
            else if ((start.Value > start.Value.Date.AddHours(20) && start.Value < start.Value.Date.AddDays(1).AddHours(8)
                && end.Value > start.Value.Date.AddDays(1).AddHours(8)))
            {
                start_first_shift = start.Value.Date.AddHours(8);
                end_first_shift = start_first_shift.AddHours(12);

                DivisionСycle(start, end, connection, status);
            }
            else if ((start.Value > start.Value.Date && start.Value < start.Value.Date.AddHours(8)
                && end.Value < start.Value.Date.AddHours(8)))
            {
                start_first_shift = start.Value.Date.AddHours(8);
                end_first_shift = start_first_shift.AddHours(12);

                DivisionСycle(start, end, connection, status);
            }
        }
        private void DivisionСycle(DateTime? start, DateTime? end, NpgsqlConnection connection, string status)
        {
            ShiftRecordHandler.InsertShiftRecord(start.Value, end_first_shift, CB_Region.Text, GetPeriodForDivision(start, end_first_shift), status, connection);

            start_first_shift = start_first_shift.AddHours(12);
            end_first_shift = end_first_shift.AddHours(12);

            while (end > end_first_shift)
            {
                ShiftRecordHandler.InsertShiftRecord(start_first_shift, end_first_shift, CB_Region.Text, 720, status, connection);

                start_first_shift = start_first_shift.AddHours(12);
                end_first_shift = end_first_shift.AddHours(12);
            }
            ShiftRecordHandler.InsertShiftRecord(start_first_shift, end.Value, CB_Region.Text, GetPeriodForDivision(start_first_shift, end), status, connection);
        }
        #endregion

        #region GetParameters
        private string GetStatusFromComboBox(string selectedStatus)
        {
            switch (selectedStatus)
            {
                case "Согласован":
                    return AnalysisStatus.Approved.ToString();
                case "Не согласован":
                    return AnalysisStatus.NotApproved.ToString();
                default:
                    return AnalysisStatus.Nothing.ToString();
            }
        }
        private int GetPeriodForDivision(DateTime? start, DateTime? end)
        {
            TimeSpan difference_intermedia_shifts = end.Value - start.Value;
            int periodInMinutes = (int)difference_intermedia_shifts.TotalMinutes;
            return periodInMinutes;
        }
        private void GetPeriodForLable(object sender, RoutedEventArgs e)
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
        #endregion
    }
}
