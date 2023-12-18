using Npgsql;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Analys_prostoev
{
    /// <summary>
    /// Логика взаимодействия для ChangeHistory.xaml
    /// </summary>
    public partial class ChangeHistory : Window
    {
        public int period;
        public string region;
        readonly long id;

        public ChangeHistory(int period, string region, long id)
        {
            InitializeComponent();
            this.id = id;
            this.period = period;
            this.region = region;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            namePros.Content = $"Простой: {id}  Период: {period}  Усчасток: {region}";
            HistorySearch();
        }

        private void HistorySearch()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(DBContext.connectionString))
            {
                connection.Open();

                using (NpgsqlCommand getHistory = new NpgsqlCommand(DBContext.getHistoryString, connection))
                {
                    getHistory.Parameters.AddWithValue("@id", id);
                    
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(getHistory);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    NpgsqlDataReader reader = getHistory.ExecuteReader();


                    HistoryTable.ItemsSource = dataTable.DefaultView;
                }
            }

            SetNewColumnNames();
        }

        private void SetNewColumnNames()
        {
            DataGridTextColumn date_change = (DataGridTextColumn)HistoryTable.Columns.FirstOrDefault(c => c.Header.ToString() == "date_change");
            if (date_change != null)
            {
                date_change.Header = "Дата изменения";
                date_change.Binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                date_change.Width = new DataGridLength(120);
            }
            DataGridTextColumn modified_element = (DataGridTextColumn)HistoryTable.Columns.FirstOrDefault(c => c.Header.ToString() == "modified_element");
            if (modified_element != null)
            {

                modified_element.Header = "Изменения";
            }
            foreach (DataGridColumn column in HistoryTable.Columns)
            {
                DataGridTextColumn textColumn = column as DataGridTextColumn;
                if (textColumn != null)
                {
                    textColumn.HeaderStyle = new Style(typeof(DataGridColumnHeader));
                    textColumn.HeaderStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
