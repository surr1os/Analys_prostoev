using Factory_shifts.Tables;

namespace Factory_shifts
{
    public interface ISaveGenerated
    {
        public void Save(List<Shift> shifts);
    }
}
