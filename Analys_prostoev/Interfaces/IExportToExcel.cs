using Analys_prostoev.Tables;
using System.Collections.Generic;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Analys_prostoev
{
	public interface IExportToExcel
	{
		List<Analysis> GetAnalysisList(string queryString, DateTimePicker startDateTime, DateTimePicker endDateTime, ListBox selectedRegions);
		void CreateExcelFile(List<Analysis> analysisList);
	}
}
