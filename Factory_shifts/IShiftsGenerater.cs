using Factory_shifts.Data;

namespace Factory_shifts
{
	public interface IShiftsGenerater
	{
        List<Shift> Generate(DateTime from, DateTime to, DateShift initialData);
	}
}
