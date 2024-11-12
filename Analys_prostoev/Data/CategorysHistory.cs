using System;

namespace Analys_prostoev.Data
{
	public class CategorysHistory
	{
		/// <summary>
		/// Идентификатор записи
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Тип изменений
		/// </summary>
        public string Type { get; set; }

		/// <summary>
		/// Описание изменений
		/// </summary>
        public string Title { get; set; }

		/// <summary>
		/// Дата изменений
		/// </summary>
		public DateTime CreatedDate { get; set; }
	}
}
