using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Analys_prostoev
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class CategoryHierarchy : Window
    {
        private string connectionString = "Host=localhost;Database=myDb;Username=postgres;Password=iqdeadzoom1r";
        public CategoryHierarchy()
        {
            InitializeComponent();


            // Создаем модель представления
           

        }

        // Реализация иерархического списка категорий и возвращение значения в ячейку

        public class Category
        {
            public string CategoryName { get; set; }
            public ObservableCollection<SubcategoryOne> SubcategoriesOne { get; set; }

            public Category()
            {
                SubcategoriesOne = new ObservableCollection<SubcategoryOne>();
            }
        }

        public class SubcategoryOne
        {
            public string SubcategoryOneName { get; set; }
            public ObservableCollection<SubcategoryScnd> SubcategoriesScnd { get; set; }

            public SubcategoryOne()
            {
                SubcategoriesScnd = new ObservableCollection<SubcategoryScnd>();
            }
        }

        public class SubcategoryScnd
        {
            public string SubcategoryScndName { get; set; }
            public ObservableCollection<SubcategoryThird> SubcategoriesThird { get; set; }

            public SubcategoryScnd()
            {
                SubcategoriesThird = new ObservableCollection<SubcategoryThird>();
            }
        }

        public class SubcategoryThird
        {
            public string SubcategoryThirdName { get; set; }
        }

        


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

           

        }
    }
}     
                    
                
            
        
    

