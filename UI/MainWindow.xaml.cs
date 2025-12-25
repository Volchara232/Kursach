using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Collections;
using System.IO;

namespace Sem3_kurs
{
    public partial class MainWindow : Window
    {

        public EstateAgency Agency { get; private set; } = new EstateAgency();

        private DataStorage _dataStorage;

        public MainWindow()
        {
            InitializeComponent();
            _dataStorage = new DataStorage("agency_data.json");
            ShowStartPage();
        }

        private void ShowStartPage()
        {
            var startPage = new StartPage();
            MainFrame.Navigate(startPage);
        }


        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            var clientsPage = new ClientsPage(Agency);
            MainFrame.Navigate(clientsPage);
        }

        private void BtnOwners_Click(object sender, RoutedEventArgs e)
        {
            var ownersPage = new OwnersPage(Agency);
            MainFrame.Navigate(ownersPage);
        }

        private void BtnProperties_Click(object sender, RoutedEventArgs e)
        {
            var propertiesPage = new PropertiesPage(Agency);
            MainFrame.Navigate(propertiesPage);
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            var ordersPage = new OrdersPage(Agency);
            MainFrame.Navigate(ordersPage);
        }

        private void BtnStatistics_Click(object sender, RoutedEventArgs e)
        {
            var statsPage = new StatisticsPage(Agency.GetStatisticsManager());
            MainFrame.Navigate(statsPage);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dataStorage.Save(Agency);
                MessageBox.Show("Данные успешно сохранены!", "Сохранение", 
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Agency = _dataStorage.Load();
                MessageBox.Show("Данные успешно загружены!", "Загрузка", 
                              MessageBoxButton.OK, MessageBoxImage.Information);
                

                if (MainFrame.Content is IRefreshablePage refreshablePage)
                {
                    refreshablePage.RefreshData();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


    public interface IRefreshablePage
    {
        void RefreshData();
    }
}