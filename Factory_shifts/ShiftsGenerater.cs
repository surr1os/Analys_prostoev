using Factory_shifts.Data;
using Factory_shifts.Tables;

namespace Factory_shifts
{
	public class ShiftsGenerater : IShiftsGenerater
	{
		private List<string> literals = new List<string> { "А", "В", "С", "Д" };
		public List<Shift> Generate(DateTime from, DateTime to, DateShift initialData)
		{
			var currentData = initialData;
			var indexDate = from;
			List<Shift> shifts = new List<Shift>();
			var count = (int)(to - from).TotalDays;
			for (int i = 0; i < count; i++)
			{
				foreach (var literal in  literals)
				{
					var currentShift = currentData.Groups.Single(g => g.Literal == literal);
					shifts.Add(GetNewShit(indexDate, currentShift));
				}

				indexDate = indexDate.AddDays(1);
			}
			return shifts;
		}

		private Shift GetNewShit(DateTime date, DateShiftGroup group)
		{
			var shift = new Shift();

			group.Increment();

			shift.Day = date;
			shift.TimeShiftId = group.GetTime();
			shift.Letter = group.Literal;

			return shift;
		}
	}
}
