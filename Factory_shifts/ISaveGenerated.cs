using Factory_shifts.Data;

namespace Factory_shifts
{
    public interface ISaveGenerated
    {
        public void Save(List<Shift> shifts);
    }
}
