using Npgsql;

namespace Analys_prostoev
{
    public interface IGetHistory
    {
        void HistoryForAnalysis(NpgsqlCommand insertCommand, string modifiedElementegory);
    }
}
