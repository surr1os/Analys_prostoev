using Factory_shifts.Data;

namespace Factory_shifts.Intefaces
{
    public interface IShiftsGenerater
    {
        List<Shift> Generate(DateTime from, DateTime to, DateShift initialData);
    }
}
