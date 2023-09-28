using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static Analys_prostoev.CategoryHierarchy;

namespace Analys_prostoev
{
    public partial class CategoryHierarchy : Window
    {

        public MainWindow ParentWindow { get; set; }

        private string connectionString = "Host=10.241.224.71;Port=5432;Database=analysisProdBD;Username=analysis_user;Password=71NfhRec";
        public CategoryHierarchy(string regionValue)
        {
            RegionValue = regionValue;
            InitializeComponent();
            //   categoryText.Text = cellValue;
            List<Category> categories = GetCategories(connectionString);


            // Установка источника данных для TreeView
            TreeViewCategories.ItemsSource = categories;

        }
        //Создаем модель представления
        public string RegionValue { get; set; }
        public class Category
        {
            public string CategoryName { get; set; }
            public List<SubcategoryOne> SubcategoriesOne { get; set; }
        }

        public class SubcategoryOne
        {
            public string SubcategoryOneName { get; set; }
            public List<SubcategorySecond> SubcategoriesSecond { get; set; }
        }

        public class SubcategorySecond
        {
            public string SubcategorySecondName { get; set; }
        }
        private List<Category> GetCategories(string connectionString)
        {
            List<Category> categories = new List<Category>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Запрос для получения данных из таблицы Category
                string categoryQuery = "SELECT category_name FROM Category";

                using (NpgsqlCommand categoryCommand = new NpgsqlCommand(categoryQuery, connection))
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

                string subcategoryOneQuery = "SELECT subcategory_one_name FROM Subcategory_one WHERE category_name = @CategoryName";

                using (NpgsqlCommand subcategoryOneCommand = new NpgsqlCommand(subcategoryOneQuery, connection))
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
                string regionValue = RegionValue ?? string.Empty; // Добавьте проверку на null и присвойте пустую строку, если переменная regionValue равна null
                if (regionValue.StartsWith("ХПТР"))
                {
                    subcategorySecondNameColumn = "subcategory_scnd_namehptr";
                }
                // Запрос для получения данных из таблицы Subcategory_scnd по заданной подкатегории
                string subcategorySecondQuery = $"SELECT {subcategorySecondNameColumn} FROM Subcategory_scnd WHERE subcategory_one_name = @SubcategoryOneName AND category_name = @CategoryName AND {subcategorySecondNameColumn} IS NOT NULL";                                                                                                                           // здесь сделай проверку на равеноство                                                                                                                                               
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
                                SubcategorySecondName = subcategorySecondName
                            };
                            subcategoriesSecond.Add(subcategorySecond);
                        }
                    }
                }
            }

            return subcategoriesSecond;
        }


        private void TreeViewCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Получаем выбранный элемент TreeView
            var selectedItem = TreeViewCategories.SelectedItem as SubcategorySecond;

            if (selectedItem != null)
            {
                // Помещаем значение SubcategorySecondName в categoryOneTextB
                categoryThirdTextB.Text = selectedItem.SubcategorySecondName;


                // Находим родителя SubcategorySecondName и помещаем его значение в categoryTwoTextB
                var parentSubcategoryOne = FindParentSubcategoryOne(selectedItem);
                if (parentSubcategoryOne != null)
                {
                    categoryTwoTextB.Text = parentSubcategoryOne.SubcategoryOneName;

                    // Находим родителя SubcategoryOneName и помещаем его значение в categoryThirdTextB
                    var parentCategory = FindParentCategory(parentSubcategoryOne);
                    if (parentCategory != null)
                    {
                        categoryOneTextB.Text = parentCategory.CategoryName;

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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что родительское окно является экземпляром класса MainWindow
            if (ParentWindow is MainWindow mainWindow)
            {
                // Получаем значения из текстовых полей
                string categoryOneValue = categoryOneTextB.Text;
                string categoryTwoValue = categoryTwoTextB.Text;
                string categoryThirdValue = categoryThirdTextB.Text;
                string reasonValue = reasonTextB.Text;
                // Закрываем текущее окно
                this.Close();

                // Вызываем метод в родительском окне для обновления значений ячеек выбранной строки
                try
                {
                    mainWindow.UpdateSelectedRowValues(categoryOneValue, categoryTwoValue, categoryThirdValue, reasonValue);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
               
            }
        }
    }



}







