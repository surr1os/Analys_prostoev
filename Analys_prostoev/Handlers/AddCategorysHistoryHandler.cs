using AnalysisDowntimes;
using Npgsql;

namespace Analys_prostoev.Handlers
{
	public class AddCategorysHistoryHandler
	{
		private string _command { get; set; }

		public AddCategorysHistoryHandler(string insertCommand)
		{
			_command = insertCommand;
		}

		public void AddHistory()
		{
			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				using (NpgsqlCommand insert = new NpgsqlCommand(_command, connection))
				{
					insert.ExecuteNonQuery();
				}
			}
		}
	}
}
