using Analys_prostoev;
using Factory_shifts.Tables;
using Npgsql;

namespace Factory_shifts
{
    internal class SaveGenerated : ISaveGenerated
    {
        public void Save(List<Shift> shifts)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
            {
                connection.Open();
                foreach (var shift in shifts)
                {
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO shifts (day, letter, time_shift_id ) VALUES (@Day, @Letter, @TimeShiftId)";
                        command.Parameters.AddWithValue("@Day", shift.Day);
                        command.Parameters.AddWithValue("@Letter", shift.Letter);
                        command.Parameters.AddWithValue("@TimeShiftId", Convert.ToInt64(shift.TimeShiftId));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
