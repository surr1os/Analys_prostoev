using System.Collections.Generic;
using static AnalysisDowntimes.Data.CategoryHierarchy;

namespace AnalysisDowntimes
{
	public partial class CategoryHierarchy
    {
        public class SubcategorySecond
        {
            public string SubcategorySecondName { get; set; }

            public List<SubcategoryThird> SubcategoriesThird { get; set; }

		}
    }
}







