using AnalysisDowntimes.Data;
using AnalysisDowntimes.Tables;

namespace AnalysisDowntimes
{
	public class DBContext
	{
		public const string connectionString = "Host=10.241.224.71;Port=5432;Database=analysis_user;Username=analysis_user;Password=71NfhRec";
		//public const string connectionString = "Host=localhost;Database=Prostoi_Test;Username=postgres;Password=20W22W20";

		static public string cancellationQuery = "UPDATE public.analysis SET category_one = NULL, category_two = NULL, category_third = NULL, category_fourth = NULL, reason = NULL WHERE id = @Id;";

		static public string deleteQuery = $"DELETE FROM analysis WHERE";
		static public string selectQuery = "SELECT region FROM hpt_select";
		static public string queryString = "";
		static public string updateQuery = "UPDATE analysis SET category_one = @categoryOne, category_two = @categoryTwo,category_third = @categoryThird, category_fourth = @categoryFourth, reason = @reason_new WHERE id = @Id";
		static public string changeQuery = "UPDATE analysis SET date_start = @dateStart, date_finish = @dateFinish, \"period\" = @period, status = @status, change_at = @change_at, shifts = @shifts WHERE id = @id";

		static public string setChangeDateQuery = "UPDATE analysis SET change_at = @change_at WHERE id = @id";

		static public string insertQuery = "INSERT INTO analysis (date_start, date_finish, status, period, region, is_manual, created_at, change_at, shifts, external_name) VALUES (@dateStart, @dateFinish,@status, @period, @region,  @is_manual, @created_at, @change_at, @shift, @external_name)";

		#region History Command

		static public string getHistoryString = "SELECT date_change, modified_element FROM change_history WHERE id_pros = @id";
		static public string insertHistory = "INSERT INTO change_history (region, date_change, id_pros, modified_element) VALUES (@region, @date_change, @id_pros, @modified_element)";

		#endregion

		#region Shift Command

		static public string shifts = "select * from shifts";
		static public string timeShifts = "select * from time_shifts WHERE id < 3";

		#endregion

		#region Category Commands

		static public string subcategoryOneQuery = "SELECT subcategory_one_name FROM subcategory_one WHERE category_name = @CategoryName";
		static public string categoryQuery = "SELECT category_name FROM category";
		static public string selectCategoryOne = "Select category_name from category";
		static public string selectCategoryTwo = "Select subcategory_one_name from subcategory_one";
		static public string selectCategoryThird = "Select subcategory_scnd_name from";
		static public string selectCategoryFourth = "Select subcategory_third_name from";

		static public string createCategoryThird = "insert into ";
		static public string createCategoryFourth = "insert into ";

		static public string UpdateCategoryThirdCommand(string tableName, string categoryOne, string categoryTwo, string changedCategory, string newCategoryName)
		{
			string updateCommand = $"update {tableName} set subcategory_scnd_name = '{newCategoryName}' where subcategory_scnd_name = '{changedCategory}' and category_name = '{categoryOne}' and subcategory_one_name = '{categoryTwo}' and is_removed = false";

			return updateCommand;
		}

		static public string UpdateCategoryThirdInCategoryFourthTableCommand(string tableName, string categoryOne, string categoryTwo, string changedCategory, string newCategoryName)
		{
			string updateCommand = $"update {tableName} set subcategory_scnd_name = '{newCategoryName}' where subcategory_scnd_name = '{changedCategory}' and category_name = '{categoryOne}' and subcategory_one_name = '{categoryTwo}' and is_removed = false";

			return updateCommand;
		}

		static public string UpdateCategoryFourth(string tableName, string categoryOne, string categoryTwo, string categoryThird, string changedCategory, string newCategoryName)
		{
			string updateCommand = $"update {tableName} set subcategory_third_name = '{newCategoryName}' where subcategory_third_name = '{changedCategory}' and subcategory_scnd_name = '{categoryThird}' and category_name = '{categoryOne}' and subcategory_one_name = '{categoryTwo}' and is_removed = false";

			return updateCommand;
		}

		static public string NotChangeThirdCommand(string tableName, string categoryOne, string categoryTwo, string categoryThird)
		{
			string notChange = $"select not_changeable from {tableName} where category_name = '{categoryOne}' and subcategory_one_name = '{categoryTwo}' and subcategory_scnd_name = '{categoryThird}' and is_removed = false";

			return notChange;
		}

		static public string NotChangeFourthCommand(string tableName, string categoryOne, string categoryTwo, string categoryThird, string categoryFourth)
		{
			string notChange = $"select not_changeable from {tableName} where category_name = '{categoryOne}' and subcategory_one_name = '{categoryTwo}' and subcategory_scnd_name = '{categoryThird}' and subcategory_third_name = '{categoryFourth}' and is_removed = false";

			return notChange;
		}

