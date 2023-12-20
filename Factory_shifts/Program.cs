using Analys_prostoev;
using Factory_shifts;
using Factory_shifts.Data;
using Npgsql;

internal class Program
{
	static void Main()
	{
		DateShift initialData = new DateShift
		{
			Groups = new List<DateShiftGroup>
			{
				new DateShiftGroup
				{
					 CurrentIndexTime = 1, Literal = "А"
				},
				new DateShiftGroup
				{
					 CurrentIndexTime = 2, Literal = "В"
				},
				new DateShiftGroup
				{
					 CurrentIndexTime = 4, Literal = "С"
				},
				new DateShiftGroup
				{
					 CurrentIndexTime = 3, Literal = "Д"
				}
			}
		};
		ShiftsGenerater shiftsGenerater = new ShiftsGenerater();
		shiftsGenerater.Generate(DateTime.Parse("2023-12-01"), DateTime.Parse("2024-12-31"), initialData);
		//using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
		//{
		//connection.Open();
		//TimeShifts Shifts = new TimeShifts();
		//List<long> shifts = Shifts.GetTime(connection);
		//FillShiftsTable fillShifts = new();
		//fillShifts.FillTable(connection, shifts);
		//Console.WriteLine("Готово");
		//}
	}
}