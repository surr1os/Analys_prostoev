using System.Collections.Generic;
using static Analys_prostoev.Data.CategoryHierarchy;

namespace Analys_prostoev
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







