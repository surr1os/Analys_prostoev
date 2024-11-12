using Analys_prostoev.Data;
using Analys_prostoev.Handlers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AnalysisDowntimes
{
	/// <summary>
	/// Логика взаимодействия для CreateCategory.xaml
	/// </summary>
	public partial class CreateCategory : Window
	{
		#region Data

		#region Boolean
		private bool not_changed { get; set; }

		private bool existsThirdCategory { get; set; }

		#endregion

		#region String
		private string selectCategoryTwo { get; set; }
		private string selectCategoryThird { get; set; }
		private string selectCategoryFourth { get; set; }

		private string currentRegionsType { get; set; } = "ХПТ";

		private string nameThirdCategoriesTable { get; set; }
		private string nameFourthCategoriesTable { get; set; }

		private string createNewCategoryThird { get; set; }
		private string createNewCategoryFourth { get; set; }

		private string selectedCategoryThird { get; set; }
		private string selectedCategoryFourth { get; set; }
		#endregion

		#endregion

		public CreateCategory()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			CategotyFourthTB.IsEnabled = false;

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				AddRegionsType();
				GetCategoriesOne(connection);
			}
		}

		#region Tree Logic

		private void AddRegionsType()
		{
			regionsType.Items.Add("ХПТ");
			regionsType.Items.Add("ХПТР");
			regionsType.Items.Add("ХПТ 10-45");
			regionsType.Items.Add("ХПТ 10-45 №14");
		}

		/// <summary>
		/// Добавление категорий первого уровня.
		/// </summary>
		/// <param name="connection">Соединение с бд.</param>
		private void GetCategoriesOne(NpgsqlConnection connection)
		{
			AddCategoryOne(connection, DBContext.selectCategoryOne, categoryOneCB);
			AddCategoryOne(connection, DBContext.selectCategoryOne, categoryOneLB);
		}

		/// <summary>
		/// Подбор категории второго уровня в зависимости от выбранной категории 1-го уровня для ComboBox.
		/// </summary>
		private void categoryOneCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			categoryTwoCB.Items.Clear();
			categoryTwoCB.SelectedItem = null;

			selectCategoryTwo = DBContext.selectCategoryTwo;

			GetCategoryScnd();

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();
				AddCategoryOne(connection, selectCategoryTwo, categoryTwoCB);
			}

			categoryOneLB.SelectedItem = categoryOneCB.SelectedItem;
		}

		/// <summary>
		/// Подбор категории второго уровня в зависимости от выбранной категории первого уровня для ListBox.
		/// </summary>
		private void categoryOneLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			categoryTwoLB.Items.Clear();
			categoryTwoLB.SelectedItem = null;

			selectCategoryTwo = DBContext.selectCategoryTwo;
			GetCategoryScnd();

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();
				AddCategoryOne(connection, selectCategoryTwo, categoryTwoLB);
			}

			categoryOneCB.SelectedItem = categoryOneLB.SelectedItem;
		}

		/// <summary>
		/// Подбор категории третьего уровня в зависимости от выбранной категории второго уровня для ListBox.
		/// </summary>
		private void categoryTwoLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			categoryThirdLB.Items.Clear();
			categoryThirdLB.SelectedItem = null;

			selectCategoryThird = DBContext.selectCategoryThird;

			selectCategoryThird += $" {GetNameCategoriesThirdsTable(currentRegionsType)} " +
				$"Where subcategory_one_name = '{(string)categoryTwoLB.SelectedItem}' " +
				$"and category_name = '{(string)categoryOneLB.SelectedItem}' " +
				$"and is_removed = false";

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();
				AddCategoryOne(connection, selectCategoryThird, categoryThirdLB);
			}

			categoryTwoCB.SelectedItem = categoryTwoLB.SelectedItem;
		}

		private void categoryTwoCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			categoryTwoLB.SelectedItem = categoryTwoCB.SelectedItem;
		}

		/// <summary>
		/// Подбор категории четвёртого уровня в зависимости от выбранной категории третьего уровня для ListBox.
		/// </summary>
		private void categoryThirdLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CategotyFourthTB.IsEnabled = true;

			categoryFourthLB.Items.Clear();
			categoryFourthLB.SelectedItem = null;

			selectCategoryFourth = DBContext.selectCategoryFourth;

			selectCategoryFourth += $" {GetNameCategoriesFourthTable(currentRegionsType)} " +
				$"Where subcategory_scnd_name = '{(string)categoryThirdLB.SelectedItem}' " +
				$"and subcategory_one_name = '{(string)categoryTwoLB.SelectedItem}' " +
				$"and category_name = '{(string)categoryOneLB.SelectedItem}' " +
				$"and is_removed = false";

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();
				AddCategoryOne(connection, selectCategoryFourth, categoryFourthLB);
			}

			CategoryThirdTB.Text = (string)categoryThirdLB.SelectedItem;
		}

		private void AddCategoryOne(NpgsqlConnection connection, string command, ComboBox currentComboBox)
		{
			List<string> categories = new List<string>();

			using (NpgsqlCommand selectCommand = new NpgsqlCommand(command, connection))
			{
				using (NpgsqlDataReader reader = selectCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						string category = reader.GetString(0);
						categories.Add(category);
					}
				}
			}

			foreach (string category in categories)
			{
				currentComboBox.Items.Add(category);
			}
		}

		private void AddCategoryOne(NpgsqlConnection connection, string command, ListBox currentListBox)
		{
			List<string> categories = new List<string>();

			using (NpgsqlCommand selectCommand = new NpgsqlCommand(command, connection))
			{
				using (NpgsqlDataReader reader = selectCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						string category = reader.GetString(0);
						categories.Add(category);
					}
				}
			}

			foreach (string category in categories)
			{
				currentListBox.Items.Add(category);
			}
		}

		private void regionsType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			categoryThirdLB.Items.Clear();
			categoryFourthLB.Items.Clear();
			CategoryThirdTB.Clear();
			CategotyFourthTB.Clear();

			currentRegionsType = (string)regionsType.SelectedItem;
		}

		private string GetNameCategoriesFourthTable(string currentRegionsType)
		{
			switch (currentRegionsType)
			{
				case "ХПТ":
					nameFourthCategoriesTable = "subcategory_third_hpt";
					break;

				case "ХПТР":
					nameFourthCategoriesTable = "subcategory_third_hptr";
					break;

				case "ХПТ 10-45":
					nameFourthCategoriesTable = "subcategory_third_hpt_1045";
					break;

				case "ХПТ 10-45 №14":
					nameFourthCategoriesTable = "subcategory_third_hpt_104514";
					break;
			}

			return nameFourthCategoriesTable;
		}

		private string GetNameCategoriesThirdsTable(string currentRegionsType)
		{
			switch (currentRegionsType)
			{
				case "ХПТ":
					nameThirdCategoriesTable = "subcategory_scnd_hpt";
					break;

				case "ХПТР":
					nameThirdCategoriesTable = "subcategory_scnd_hptr";
					break;

				case "ХПТ 10-45":
					nameThirdCategoriesTable = "subcategory_scnd_hpt_1045";
					break;

				case "ХПТ 10-45 №14":
					nameThirdCategoriesTable = "subcategory_scnd_hpt_104514";
					break;
			}

			return nameThirdCategoriesTable;
		}

		private void GetCategoryScnd()
		{
			switch ((string)categoryOneLB.SelectedItem)
			{
				case "Плановые простои":
					selectCategoryTwo += " where category_name = 'Плановые простои'";

					break;

				case "Внеплановые простои":
					selectCategoryTwo += " where category_name = 'Внеплановые простои'";
					break;

				case "Внешние для цеха":
					selectCategoryTwo += " where category_name = 'Внешние для цеха'";
					break;
			}
		}

		#endregion

		#region Create

		private void CreateThird_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(CategoryThirdTB.Text) ||
				categoryTwoLB.SelectedItem == null || categoryOneLB.SelectedItem == null ||
				categoryTwoCB.SelectedItem == null || categoryOneCB.SelectedItem == null)
			{
				MessageBox.Show("Вы не заполнили одну и категорий!", "info");
				return;
			}

			createNewCategoryThird = DBContext.createCategoryThird;

			createNewCategoryThird += $"{GetNameCategoriesThirdsTable(currentRegionsType)}" +
				$"(category_name, subcategory_one_name, subcategory_scnd_name, not_changeable) " +
				$"values('{(string)categoryOneLB.SelectedItem}', '{(string)categoryTwoLB.SelectedItem}', '{CategoryThirdTB.Text}', false)";

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				using (NpgsqlCommand createCommand = new NpgsqlCommand(createNewCategoryThird, connection))
				{
					createCommand.ExecuteNonQuery();
				}
			}

			MessageBox.Show("Успешно добавлена категория 3-го уровня!", "info");

			//history CategoryThirdTB.Text
			AddHistory(CategoryChangeTypes.Created, $"Создана категория \"{CategoryThirdTB.Text}\"");

			CategoryThirdTB.Clear();

			categoryThirdLB.Items.Clear();
			categoryTwoLB.SelectedItem = null;
		}

		private void CreateFourth_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(CategotyFourthTB.Text) ||
				string.IsNullOrEmpty(CategoryThirdTB.Text) || CategoryThirdTB.Text == "" ||
				categoryTwoLB.SelectedItem == null || categoryOneLB.SelectedItem == null ||
				categoryTwoCB.SelectedItem == null || categoryOneCB.SelectedItem == null)
			{
				MessageBox.Show("Вы не заполнили одну и категорий!", "info");
				return;
			}

			selectedCategoryThird = (string)categoryThirdLB.SelectedItem;

			CategoryThirdTB.Text = selectedCategoryThird;

			createNewCategoryFourth = DBContext.createCategoryFourth;

			createNewCategoryFourth += $"{GetNameCategoriesFourthTable(currentRegionsType)}" +
				$"(category_name, subcategory_one_name, subcategory_scnd_name, subcategory_third_name, not_changeable) " +
				$"values('{(string)categoryOneLB.SelectedItem}', '{(string)categoryTwoLB.SelectedItem}', '{CategoryThirdTB.Text}', '{CategotyFourthTB.Text}', false)";

			
			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				using (NpgsqlCommand createCommand = new NpgsqlCommand(createNewCategoryFourth, connection))
				{
					createCommand.ExecuteNonQuery();
				}
			}

			MessageBox.Show("Успешно добавлена категория 4-го уровня!", "info");

			//history CategotyFourthTB.Text
			AddHistory(CategoryChangeTypes.Created, $"Создана категория \"{CategotyFourthTB.Text}\"");

			CategoryThirdTB.Clear();
			CategotyFourthTB.Clear();

			categoryFourthLB.Items.Clear();
			categoryThirdLB.SelectedItem = null;
			CategotyFourthTB.IsEnabled = false;
		}

		private void CreateCategoryBtn_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(CategoryThirdTB.Text) && string.IsNullOrEmpty(CategotyFourthTB.Text))
			{
				MessageBox.Show("Введите названия для необходимых категорий!", "info");
				return;
			}

			if (!string.IsNullOrEmpty(CategoryThirdTB.Text) && string.IsNullOrEmpty(CategotyFourthTB.Text))
			{
				CreateThird_Click(sender, e);
			}
			else if (!string.IsNullOrEmpty(CategoryThirdTB.Text) && !string.IsNullOrEmpty(CategotyFourthTB.Text))
			{
				CreateFourth_Click(sender, e);
			}

		}
		#endregion

		#region Change

		private void ChangeThird_Click(object sender, RoutedEventArgs e)
		{
			//для обновления нужно выбрать элемент и его родителей.
			if (categoryThirdLB.SelectedItem == null)
			{
				MessageBox.Show("Вы не выбрали категорию для изменения", "Info");
			}

			selectedCategoryFourth = null;

			CreateCategoryBtn.Visibility = Visibility.Collapsed;
			ChangeCategoryBtn.Visibility = Visibility.Visible;
			Cansel.Visibility = Visibility.Visible;

			categoryOneCB.IsEnabled = false;
			categoryTwoCB.IsEnabled = false;

			selectedCategoryThird = (string)categoryThirdLB.SelectedItem;
			CategoryThirdTB.Text = selectedCategoryThird;
		}

		private void ChangeFourth_Click(object sender, RoutedEventArgs e)
		{
			//для обновления нужно выбрать элемент и его родителей.
			if (categoryFourthLB.SelectedItem == null)
			{
				MessageBox.Show("Вы не выбрали категорию для изменения", "Info");
			}

			CreateCategoryBtn.Visibility = Visibility.Collapsed;
			ChangeCategoryBtn.Visibility = Visibility.Visible;
			Cansel.Visibility = Visibility.Visible;

			categoryOneCB.IsEnabled = false;
			categoryTwoCB.IsEnabled = false;

			selectedCategoryThird = (string)categoryThirdLB.SelectedItem;
			selectedCategoryFourth = (string)categoryFourthLB.SelectedItem;
			CategoryThirdTB.Text = selectedCategoryThird;
			CategotyFourthTB.Text = selectedCategoryFourth;
		}

		private void ChangeCategoryBtn_Click(object sender, RoutedEventArgs e)
		{
			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				if (!string.IsNullOrEmpty(selectedCategoryThird) && string.IsNullOrEmpty(selectedCategoryFourth))
				{
					CheckNotChangedThird(connection);

					if (not_changed)
					{
						MessageBox.Show("Категории из первоначального списка изменять нельзя!", "info");
						return;
					}

					using (NpgsqlCommand updateCommand = new NpgsqlCommand(DBContext.UpdateCategoryThirdCommand(
						GetNameCategoriesThirdsTable(currentRegionsType),
						(string)categoryOneCB.SelectedItem,
						(string)categoryTwoCB.SelectedItem,
						selectedCategoryThird,
						CategoryThirdTB.Text),
						connection))
					{
						updateCommand.ExecuteNonQuery();
					}

					CheckThirdCategory(connection);

					if (existsThirdCategory)
					{
						using (NpgsqlCommand updateCommand = new NpgsqlCommand(DBContext.UpdateCategoryThirdInCategoryFourthTableCommand(
						GetNameCategoriesFourthTable(currentRegionsType),
						(string)categoryOneCB.SelectedItem,
						(string)categoryTwoCB.SelectedItem,
						selectedCategoryThird,
						CategoryThirdTB.Text),
						connection))
						{
							updateCommand.ExecuteNonQuery();
						}
					}

					MessageBox.Show("Категория успешно обновлена!", "info");

					//history с selectedCategoryThird на CategoryThirdTB.Text
					AddHistory(CategoryChangeTypes.Changed, $"Категория \"{selectedCategoryThird}\" изменена на \"{CategoryThirdTB.Text}\"");

					categoryThirdLB.Items.Clear();
					categoryTwoLB.SelectedItem = null;

					CategoryThirdTB.Clear();

					Cansel_Click(sender, e);

				}
				else if (!string.IsNullOrEmpty(selectedCategoryThird) && !string.IsNullOrEmpty(selectedCategoryFourth))
				{
					CheckNotChangedFourth(connection);

					if (not_changed)
					{
						MessageBox.Show("Категории из первоначального списка изменять нельзя!", "info");
						return;
					}

					using (NpgsqlCommand updateCommand = new NpgsqlCommand(DBContext.UpdateCategoryFourth(
						GetNameCategoriesFourthTable(currentRegionsType),
						(string)categoryOneCB.SelectedItem,
						(string)categoryTwoCB.SelectedItem,
						selectedCategoryThird,
						selectedCategoryFourth,
						CategotyFourthTB.Text),
						connection))
					{
						updateCommand.ExecuteNonQuery();
					}

					MessageBox.Show("Категория успешно обновлена!", "info");

					//history с selectedCategoryFourth на CategotyFourthTB.Text
					AddHistory(CategoryChangeTypes.Changed, $"Категория \"{selectedCategoryFourth}\" изменена на \"{CategotyFourthTB.Text}\"");

					categoryFourthLB.Items.Clear();
					categoryThirdLB.SelectedItem = null;

					CategotyFourthTB.Clear();

					Cansel_Click(sender, e);
				}
			}
		}

		#endregion

		#region Remove

		private void RemoveThird_Click(object sender, RoutedEventArgs e)
		{
			selectedCategoryThird = (string)categoryThirdLB.SelectedItem;

			if (selectedCategoryThird == null)
			{
				MessageBox.Show("Вы не выбрали категорию для удаления", "Info");
			}

			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				CheckNotChangedThird(connection);

				if (not_changed)
				{
					MessageBox.Show("Категории из первоначального списка удалить нельзя!", "info");
					return;
				}

				using (NpgsqlCommand removeCommand = new NpgsqlCommand(DBContext.RemoveCategoryThird(
					GetNameCategoriesThirdsTable(currentRegionsType),
					(string)categoryOneCB.SelectedItem,
					(string)categoryTwoCB.SelectedItem,
					selectedCategoryThird),
					connection))
				{
					removeCommand.ExecuteNonQuery();
				}

				CheckThirdCategory(connection);

				if (existsThirdCategory)
				{
					RemoveThirdCategoryInFourthTable(connection,
													selectedCategoryThird,
													(string)categoryTwoCB.SelectedItem,
													(string)categoryOneCB.SelectedItem);
				}
			
				
				MessageBox.Show($"Категория \'{selectedCategoryThird}\' удалена", "Info");

				//history selectedCategoryThird
				AddHistory(CategoryChangeTypes.Removed, $"Категория 3-го уровня \"{selectedCategoryThird}\" удалена");

				categoryThirdLB.Items.Clear();
				categoryTwoLB.SelectedItem = null;
			}
		}

		private void RemoveThirdCategoryInFourthTable(NpgsqlConnection connection, string selectedCategoryThird, string selectedCategoryTwo, string selectedCategoryOne)
		{
			using (NpgsqlCommand removeCommand = new NpgsqlCommand(DBContext.RemoveCategoryThirdInFourthTable(
				GetNameCategoriesFourthTable(currentRegionsType),
				selectedCategoryOne,
				selectedCategoryTwo,
				selectedCategoryThird),
				connection))
			{
				removeCommand.ExecuteNonQuery();
			}
		}

		private void RemoveFourth_Click(object sender, RoutedEventArgs e)
		{

			selectedCategoryThird = (string)categoryThirdLB.SelectedItem;

			selectedCategoryFourth = (string)categoryFourthLB.SelectedItem;


			if (string.IsNullOrEmpty(selectedCategoryThird) && string.IsNullOrEmpty(selectedCategoryFourth))
			{
				MessageBox.Show("Вы не выбрали категорию для удаления", "Info");
				return;
			}


			using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
			{
				connection.Open();

				CheckNotChangedFourth(connection);

				if (not_changed)
				{
					MessageBox.Show("Категории из первоначального списка удалять нельзя!", "info");
				}

				using (NpgsqlCommand removeCommand = new NpgsqlCommand(DBContext.RemoveCategoryFourth(
					GetNameCategoriesFourthTable(currentRegionsType),
					(string)categoryOneCB.SelectedItem,
					(string)categoryTwoCB.SelectedItem,
					selectedCategoryThird,
					selectedCategoryFourth),
					connection))
				{
					removeCommand.ExecuteNonQuery();
				}

				MessageBox.Show($"Категория \'{selectedCategoryFourth}\' удалена", "Info");

				//history selectedCategoryFourth
				AddHistory(CategoryChangeTypes.Removed, $"Категория 4-го уровня \"{selectedCategoryFourth}\" удалена");

				categoryFourthLB.Items.Clear();
				categoryThirdLB.SelectedItem = null;
			}
		}

		#endregion

		#region Helpers

		private void CheckThirdCategory(NpgsqlConnection connection)
		{
			existsThirdCategory = false;

			using (NpgsqlCommand checkRemoved = new NpgsqlCommand(DBContext.SearchThirdCategory(
				GetNameCategoriesFourthTable(currentRegionsType),
				(string)categoryOneCB.SelectedItem,
				(string)categoryTwoCB.SelectedItem,
				selectedCategoryThird),
				connection))
			{
				using (NpgsqlDataReader reader = checkRemoved.ExecuteReader())
				{
					while (reader.Read())
					{
						if (reader != null)
						{
							existsThirdCategory = true;
						}
					}
				}
			}
		}

		private void CheckNotChangedThird(NpgsqlConnection connection)
		{
			using (NpgsqlCommand notChange = new NpgsqlCommand(DBContext.NotChangeThirdCommand(
				GetNameCategoriesThirdsTable(currentRegionsType),
				(string)categoryOneCB.SelectedItem,
				(string)categoryTwoCB.SelectedItem,
				selectedCategoryThird),
				connection))
			{
				using (NpgsqlDataReader reader = notChange.ExecuteReader())
				{
					while (reader.Read())
					{
						not_changed = reader.GetBoolean(0);
					}
				}
			}
		}

		private void CheckNotChangedFourth(NpgsqlConnection connection)
		{
			using (NpgsqlCommand notChange = new NpgsqlCommand(DBContext.NotChangeFourthCommand(
				GetNameCategoriesFourthTable(currentRegionsType),
				(string)categoryOneCB.SelectedItem,
				(string)categoryTwoCB.SelectedItem,
				selectedCategoryThird,
				selectedCategoryFourth),
				connection))
			{
				using (NpgsqlDataReader reader = notChange.ExecuteReader())
				{
					while (reader.Read())
					{
						not_changed = reader.GetBoolean(0);
					}
				}
			}
		}

		private void Cansel_Click(object sender, RoutedEventArgs e)
		{
			CreateCategoryBtn.Visibility = Visibility.Visible;
			ChangeCategoryBtn.Visibility = Visibility.Collapsed;
			Cansel.Visibility = Visibility.Collapsed;
			CategoryThirdTB.Clear();
			CategotyFourthTB.Clear();

			categoryOneCB.IsEnabled = true;
			categoryTwoCB.IsEnabled = true;
		}

		private void AddHistory(string type, string title)
		{
			CategorysHistory history = new CategorysHistory 
			{
				Id = Guid.NewGuid(),
				Type = type,
				Title = title,
				CreatedDate = DateTime.Now,
			};

			var command = DBContext.AddHistoryOfCategory(history);
			AddCategorysHistoryHandler historyHandler = new AddCategorysHistoryHandler(command);
			historyHandler.AddHistory();
		}

		#endregion

		#region Copy

		private void Copy_Click(object sender, RoutedEventArgs e)
		{
			if (categoryFourthLB.SelectedItem != null)
			{
				string selectedText = (string)categoryFourthLB.SelectedItem;
				Clipboard.SetText(selectedText);
			}
		}

		private void CopyParent_Click(object sender, RoutedEventArgs e)
		{
			if (categoryThirdLB.SelectedItem != null)
			{
				string selectedText = (string)categoryThirdLB.SelectedItem;
				Clipboard.SetText(selectedText);
			}
		}

		#endregion
	}
}
