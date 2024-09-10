using System;
using System.Reflection.Emit;
using System.Windows.Navigation;

namespace Analys_prostoev
{
    public class DBContext
    {
        //static public string connectionString = "Host=10.241.224.71;Port=5432;Database=analysis_user;Username=analysis_user;Password=71NfhRec";
		static public string connectionString = "Host=localhost;Database=Prostoi_Test;Username=postgres;Password=431Id008";

		static public string cancellationQuery = "UPDATE public.analysis SET category_one = NULL, category_two = NULL, category_third = NULL, category_fourth = NULL, reason = NULL WHERE \"Id\" = @Id;";
        static public string deleteQuery = $"DELETE FROM analysis WHERE";
        static public string selectQuery = "SELECT region FROM hpt_select";
        static public string queryString = "";
        static public string updateQuery = "UPDATE analysis SET category_one = @categoryOne, category_two = @categoryTwo,category_third = @categoryThird, category_fourth = @categoryFourth, reason = @reason_new WHERE \"Id\" = @Id";
        static public string changeQuery = "UPDATE analysis SET date_start = @dateStart, date_finish = @dateFinish, \"period\" = @period, status = @status, change_at = @change_at, shifts = @shifts WHERE \"Id\" = @id";

        static public string setChangeDateQuery = "UPDATE analysis SET change_at = @change_at WHERE \"Id\" = @id";

		static public string insertQuery = "INSERT INTO analysis (date_start, date_finish, status, period, region, is_manual, created_at, change_at, shifts) VALUES (@dateStart, @dateFinish,@status, @period, @region,  @is_manual, @created_at, @change_at, @shift)";
		
        static public string getHistoryString = "SELECT date_change, modified_element FROM change_history WHERE id_pros = @id";
        static public string insertHistory = "INSERT INTO change_history (region, date_change, id_pros, modified_element) VALUES (@region, @date_change, @id_pros, @modified_element)";
        static public string subcategoryOneQuery = "SELECT subcategory_one_name FROM subcategory_one WHERE category_name = @CategoryName";
        static public string categoryQuery = "SELECT category_name FROM category";
        static public string shifts = "select * from shifts";
        static public string timeShifts = "select * from time_shifts WHERE id < 3";

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

        static public string UpdateHalf(DateTime date_finish, int period, string shift, long id)
        {
            string updateHalf = $"update analysis SET date_finish = \'{date_finish}\', period = {period}, shifts = \'{shift}\', change_at = CURRENT_TIMESTAMP, processed = false WHERE \"Id\" = {id}";

            return updateHalf;
        }

        static public string InsertHalf(DateTime date_start, DateTime date_finish, int period, string region, string shift)
        {
            string inserthalf = $"INSERT INTO analysis (date_start, date_finish, status, period, region, is_manual, created_at, change_at, shifts) " +
                $"VALUES (\'{date_start}\', \'{date_finish}\', null, {period}, \'{region}\',  true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, \'{shift}\')";

            return inserthalf;
        } 
	}
}
