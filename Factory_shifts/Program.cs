using Analys_prostoev;
using Factory_shifts;
using Npgsql;

internal class Program
{
    private static void Main(string[] args)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
        {
            FillShifts Shifts = new FillShifts();
            List<long> shifts = Shifts.GetTime(connection);
            Console.WriteLine("Готово");
        }

    }
}