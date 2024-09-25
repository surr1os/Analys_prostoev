using Analys_prostoev.Tables;
using System.Collections.Generic;
using System.Linq;

namespace Analys_prostoev.Handlers
{
	public class PeriodSumHandler
	{
		public List<Analysis> Downtimes { get; set; }
		public PeriodSumHandler(List<Analysis> downtimes) 
		{
			Downtimes = downtimes;
		}

		public int GetPeriodsSum()
		{
			int sum = Downtimes.Sum(x => x.Period);

			return sum;
		}
	}
}
