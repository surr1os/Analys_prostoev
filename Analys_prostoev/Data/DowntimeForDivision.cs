using System;

namespace Analys_prostoev.Data
{
	public class DowntimeForDivision
	{
		public string DateStart { get; set; }
		public string DateFinish { get; set; }
		public string Region { get; set; }
		public int Period { get; set; }
		public string Shifts { get; set; }
	}
}
