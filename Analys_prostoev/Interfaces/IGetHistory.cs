using Npgsql;

namespace Analys_prostoev
{
    public interface IGetHistory
    {
        void AddHistory(NpgsqlCommand insertCommand, string modifiedElementegory);
    }
}
