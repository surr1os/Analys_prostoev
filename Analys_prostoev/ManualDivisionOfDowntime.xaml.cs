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
		private List<AnalysisSource> sources { get; set; }
		IGetHistory _changeHistory { get; set; }
		Guid recordId { get; set; }

		#endregion

		public ManualDivisionOfDowntime(Analysis analysis)
		{
			InitializeComponent();

			Downtime = analysis;
			_changeHistory = new GlobalChangeHistory(Downtime.Region, Downtime.Id);
			sources = new List<AnalysisSource>();
			recordId = Guid.NewGuid();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DowntimeId.Content = Downtime.Id;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Division();
			Escape(sender, e);
			MessageBox.Show("Деление произошло успешно!");

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

		#region Division Logic

		private void Escape(object sender, RoutedEventArgs e)
		{
			Close();
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

				foreach (var half in GetHalfs(PointOfDivision))
				{
					var downtime = new Analysis
					{
						DateStart = half.DateStart,
						DateFinish = half.DateFinish,
						Region = half.Region,
						Period = half.Period,
						Shifts = half.Shifts,
					};

					using (NpgsqlCommand search = new NpgsqlCommand(DBContext.Search(downtime), connection))
					{
						var _resultId = (long)search.ExecuteScalar();

						GetAnalysisSourse(_resultId);
					}
				}

				foreach (var source in sources)
				{
					using (NpgsqlCommand insertToSource = new NpgsqlCommand(DBContext.SourceInsert(source), connection))
					{
						insertToSource.ExecuteNonQuery();
					}
				}

				using (NpgsqlCommand history = new NpgsqlCommand(DBContext.insertHistory, connection))
				{
					_changeHistory.AddHistory(history, $"Простой разделён. Дата конца изменена с \"{Downtime.DateFinish}\" на \"{PointOfDivision}\".");
				}
			}
		}

		private void GetAnalysisSourse(long resultId)
		{
			DateTime operationDate = DateTime.Now;

			AnalysisSource source = new AnalysisSource
			{
				RecordId = recordId,
				OperationDate = operationDate,
				ParticipantId = Downtime.Id,
				ParticipantSourseId = resultId,
				OperationType = OperationTypes.Division
			};

			sources.Add(source);
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

		#endregion
	}
}
