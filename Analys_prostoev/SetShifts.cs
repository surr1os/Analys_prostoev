using Analys_prostoev.Tables;
using Factory_shifts.Tables;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Analys_prostoev
{
    public class SetShifts
    {
        public void Set(NpgsqlConnection connection)
        {
            connection.Open();

            var listShifts = GetShiftsList(connection);
            var listAnalysis = GetSortAnalysis(connection);
            var listTimeShifts = GetTimeShifts(connection);

            foreach (Analysis a in listAnalysis)
            {
                TimeSpan dateStart = a.DateStart.TimeOfDay;
                TimeSpan dateFinish = a.DateFinish.TimeOfDay;

                bool alreadyInserted = false;

                using (NpgsqlCommand checkCommand = new NpgsqlCommand())
                {
                    checkCommand.Connection = connection;
                    checkCommand.CommandText = "SELECT COUNT(*) FROM analysis WHERE shifts = @Shifts";
                    checkCommand.Parameters.AddWithValue("@Shifts", a.Shifts);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        alreadyInserted = true; // Если запись уже есть, устанавливаем флаг
                    }
                }

                if (!alreadyInserted) // Если записи нет, продолжаем выполнение
                {
                    foreach (Shift s in listShifts)
                    {
                        foreach (TimeShifts t in listTimeShifts)
                        {
                            if (a.DateStart.Date == s.Day)
                            {
                                if (s.TimeShiftId == 1)
                                {
                                    if (dateStart <= t.TimeBegin & dateStart >= t.TimeEnd)
                                    {
                                        a.Shifts = s.Letter;
                                    }
                                }
                                if (s.TimeShiftId == 2)
                                {
                                    if (dateStart >= t.TimeBegin && dateStart <= TimeSpan.Parse("23:59:59"))
                                    {
                                        a.Shifts = s.Letter;
                                    }
                                    if (dateStart >= TimeSpan.Parse("00:00:00") && dateStart <= t.TimeEnd)
                                    {
                                        a.Shifts = s.Letter;
                                    }
                                }
                            }
                        }
                    }

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO analysis(shifts) VALUES(@Shifts)";
                        command.Parameters.AddWithValue("@Shifts", a.Shifts);
                        command.ExecuteNonQuery(); // Выполняем команду вставки в базу данных
                    }
                }
            }
        }

        private List<TimeShifts> GetTimeShifts(NpgsqlConnection connection)
        {
            List<TimeShifts> timeShifts = new List<TimeShifts>();
            NpgsqlCommand selectTime = new NpgsqlCommand(DBContext.timeShifts, connection);
            using (NpgsqlDataReader reader = selectTime.ExecuteReader())
            {
                while (reader.Read())
                {
                    TimeShifts time = new TimeShifts()
                    {
                        Id = reader.GetInt64(0),
                        TimeBegin = reader.GetTimeSpan(1),
                        TimeEnd = reader.GetTimeSpan(2)
                    };
                    timeShifts.Add(time);
                }
            }
            return timeShifts;
        }

        private List<Analysis> GetSortAnalysis(NpgsqlConnection connection)
        {
            List<Analysis> analysis = new List<Analysis>();
            NpgsqlCommand test = new NpgsqlCommand("SELECT * FROM analysis where date_start >= '2023-12-01'", connection);

            using (NpgsqlDataReader reader = test.ExecuteReader())
            {
                while (reader.Read())
                {
                    Analysis shift = new Analysis()
                    {
                        Id = reader.GetInt32(0),
                        DateStart = reader.GetDateTime(1),
                        DateFinish = reader.GetDateTime(2),
                    };
                    analysis.Add(shift);
                }
            }
            return analysis;
        }

        private List<Shift> GetShiftsList(NpgsqlConnection connection)
        {
            List<Shift> shifts = new List<Shift>();
            NpgsqlCommand selectShifts = new NpgsqlCommand(DBContext.shifts, connection);

            using (NpgsqlDataReader reader = selectShifts.ExecuteReader())
            {
                while (reader.Read())
                {
                    Shift shift = new Shift()
                    {
                        Day = reader.GetDateTime(0),
                        Letter = reader.GetString(1),
                        TimeShiftId = reader.GetInt64(2)
                    };
                    shifts.Add(shift);
                }
            }
            return shifts;
        }
    }
}
