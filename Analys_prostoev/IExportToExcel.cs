using Analys_prostoev.Tables;
using System.Collections.Generic;

namespace Analys_prostoev
{
	public interface IExportToExcel
	{
		List<Analysis> GetAnalysisList(string queryString);
		void CreateExcelFile(List<Analysis> analysisList);
	}
}
