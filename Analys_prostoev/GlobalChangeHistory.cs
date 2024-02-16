using Npgsql;
using System;

namespace Analys_prostoev
{
    public class GlobalChangeHistory : IGetHistory
    {
        readonly long _id;
        public string RegionValue { get; set; }

        public GlobalChangeHistory(string regionValue, long id)
        {
            _id = id;
            RegionValue = regionValue;
        }

        public void HistoryForAnalysis(NpgsqlCommand insertCommand, string modifiedElementegory)
        {
            insertCommand.Parameters.AddWithValue("@region", RegionValue);
            insertCommand.Parameters.AddWithValue("@date_change", DateTime.Now);
            insertCommand.Parameters.AddWithValue("@id_pros", _id);
            insertCommand.Parameters.AddWithValue("@modified_element", modifiedElementegory);  //текст изменений, который запишется в базу
            insertCommand.ExecuteNonQuery();
        }
    }
}
