using Analys_prostoev.Data;
using Analys_prostoev.Tables;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Analys_prostoev
{
	/// <summary>
	/// Логика взаимодействия для ManualDivisionOfDowntime.xaml
	/// </summary>
	public partial class ManualDivisionOfDowntime : Window
	{
		#region Data
		private DateTime PointOfDivision { get; set; }
		private DowntimeForDivision FirstHalf { get; set; }
		private DowntimeForDivision LastHalf { get; set; }
		private Analysis Downtime { get; set; }
		IGetHistory _changeHistory { get; set; }
		#endregion

		public ManualDivisionOfDowntime(Analysis analysis)
		{
			InitializeComponent();

			Downtime = analysis;
			_changeHistory = new GlobalChangeHistory(Downtime.Region, Downtime.Id);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DowntimeId.Content = Downtime.Id;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Division();
			
			Escape(sender, e);
		}

		private void Division()
		{
			string updateCommand = DBContext.UpdateHalf(FirstHalf.DateFinish, FirstHalf.Period, FirstHalf.Shifts, Downtime.Id);
			string insertCommand = DBContext.InsertHalf(LastHalf.DateStart, LastHalf.DateFinish, LastHalf.Period, LastHalf.Region, LastHalf.Shifts);

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				using (NpgsqlCommand update = new NpgsqlCommand(updateCommand, connection))
				{
					update.ExecuteNonQuery();
				}

				using (NpgsqlCommand insert = new NpgsqlCommand(insertCommand, connection))
				{
					insert.ExecuteNonQuery();
				}

				using (NpgsqlCommand history = new NpgsqlCommand(DBContext.insertHistory, connection))
				{
					_changeHistory.AddHistory(history, $"Простой разделён. Дата конца изменена с \"{Downtime.DateFinish}\" на \"{PointOfDivision}\".");
				}
			}
		}

		private List<DowntimeForDivision> GetHalfs(DateTime PointOfDivision)
		{
			List<DowntimeForDivision> result = new List<DowntimeForDivision>();

			if (PointOfDivision != null)
			{
				if (GetPeriodForDivision(Downtime.DateStart, PointOfDivision) >= 5 &&
					GetPeriodForDivision(PointOfDivision, Downtime.DateFinish) >= 5)
				{
					FirstHalf = new DowntimeForDivision
					{
						DateStart = Downtime.DateStart,
						DateFinish = PointOfDivision,
						Region = Downtime.Region,
						Period = GetPeriodForDivision(Downtime.DateStart, PointOfDivision),
						Shifts = Downtime.Shifts
					};

					LastHalf = new DowntimeForDivision
					{
						DateStart = PointOfDivision,
						DateFinish = Downtime.DateFinish,
						Region = Downtime.Region,
						Period = GetPeriodForDivision(PointOfDivision, Downtime.DateFinish),
						Shifts = Downtime.Shifts
					};
				}
				else
				{
					return null;
				}

				result.Add(FirstHalf);
				result.Add(LastHalf);
			}

			return result;
		}

		private int GetPeriodForDivision(DateTime? start, DateTime? end)
		{
			TimeSpan difference_intermedia_shifts = end.Value - start.Value;
			int periodInMinutes = (int)difference_intermedia_shifts.TotalMinutes;
			return periodInMinutes;
		}

		private void DivisionPoint_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			PointOfDivision = (DateTime)DivisionPoint.Value;

			resultTable.ItemsSource = null;

			var halves = GetHalfs(PointOfDivision);
			if (halves != null)
			{
				resultTable.ItemsSource = halves;
			}
		}

		private void Escape(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
