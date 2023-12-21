using Factory_shifts.Data;
using Factory_shifts.Tables;

namespace Factory_shifts
{
	public interface IShiftsGenerater
	{
        List<Shift> Generate(DateTime from, DateTime to, DateShift initialData);
	}
}
