using System;

namespace Analys_prostoev.Data
{
	public class DowntimeForDivision
	{
		public DateTime DateStart { get; set; }
		public DateTime DateFinish { get; set; }
		public string Region { get; set; }
		public int Period { get; set; }
		public string Shifts { get; set; }
	}
}
