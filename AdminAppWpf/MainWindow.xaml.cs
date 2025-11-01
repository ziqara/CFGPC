using System;
using System.Collections.ObjectModel;
using System.Windows;
using DDMLib;

namespace AdminAppWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SupplierService supplierService_;
        private readonly ObservableCollection<Supplier> suppliers_ = new ObservableCollection<Supplier>();

        public MainWindow()
        {
            InitializeComponent();
            supplierService_ = new SupplierService(new MySqlSupplierRepository());
            SuppliersDataGrid.ItemsSource = suppliers_;
        }

        private void ShowSuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                suppliers_.Clear();
                var suppliers = supplierService_.GetAllSuppliers();

                foreach (var supplier in suppliers)
                {
                    suppliers_.Add(supplier);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка поставщиков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
