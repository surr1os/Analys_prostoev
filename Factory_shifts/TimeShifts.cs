using Npgsql;

namespace Factory_shifts
{
    public class FillShifts
    {
        List<long> timeShift = new List<long>();
        public List<long> GetTime(NpgsqlConnection connection)
        {
            connection.Open();

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
