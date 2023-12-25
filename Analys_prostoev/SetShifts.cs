using Analys_prostoev.Tables;
using Factory_shifts.Tables;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analys_prostoev
{
    public class SetShifts
    {
        public void Set(NpgsqlConnection connection)
        {
            var listAnalysis = GetSortAnalysis(connection);
            var listShifts = GetShiftsList(connection);
            var listTimeShifts = GetTimeShifts(connection);

            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "update analysis set shifts = @Shifts where \"Id\" = @Id";
                command.Parameters.Add("@Shifts", NpgsqlDbType.Varchar);
                command.Parameters.Add("@Id", NpgsqlDbType.Integer);
                foreach (var analysis in listAnalysis)
                {
                    TimeSpan dateStart = analysis.DateStart.TimeOfDay;
                    TimeSpan dateFinish = analysis.DateFinish.TimeOfDay;
                    var shifts = listShifts.FirstOrDefault(shift => analysis.DateStart.Date == shift.Day);

                    if (shifts != null)
                    {
                        foreach (var time in listTimeShifts)
                        {
                            if (shifts.TimeShiftId == 1 && dateStart <= time.TimeBegin && dateStart >= time.TimeEnd)
                            {
                                analysis.Shifts = shifts.Letter;
                                break;
                            }

                            if (shifts.TimeShiftId == 2 && ((dateStart >= time.TimeBegin && dateStart <= TimeSpan.Parse("23:59:59")) || (dateStart >= TimeSpan.Parse("00:00:00") && dateStart <= time.TimeEnd)))
                            {
                                analysis.Shifts = shifts.Letter;
                                break;
                            }
                        }
                    }

                    command.Parameters["@Shifts"].Value = analysis.Shifts;
                    command.Parameters["@Id"].Value = analysis.Id;
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<TimeShifts> GetTimeShifts(NpgsqlConnection connection)
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

        public List<Analysis> GetSortAnalysis(NpgsqlConnection connection)
        {
            List<Analysis> analysisList = new List<Analysis>();
            NpgsqlCommand test = new NpgsqlCommand("SELECT * FROM analysis WHERE date_start >= '2023-12-01' ORDER BY date_start", connection);

            using (NpgsqlDataReader reader = test.ExecuteReader())
            {
                while (reader.Read())
                {
                    Analysis analysis = new Analysis()
                    {
                        Id = reader.GetInt32(0),
                        DateStart = reader.GetDateTime(1),
                        DateFinish = reader.GetDateTime(2),
                    };
                    analysisList.Add(analysis);
                }
            }
            return analysisList;
        }

        public List<Shift> GetShiftsList(NpgsqlConnection connection)
        {
            List<Shift> shifts = new List<Shift>();
            NpgsqlCommand selectShifts = new NpgsqlCommand(DBContext.shifts, connection);

            using (NpgsqlDataReader reader = selectShifts.ExecuteReader())
            {
                while (reader.Read())
                {
                    Shift shift = new Shift()
                    {
                        Id = reader.GetInt64(0),
                        Day = reader.GetDateTime(1),
                        Letter = reader.GetString(2),
                        TimeShiftId = reader.GetInt64(3)
                    };
                    shifts.Add(shift);
                }
            }
            return shifts;
        }
    }
}
