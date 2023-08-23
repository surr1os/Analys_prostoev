using Npgsql;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;


using static Analys_prostoev.CategoryHierarchy;

namespace Analys_prostoev
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class CategoryHierarchy : Window
    {

        private string connectionString = "Host=localhost;Port=5432;Database=myDb;Username=postgres;Password=iqdeadzoom1r";
        public CategoryHierarchy(string cellValue)
        {
            InitializeComponent();
            //   categoryText.Text = cellValue;
            List<Category> categories = GetCategories(connectionString);
            

            // Установка источника данных для TreeView
            TreeViewCategories.ItemsSource = categories;

        }
        //Создаем модель представления

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
        private List<SubcategorySecond> GetSubcategoriesSecond(string connectionString, string subcategoryOneName,string categoryName) // ,string categoryName
        {
            List<SubcategorySecond> subcategoriesSecond = new List<SubcategorySecond>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Запрос для получения данных из таблицы Subcategory_scnd по заданной подкатегории
                string subcategorySecondQuery = "SELECT subcategory_scnd_name FROM Subcategory_scnd WHERE subcategory_one_name = @SubcategoryOneName AND category_name = @CategoryName"; // здесь сделай проверку на равеноство                                                                                                                                               
                                                                                                                                                       // categoryName и category_name из subcategory_scnd 
                                                                                                                                                       // нужно добавить поле category_name в таблицу subcategory_scnd 

                using (NpgsqlCommand subcategorySecondCommand = new NpgsqlCommand(subcategorySecondQuery, connection))
                {
                    subcategorySecondCommand.Parameters.AddWithValue("@SubcategoryOneName", subcategoryOneName);
                    subcategorySecondCommand.Parameters.AddWithValue("@CategoryName", categoryName);

                    using (NpgsqlDataReader subcategorySecondReader = subcategorySecondCommand.ExecuteReader())
                    {
                        while (subcategorySecondReader.Read())
                        {
                            string subcategorySecondName = subcategorySecondReader["subcategory_scnd_name"].ToString();

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
    }



}

                
                
            
        
    

