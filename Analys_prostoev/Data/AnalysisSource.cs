using System;

namespace Analys_prostoev.Data
{
	public class AnalysisSource
	{
		public Guid RecordId { get; set; }
		public DateTime OperationDate { get; set; }
		public long ParticipantId { get; set; }
		public long ParticipantSourseId { get; set; }
		public string OperationType { get; set; }
	}
}
