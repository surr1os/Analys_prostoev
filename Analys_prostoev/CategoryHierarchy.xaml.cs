using Analys_prostoev.Data;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static Analys_prostoev.Data.CategoryHierarchy;

namespace Analys_prostoev
{
	public partial class CategoryHierarchy : Window
	{
		readonly long _id;
		private string originalCategoryOne;
		private string originalCategoryTwo;
		private string originalCategoryThird;
		private string originalCategoryFourth;
		public string RegionValue { get; set; }

		public MainWindow ParentWindow { get; set; }

		public CategoryHierarchy(string regionValue, long id, string CategoryOne, string CategoryTwo, string CategoryThird, string CategoryFourth)
		{
			RegionValue = regionValue;
			InitializeComponent();

			_id = id;
			originalCategoryOne = CategoryOne;
			originalCategoryTwo = CategoryTwo;
			originalCategoryThird = CategoryThird;
			originalCategoryFourth = CategoryFourth;


			List<Category> categories = GetCategories(DBContext.connectionString);

			TreeViewCategories.ItemsSource = categories;

		}


		private void CategoryHistory(string CategoryOne, string CategoryTwo, string CategoryThird, string CategoryFourth)
		{
			bool isCategoryOneChange = CategoryOne != originalCategoryOne;
			bool isCategoryTwoChange = CategoryTwo != originalCategoryTwo;
			bool isCategoryThirdChange = CategoryThird != originalCategoryThird;
			bool isCategoryFourthChange = CategoryFourth != originalCategoryFourth;

			IGetHistory changeHistory = new GlobalChangeHistory(RegionValue, _id);

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				if (isCategoryOneChange)
				{
					using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertHistory, connection))
					{
						changeHistory.AddHistory(insertCommand, $"Категория 1 ур. изменена на \"{CategoryOne}\"");
					}
				}
				if (isCategoryTwoChange)
				{
					using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertHistory, connection))
					{
						changeHistory.AddHistory(insertCommand, $"Категория 2 ур. изменена на \"{CategoryTwo}\"");
					}
				}
				if (isCategoryThirdChange)
				{
					using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertHistory, connection))
					{
						changeHistory.AddHistory(insertCommand, $"Категория 3 ур. изменена на \"{CategoryThird}\"");
					}
				}
				if (isCategoryFourthChange)
				{
					using (NpgsqlCommand insertCommand = new NpgsqlCommand(DBContext.insertHistory, connection))
					{
						changeHistory.AddHistory(insertCommand, $"Категория 4 ур. изменена на \"{CategoryFourth}\"");
					}

				}

			}
		}


		private List<Category> GetCategories(string connectionString)
		{
			List<Category> categories = new List<Category>();

			using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
			{
				connection.Open();

				// Запрос для получения данных из таблицы Category
				using (NpgsqlCommand categoryCommand = new NpgsqlCommand(DBContext.categoryQuery, connection))
				{
					using (NpgsqlDataReader categoryReader = categoryCommand.ExecuteReader())
					{
						while (categoryReader.Read())
						{
							string categoryName = categoryReader["category_name"].ToString();

							Category category = new Category
							{
								CategoryName = categoryName,
								SubcategoriesOne = GetSubcategoriesOne(connectionString, categoryName)
							};

							categories.Add(category);
						}
					}
				}
			}

			return categories;
		}

		private List<SubcategoryOne> GetSubcategoriesOne(string connectionString, string categoryName)
		{
			List<SubcategoryOne> subcategoriesOne = new List<SubcategoryOne>();

			using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
			{
				connection.Open();

				// Запрос для получения данных из таблицы Subcategory_one по заданной категории

				using (NpgsqlCommand subcategoryOneCommand = new NpgsqlCommand(DBContext.subcategoryOneQuery, connection))
				{
					subcategoryOneCommand.Parameters.AddWithValue("@CategoryName", categoryName);
					using (NpgsqlDataReader subcategoryOneReader = subcategoryOneCommand.ExecuteReader())
					{
						while (subcategoryOneReader.Read())
						{
							string subcategoryOneName = subcategoryOneReader["subcategory_one_name"].ToString();

							SubcategoryOne subcategoryOne = new SubcategoryOne
							{
								SubcategoryOneName = subcategoryOneName,
								SubcategoriesSecond = GetSubcategoriesSecond(connectionString, subcategoryOneName, categoryName) // в аргумент добавить categoryName 
							};

							subcategoriesOne.Add(subcategoryOne);
						}
					}
				}
			}

			return subcategoriesOne;
		}


		private List<SubcategorySecond> GetSubcategoriesSecond(string connectionString, string subcategoryOneName, string categoryName)
		{
			List<SubcategorySecond> subcategoriesSecond = new List<SubcategorySecond>();

			using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
			{
				connection.Open();
				 
				string subcategorySecondNameColumn = "subcategory_scnd_name";
				string regionValue = RegionValue ?? string.Empty;
				string subcategorySecondQuery = string.Empty;

				if (regionValue.StartsWith("ХПТР"))
				{
					subcategorySecondQuery = $"SELECT subcategory_scnd_name" +
						" FROM subcategory_scnd_hptr WHERE subcategory_one_name = @SubcategoryOneName AND category_name = @CategoryName AND subcategory_scnd_name IS NOT NULL";
				}
				else if (regionValue.StartsWith("ХПТ "))
				{
					subcategorySecondQuery = $"SELECT subcategory_scnd_name " +
						"FROM subcategory_scnd_hpt WHERE subcategory_one_name = @SubcategoryOneName AND category_name = @CategoryName AND subcategory_scnd_name IS NOT NULL";
				}
				else if (regionValue.StartsWith("HPT_1045") && !regionValue.EndsWith("4514"))
				{
					subcategorySecondQuery = $"SELECT subcategory_scnd_name " +
						"FROM subcategory_scnd_hpt_1045 WHERE subcategory_one_name = @SubcategoryOneName AND category_name = @CategoryName AND subcategory_scnd_name IS NOT NULL";
				}
				else
				{
					subcategorySecondQuery = $"SELECT subcategory_scnd_name " +
						"FROM subcategory_scnd_hpt_104514 WHERE subcategory_one_name = @SubcategoryOneName AND category_name = @CategoryName AND subcategory_scnd_name IS NOT NULL";
				}

				using (NpgsqlCommand subcategorySecondCommand = new NpgsqlCommand(subcategorySecondQuery, connection))
				{
					subcategorySecondCommand.Parameters.AddWithValue("@SubcategoryOneName", subcategoryOneName);
					subcategorySecondCommand.Parameters.AddWithValue("@CategoryName", categoryName);

					using (NpgsqlDataReader subcategorySecondReader = subcategorySecondCommand.ExecuteReader())
					{
						while (subcategorySecondReader.Read())
						{
							string subcategorySecondName = subcategorySecondReader[subcategorySecondNameColumn].ToString();

							SubcategorySecond subcategorySecond = new SubcategorySecond
							{
								SubcategorySecondName = subcategorySecondName,
								SubcategoriesThird = GetSubcategoryThirds(connectionString, subcategorySecondName, subcategoryOneName, categoryName)
							};
							subcategoriesSecond.Add(subcategorySecond);
						}
					}
				}
			}

			return subcategoriesSecond;
		}


		private List<SubcategoryThird> GetSubcategoryThirds(string connectionString, string subcategoryScndName, string subcategoryOneName, string categoryName)
		{
			List<SubcategoryThird> subcategoryThirds = new List<SubcategoryThird>();

			using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
			{
				connection.Open();

				string subcategoryThirdNameColumn = "subcategory_third_name";
				string regionValue = RegionValue ?? string.Empty;
				string subcategoryThirdQuery = string.Empty;

				if (regionValue.StartsWith("ХПТР"))
				{
					subcategoryThirdQuery = "SELECT subcategory_third_name" +
						" FROM subcategory_third_hptr WHERE subcategory_scnd_name = @SubcategoryScndName AND subcategory_one_name = @SubcategoryOneName" +
						" AND category_name = @CategoryName AND subcategory_third_name IS NOT NULL";
				}
				else if (regionValue.StartsWith("ХПТ "))
				{
					subcategoryThirdQuery = "SELECT subcategory_third_name " +
						"FROM subcategory_third_hpt WHERE subcategory_scnd_name = @SubcategoryScndName AND subcategory_one_name = @SubcategoryOneName" +
						" AND category_name = @CategoryName AND subcategory_third_name IS NOT NULL";
				}
				else if (regionValue.StartsWith("HPT_1045") && !regionValue.EndsWith("4514"))
				{
					subcategoryThirdQuery = "SELECT subcategory_third_name " +
						"FROM subcategory_third_hpt_1045 WHERE subcategory_scnd_name = @SubcategoryScndName AND subcategory_one_name = @SubcategoryOneName" +
						" AND category_name = @CategoryName AND subcategory_third_name IS NOT NULL";
				}
				else
				{
					subcategoryThirdQuery = "SELECT subcategory_third_name " +
						"FROM subcategory_third_hpt_104514 WHERE subcategory_scnd_name = @SubcategoryScndName AND subcategory_one_name = @SubcategoryOneName" +
						" AND category_name = @CategoryName AND subcategory_third_name IS NOT NULL";
				}

				using (NpgsqlCommand subcategoryThirdCommand = new NpgsqlCommand(subcategoryThirdQuery, connection))
				{
					subcategoryThirdCommand.Parameters.AddWithValue("@SubcategoryScndName", subcategoryScndName);
					subcategoryThirdCommand.Parameters.AddWithValue("@SubcategoryOneName", subcategoryOneName);
					subcategoryThirdCommand.Parameters.AddWithValue("@CategoryName", categoryName);

					using (NpgsqlDataReader subcategoryThirdReader = subcategoryThirdCommand.ExecuteReader())
					{
						while (subcategoryThirdReader.Read())
						{
							string subcategoryThirdName = subcategoryThirdReader[subcategoryThirdNameColumn].ToString();

							SubcategoryThird subcategorySecond = new SubcategoryThird
							{
								SubcategoryThirdName = subcategoryThirdName
							};
							subcategoryThirds.Add(subcategorySecond);
						}
					}
				}
			}

			return subcategoryThirds;
		}

		private void TreeViewCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			// Получаем выбранный элемент TreeView
			var selectedItem = TreeViewCategories.SelectedItem as SubcategoryThird;

			if (selectedItem != null)
			{
				categoryFourthTextB.Text = selectedItem.SubcategoryThirdName;

				var parentSubcategoryScnd = FindParentSubcategorySecond(selectedItem);
				if (parentSubcategoryScnd != null)
				{
					categoryThirdTextB.Text = parentSubcategoryScnd.SubcategorySecondName;

					var parentSubcategoryOne = FindParentSubcategoryOne(parentSubcategoryScnd);
					if (parentSubcategoryOne != null)
					{
						categoryTwoTextB.Text = parentSubcategoryOne.SubcategoryOneName;

						var parentCategory = FindParentCategory(parentSubcategoryOne);
						if (parentCategory != null)
						{
							categoryOneTextB.Text = parentCategory.CategoryName;
						}
					}
				}
			}
		}

		private Category FindParentCategory(SubcategoryOne subcategoryOne)
		{
			// Находим родительскую категорию, перебирая коллекцию элементов TreeView
			var categories = TreeViewCategories.ItemsSource as IEnumerable<Category>;
			return categories?.FirstOrDefault(category => category.SubcategoriesOne.Contains(subcategoryOne));
		}

		private SubcategoryOne FindParentSubcategoryOne(SubcategorySecond subcategorySecond)
		{
			// Находим родительскую SubcategoryOne, перебирая коллекцию элементов TreeView
			var categories = TreeViewCategories.ItemsSource as IEnumerable<Category>;
			foreach (var category in categories)
			{
				foreach (var subcategoryOne in category.SubcategoriesOne)
				{
					if (subcategoryOne.SubcategoriesSecond.Contains(subcategorySecond))
					{
						return subcategoryOne;
					}
				}
			}
			return null;
		}

		private SubcategorySecond FindParentSubcategorySecond(SubcategoryThird subcategoryThird)
		{
			var categories = TreeViewCategories.ItemsSource as IEnumerable<Category>;
			foreach (var category in categories)
			{
				foreach (var subcategoryOne in category.SubcategoriesOne)
				{
					foreach(var subcategoryScnd in subcategoryOne.SubcategoriesSecond)
					{
						if (subcategoryScnd.SubcategoriesThird.Contains(subcategoryThird))
						{
							return subcategoryScnd;
						}
					}
				}
			}
			return null;
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			// Проверяем, что родительское окно является экземпляром класса MainWindow
			if (ParentWindow is MainWindow mainWindow)
			{
				// Получаем значения из текстовых полей
				string categoryOneValue = categoryOneTextB.Text;
				string categoryTwoValue = categoryTwoTextB.Text;
				string categoryThirdValue = categoryThirdTextB.Text;
				string categoryFourthValue = categoryFourthTextB.Text;
				string reasonValue = reasonTextB.Text;
				// Закрываем текущее окно

				CategoryHistory(categoryOneValue, categoryTwoValue, categoryThirdValue, categoryFourthValue);
				Close();

				// Вызываем метод в родительском окне для обновления значений ячеек выбранной строки
				try
				{
					using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
					{
						connection.Open();
						using (NpgsqlCommand updateCommand = new NpgsqlCommand(DBContext.setChangeDateQuery, connection))
						{
							updateCommand.Parameters.AddWithValue("@id", _id);
							updateCommand.Parameters.AddWithValue("@change_at", DateTime.Now);

							updateCommand.ExecuteNonQuery();
						}
					}
					mainWindow.UpdateSelectedRowValues(categoryFourthValue, categoryOneValue, categoryTwoValue, categoryThirdValue, reasonValue);
					SortingTable sortingTable = new SortingTable(mainWindow.startDatePicker,
						mainWindow.endDatePicker, mainWindow.RegionsLB,
						mainWindow.selectRowComboBox, mainWindow.DataGridTable);
					sortingTable.GetSortTable();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}

			}
		}
	}
}







