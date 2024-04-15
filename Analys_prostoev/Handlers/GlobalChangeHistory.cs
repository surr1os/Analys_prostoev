using Npgsql;
using System;

namespace Analys_prostoev
{
    /// <summary>
    /// Записывает историю изменений простоя.
    /// </summary>
    public class GlobalChangeHistory : IGetHistory
    {
        readonly long _id;
        public string RegionValue { get; set; }

        public GlobalChangeHistory(string regionValue, long id)
        {
            _id = id;
            RegionValue = regionValue;
        }

        /// <summary>
        /// Добавляет запись изменения в базу данных.
        /// </summary>
        /// <param name="insertCommand">Исполняет команду на добавление записи.</param>
        /// <param name="modifiedElementegory">Изменённый элемент</param>
        public void AddHistory(NpgsqlCommand insertCommand, string modifiedElementegory)
        {
            insertCommand.Parameters.AddWithValue("@region", RegionValue);
            insertCommand.Parameters.AddWithValue("@date_change", DateTime.Now);
            insertCommand.Parameters.AddWithValue("@id_pros", _id);
            insertCommand.Parameters.AddWithValue("@modified_element", modifiedElementegory);
            insertCommand.ExecuteNonQuery();
        }
    }
}
