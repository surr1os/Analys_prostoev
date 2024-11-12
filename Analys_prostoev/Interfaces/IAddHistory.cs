using Npgsql;

namespace AnalysisDowntimes
{
    public interface IAddHistory
    {
        void AddHistory(NpgsqlCommand insertCommand, string modifiedElementegory);
    }
}
