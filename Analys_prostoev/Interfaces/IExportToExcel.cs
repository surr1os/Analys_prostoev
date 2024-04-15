using Analys_prostoev.Tables;
using System.Collections.Generic;
using Xceed.Wpf.Toolkit;

namespace Analys_prostoev
{
	public interface IExportToExcel
	{
		List<Analysis> GetAnalysisList(string queryString, DateTimePicker startDateTime, DateTimePicker endDateTime, System.Windows.Controls.ComboBox selectComboBox);
		void CreateExcelFile(List<Analysis> analysisList);
	}
}
