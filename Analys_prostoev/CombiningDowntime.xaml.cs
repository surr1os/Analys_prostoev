using AnalysisDowntimes.Data;
using AnalysisDowntimes.Tables;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AnalysisDowntimes
{
	/// <summary>
	/// Логика взаимодействия для CombiningDowntime.xaml
	/// </summary>
	public partial class CombiningDowntime : Window
	{
		#region Data
		private List<Analysis> _downtimes { get; set; }
		private Analysis validDowntime { get; set; }
		private Analysis firstDowntime { get; set; }
		private List<AnalysisSource> sources { get; set; }

		MainWindow main = Application.Current.MainWindow as MainWindow;
		#endregion

		private long _resultId;

		public CombiningDowntime(List<Analysis> downtimes)
		{
			InitializeComponent();

			_downtimes = downtimes;
			sources = new List<AnalysisSource>();
			firstDowntime = _downtimes.First();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			GetDowntimeId();
			GetDowntimeParameters();
			GetCombiningDowntime(firstDowntime);
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			InsertCombiningDowntime();
			Close();
			MessageBox.Show($"Простои объединены.\nИдентификатор новой записи: {_resultId}");

			main.GetTable();
		}

		#region Combining Logic

		private void GetDowntimeId()
		{
			foreach (var downtime in _downtimes)
			{
				DowntimeNumbers.Items.Add(downtime.Id);
			}
		}

		private void GetDowntimeParameters()
		{
			start.Content = _downtimes.Min(x => x.DateStart).ToString("dd.MM.yyyy HH:mm:ss");
			finish.Content = _downtimes.Max(x => x.DateFinish).ToString("dd.MM.yyyy HH:mm:ss");

			TimeSpan differense = _downtimes.Max(x => x.DateFinish) - _downtimes.Min(x => x.DateStart);

			period.Content = differense.TotalMinutes;

			if ((int)differense.TotalMinutes == 720)
			{
				period.Content += " max.";
			}

			shift.Content = firstDowntime.Shifts;
			region.Content = firstDowntime.Region;
		}

		private void GetCombiningDowntime(Analysis firstDowntime)
		{
			TimeSpan differense = _downtimes.Max(x => x.DateFinish) - _downtimes.Min(x => x.DateStart);

			validDowntime = new Analysis
			{
				DateStart = _downtimes.Min(x => x.DateStart),
				DateFinish = _downtimes.Max(y => y.DateFinish),
				Region = firstDowntime.Region,
				Shifts = firstDowntime.Shifts,
				Period = (int)differense.TotalMinutes,
			};

			if (HasAnyCategories())
			{
				validDowntime.CategoryOne = _downtimes.Where(d => !string.IsNullOrEmpty(d.CategoryOne))
					.Select(d => d.CategoryOne).FirstOrDefault();

				validDowntime.CategoryTwo = _downtimes.Where(d => !string.IsNullOrEmpty(d.CategoryTwo))
					.Select(d => d.CategoryTwo).FirstOrDefault();

				validDowntime.CategoryThird = _downtimes.Where(d => !string.IsNullOrEmpty(d.CategoryThird))
					.Select(d => d.CategoryThird).FirstOrDefault();

				validDowntime.CategoryFourth = _downtimes.Where(d => !string.IsNullOrEmpty(d.CategoryFourth))
					.Select(d => d.CategoryFourth).FirstOrDefault();
			}
		}

		private bool HasAnyCategories()
		{
			return _downtimes.Any(d =>
				!string.IsNullOrEmpty(d.CategoryOne) &&
				!string.IsNullOrEmpty(d.CategoryTwo) &&
				!string.IsNullOrEmpty(d.CategoryThird) &&
				!string.IsNullOrEmpty(d.CategoryFourth));
		}

		private void GetAnalysisSourse(long resultId)
		{
			Guid recordId = Guid.NewGuid();

			DateTime operationDate = DateTime.Now;

			foreach (var downtime in _downtimes)
			{
				AnalysisSource source = new AnalysisSource
				{
					RecordId = recordId,
					OperationDate = operationDate,
					ParticipantId = downtime.Id,
					ParticipantSourseId = resultId,
					OperationType = OperationTypes.Combining
				};

				sources.Add(source);
			}
		}

		private void InsertCombiningDowntime()
		{
			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				using (NpgsqlCommand insertToAnalysis = new NpgsqlCommand(DBContext.InsertCombiningDowntime(validDowntime), connection))
				{
					insertToAnalysis.ExecuteNonQuery();
				}

				using (NpgsqlCommand search = new NpgsqlCommand(DBContext.Search(validDowntime), connection))
				{
					 _resultId = (long)search.ExecuteScalar();

					GetAnalysisSourse(_resultId);
				}

				foreach (var source in sources)
				{
					using (NpgsqlCommand insertToSource = new NpgsqlCommand(DBContext.SourceInsert(source), connection))
					{
						insertToSource.ExecuteNonQuery();
					}
				}

				foreach (var downtime in _downtimes)
				{
					using (NpgsqlCommand remove = new NpgsqlCommand(DBContext.RemovePaticipants(downtime.Id), connection))
					{
						remove.ExecuteNonQuery();
					}
				}
			}
		}

		#endregion
	}
}
