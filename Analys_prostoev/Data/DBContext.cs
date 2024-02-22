namespace Analys_prostoev
{
    public class DBContext
    {
        static public string connectionString = "Host=10.241.224.71;Port=5432;Database=analysis_user;Username=analysis_user;Password=71NfhRec";
        //static public string connectionString = "Host=localhost;Database=Prostoi_Test;Username=postgres;Password=431Id008";

        static public string cancellationQuery = "UPDATE public.analysis SET category_one = NULL, category_two = NULL, category_third = NULL, reason = NULL WHERE \"Id\" = @Id;";
        static public string deleteQuery = $"DELETE FROM analysis WHERE";
        static public string selectQuery = "SELECT region FROM hpt_select";
        static public string queryString = "";
        static public string updateQuery = "UPDATE analysis SET category_one = @categoryOne, category_two = @categoryTwo,category_third = @categoryThird, reason = @reason_new WHERE \"Id\" = @Id";
        static public string changeQuery = "UPDATE analysis SET date_start = @dateStart, date_finish = @dateFinish, \"period\" = @period, status = @status, change_at = @change_at, shifts = @shifts WHERE \"Id\" = @id";
        static public string insertQuery = "INSERT INTO analysis (date_start, date_finish, status, period, region, is_manual, created_at, change_at, shifts) VALUES (@dateStart, @dateFinish,@status, @period, @region,  @is_manual, @created_at, @change_at, @shift)";
        static public string getHistoryString = "SELECT date_change, modified_element FROM change_history WHERE id_pros = @id";
        static public string insertHistory = "INSERT INTO change_history (region, date_change, id_pros, modified_element) VALUES (@region, @date_change, @id_pros, @modified_element)";
        static public string subcategoryOneQuery = "SELECT subcategory_one_name FROM Subcategory_one WHERE category_name = @CategoryName";
        static public string categoryQuery = "SELECT category_name FROM Category";
        static public string shifts = "select * from shifts";
        static public string timeShifts = "select * from time_shifts WHERE id < 3";
    }
}
