using Factory_shifts.Tables;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory_shifts
{
    public class FillShiftsTable
    {
        private List<Shifts> shifts_sourse = new();

        public void FillTable(NpgsqlConnection connection, List<long> timeShift)
        {
            DateTime startDate = new DateTime(2023, 12, 1); // Start date for shifts
            string[] groups = new string[] { "А", "В", "С", "Д" };
            int[] groupShifts = new int[] { 2, 3, 1, 3 }; // Shifts for each group

            for (int i = 0; i < 31; i++) // December has 31 days
            {
                DateTime currentDate = startDate.AddDays(i);
                int groupIndex = i % 4; // Calculate group index based on day

                Shifts shifts = new Shifts
                {
                    Day = currentDate,
                    Letter = groups[groupIndex],
                    TimeShiftId = groupShifts[groupIndex] // Assign shift based on group index
                };

                shifts_sourse.Add(shifts);

                using (NpgsqlCommand insert = new NpgsqlCommand("Insert into shifts(day, letter, time_shift_id) values(@day, @letter, @time_shift_id)", connection))
                {
                    insert.Parameters.AddWithValue("@day", currentDate);
                    insert.Parameters.AddWithValue("@letter", groups[groupIndex]);
                    insert.Parameters.AddWithValue("@time_shift_id", groupShifts[groupIndex]);
                    insert.ExecuteNonQuery();
                }
            }
        }
    }
}
