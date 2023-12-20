using Analys_prostoev;
using Factory_shifts;
using Npgsql;

internal class Program
{
    private static void Main()
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
        {
            connection.Open();
            TimeShifts Shifts = new TimeShifts();
            List<long> shifts = Shifts.GetTime(connection);
            FillShiftsTable fillShifts = new();
            fillShifts.FillTable(connection, shifts);
            Console.WriteLine("Готово");
        }

    }
}