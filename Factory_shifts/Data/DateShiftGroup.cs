namespace Factory_shifts.Data
{
	public class DateShiftGroup
	{
		public string Literal { get; set; }
		public int CurrentIndexTime { get; set; }

		public int GetTime()
		{
			return CurrentIndexTime switch
			{
				1 => 1,
				2 => 2,
				3 => 3,
				4 => 3
			};
		}

		public void Increment()
		{
			if (CurrentIndexTime < 4) 
			{ 
				CurrentIndexTime++;
			}
			else if (CurrentIndexTime == 4)
			{
				CurrentIndexTime = 1;
			}
		}
	}
}
