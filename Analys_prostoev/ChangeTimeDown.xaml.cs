﻿using Npgsql;
using System;
using System.Data;
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
        readonly long id;

        public ChangeTimeDown(DateTime start, DateTime finish, int period, string region, long id)
        {
            InitializeComponent();
            this.id = id;
            this.start = start;
            this.finish = finish;
            this.period = period;
            this.region = region;
            startDatePicker.Value = start;
            endDatePicker.Value = finish;
            Period.Text = period.ToString();
            CB_Region.SelectedItem = region;
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
            startDatePicker.IsEnabled = false;
            endDatePicker.IsEnabled = false;
            Period.IsEnabled = false;
            CB_Region.IsEnabled = false;

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


        private void ChangeDownTime(object sender, RoutedEventArgs e)
        {
            if (CB_Region.Text == string.Empty)
            {
                Close();
            }
            else
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    MainWindow main = Application.Current.MainWindow as MainWindow;
                    long id = this.id;

                    string updateQuery = "UPDATE analysis SET  status = @status WHERE \"Id\" = @id";
                    /*date_start = @dateStart, date_finish = @dateFinish, period = @period, region = @region,*/
                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, connection))
                    {
                        //updateCommand.Parameters.AddWithValue("@dateStart", startDatePicker.Value);
                        //updateCommand.Parameters.AddWithValue("@dateFinish", endDatePicker.Value);
                        //updateCommand.Parameters.AddWithValue("@period", Convert.ToInt32(Period.Text));
                        //updateCommand.Parameters.AddWithValue("@region", CB_Region.Text);   На случай если понадобися изменение по другим параметрам

                        AnalysisStatus status = CB_Status.Text == "Согласован" ? AnalysisStatus.Approved : AnalysisStatus.NotApproved;
                        updateCommand.Parameters.AddWithValue("@status", (byte)status);
                        updateCommand.Parameters.AddWithValue("@id", id);
                        updateCommand.ExecuteNonQuery();

                        main.GetSortTable();
                    }
                }

                using (NpgsqlConnection connection2 = new NpgsqlConnection(connectionString)) // Open a new connection
                {
                    connection2.Open();
                    string insertQuery = "INSERT INTO change_history (region, date_change, id_pros, modified_element) VALUES (@region, @date_change, @id_pros, @modified_element)";

                    using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, connection2))
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
