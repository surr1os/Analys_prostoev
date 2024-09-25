using Npgsql;

namespace AnalysisDowntimes
{
    public interface IGetHistory
    {
        void AddHistory(NpgsqlCommand insertCommand, string modifiedElementegory);
    }
}
