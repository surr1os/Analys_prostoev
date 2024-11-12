using Analys_prostoev.Tables;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace Analys_prostoev
{


	public class ShiftRecordHandler
	{
		private static List<CompletedShifts> shifts = new List<CompletedShifts>();

		public static void InsertShiftRecord(DateTime start, DateTime? end, string region, int periodInMinutes, string status, NpgsqlConnection connection)
		{
			using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertQuery, connection))
			{
				insertCommand.Parameters.AddWithValue("@dateStart", start);
				insertCommand.Parameters.AddWithValue("@dateFinish", end);
				insertCommand.Parameters.AddWithValue("@period", periodInMinutes);
				insertCommand.Parameters.AddWithValue("@region", region);
				insertCommand.Parameters.AddWithValue("@status", status);
				insertCommand.Parameters.AddWithValue("@created_at", DateTime.Now);
				insertCommand.Parameters.AddWithValue("@change_at", DateTime.Now);
				insertCommand.Parameters.AddWithValue("@is_manual", true);

				string selectedShift = CalculateShiftForDateTime(start, (DateTime)end);
				insertCommand.Parameters.AddWithValue("@shift", selectedShift);
				
				insertCommand.ExecuteNonQuery();
			}
		}

		public static void InitializeShifts(NpgsqlConnection connection)
		{
			using (NpgsqlCommand command = new NpgsqlCommand("SELECT s.id AS s_id, s.day + ts.time_begin AS date_start_s, " +
				"CASE WHEN ts.time_end = '08:00:00' THEN s.day + ts.time_end + INTERVAL '1 day' ELSE s.day + ts.time_end END AS date_end_s, " +
				"s.letter AS shift_letter FROM public.shifts s JOIN public.time_shifts ts ON s.time_shift_id = ts.id " +
				"where ts.time_end is not null and ts.time_begin is not null", connection))
			{
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						CompletedShifts shift = new CompletedShifts()
						{
							Id = Convert.ToInt32(reader["s_id"]),
							Start = Convert.ToDateTime(reader["date_start_s"]),
							End = Convert.ToDateTime(reader["date_end_s"]),
							Letter = Convert.ToString(reader["shift_letter"])
						};
						shifts.Add(shift);
					}
				}
			}
		}
		public static string CalculateShiftForDateTime(DateTime start, DateTime end)
		{
			foreach (var shift in shifts)
			{
				if (start >= shift.Start && end <= shift.End)
				{
					return shift.Letter;
				}
			}
			return null;
		}
	}
}