		static public string RemoveCategoryThird(string tableName, string categoryOne, string categoryTwo, string categoryThird)
		{
			string removeCommand = $"update {tableName} set is_removed = true where subcategory_scnd_name = '{categoryThird}' and category_name = '{categoryOne}' and subcategory_one_name = '{categoryTwo}' and not_changeable = false";

			return removeCommand;
		}

		static public string SearchThirdCategory(string tableName, string categoryOne, string categoryTwo, string categoryThird)
		{
			string removedCategoryCommand = $"select subcategory_scnd_name from {tableName} where category_name = '{categoryOne}' and subcategory_one_name = '{categoryTwo}' and subcategory_scnd_name = '{categoryThird}' and is_removed = false";

			return removedCategoryCommand;
		}

		static public string RemoveCategoryThirdInFourthTable(string tableName, string categoryOne, string categoryTwo, string categoryThird)
		{
			string removedThirdCategoryCommand = $"update {tableName} set is_removed = true where subcategory_scnd_name = '{categoryThird}' and category_name = '{categoryOne}' and subcategory_one_name = '{categoryTwo}' and not_changeable = false";

			return removedThirdCategoryCommand;
		}

		static public string RemoveCategoryFourth(string tableName, string categoryOne, string categoryTwo, string categoryThird, string categoryFourth)
		{
			string removeCommand = $"update {tableName} set is_removed = true where subcategory_third_name = '{categoryFourth}' and subcategory_scnd_name = '{categoryThird}' and category_name = '{categoryOne}' and subcategory_one_name = '{categoryTwo}' and not_changeable = false";

			return removeCommand;
		}
		
		#endregion

		#region Division

		static public string UpdateHalf(string date_finish, int period, string shift, long id, string ext_name)
		{
			string updateHalf = $"update analysis SET date_finish = \'{date_finish}\'::timestamp, " +
				$"period = {period}, shifts = \'{shift}\', change_at = CURRENT_TIMESTAMP, processed = false, external_name = \'{ext_name}\' WHERE id = {id}";

			return updateHalf;
		}

		static public string InsertHalf(string date_start, string date_finish, int period, string region, string shift, string ext_name)
		{
			string inserthalf = $"INSERT INTO analysis (date_start, date_finish, status, period, region, is_manual, created_at, change_at, shifts, external_name) " +
				$"VALUES (\'{date_start}\'::timestamp, \'{date_finish}\'::timestamp, null, {period}, \'{region}\',  true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, \'{shift}\', \'{ext_name}\')";

			return inserthalf;
		}

		#endregion

		#region Combining

		static public string SourceInsert(AnalysisSource sourse)
		{
			var addDate = sourse.OperationDate.ToString("yyyy-MM-dd HH:mm:ss");

			string sourseInsert = $"Insert into analysis_source(record_id, operation_date, participant_id, participant_sourse_id, operation_type)" +
				$" values(\'{sourse.RecordId}\',\'{addDate}\'::timestamp,{sourse.ParticipantId},{sourse.ParticipantSourseId},\'{sourse.OperationType}\')";

			return sourseInsert;
		}

		static public string InsertCombiningDowntime(Analysis downtime, string ext_name)
		{
			var start = downtime.DateStart.ToString("yyyy-MM-dd HH:mm:ss");

			var end = downtime.DateFinish.ToString("yyyy-MM-dd HH:mm:ss");

			string insert = $"Insert into analysis(date_start, date_finish, status, period, region, is_manual, created_at, change_at, shifts, category_one, category_two, category_third, category_fourth, external_name) " +
				$"values(\'{start}\'::timestamp, \'{end}\'::timestamp, null, {downtime.Period}, \'{downtime.Region}\',  true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, \'{downtime.Shifts}\', " +
				$"\'{downtime.CategoryOne}\', \'{downtime.CategoryTwo}\', \'{downtime.CategoryThird}\', \'{downtime.CategoryFourth}\', \'{ext_name}\')";

			return insert;
		}

		#endregion

		#region Help Command

		static public string RemovePaticipants(long id)
		{
			string remove = $"update analysis " +
							$"SET is_removed = true " +
							$"where id = {id}";

			return remove;
		}

		static public string SearchResultDowntime(Analysis downtime)
		{
			var start = downtime.DateStart.ToString("yyyy-MM-dd HH:mm:ss");
			
			var finish = downtime.DateFinish.ToString("yyyy-MM-dd HH:mm:ss");

			string searchString = $"select id " +
								  $"from analysis where date_start = \'{start}\'::timestamp " +
								  $"and date_finish = \'{finish}\'::timestamp " +
								  $"and status is null and period = {downtime.Period} " +
								  $"and shifts = \'{downtime.Shifts}\' " +
								  $"and region = \'{downtime.Region}\'";

			return searchString;
		}

		static public string GetExternalName(string region)
		{
			string externalName = $"select region_fullname " +
								  $"from public.variables " +
								  $"where description = \'{region}\'";

			return externalName;
		}

		#endregion
	}
}
