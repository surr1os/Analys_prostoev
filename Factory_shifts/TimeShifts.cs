using Npgsql;

namespace Factory_shifts
{
    public class TimeShifts
    {
        private List<long> timeShift = new List<long>();
        public List<long> GetTime(NpgsqlConnection connection)
        {
            using (NpgsqlCommand select = new NpgsqlCommand("Select * from time_shifts", connection))
            {
                NpgsqlDataReader reader = select.ExecuteReader();
                while (reader.Read())
                {
                    timeShift.Add((long)reader[0]);
                }
                reader.Close();
                return timeShift;
            }
        }
    }
}
