using Factory_shifts.Data;

namespace Factory_shifts
{
	public interface IShiftsGenerater
	{
		void Generate(DateTime from, DateTime to, DateShift initialData);
	}
}
