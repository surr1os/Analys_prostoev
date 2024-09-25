using AnalysisDowntimes.Tables;
using System.Collections.Generic;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace AnalysisDowntimes
{
	public interface IExportToExcel
	{
		List<Analysis> GetAnalysisList(string queryString, 
			DateTimePicker startDateTime, 
			DateTimePicker endDateTime, 
			ListBox selectedRegions,
			ComboBox selectedRow);
		void CreateExcelFile(List<Analysis> analysisList);
	}
}
