using System;
using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Collections;
using System.IO;
using System.Windows.Media;

namespace Sem3_kurs
{
    public partial class MainWindow : Window
    {
        public EstateAgency Agency { get; private set; }
        private DataStorage _dataStorage;
        private string _filePath;

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Риэлторское агентство";

            // Инициализация хранилища
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RealEstateAgency");

            Directory.CreateDirectory(appDataPath);
            _filePath = Path.Combine(appDataPath, "agency_data.json");
            _dataStorage = new DataStorage(_filePath);

            // Загружаем или создаем данные
            LoadOrCreateData();

            // Показываем стартовую страницу
            ShowStartPage();
        }

        private void LoadOrCreateData()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    Agency = _dataStorage.Load();
                }
                else
                {
                    Agency = new EstateAgency();
                    CreateTestData();
                    _dataStorage.Save(Agency);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}\nСоздаю новую базу данных...",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Agency = new EstateAgency();
                CreateTestData();
            }
        }

        private void CreateTestData()
        {
            // Тестовый клиент 1
            var client1 = new Model.Client
            {
                FullName = "Иванов Иван Иванович",
                Address = "ул. Ленина, 10",
                Phone = "+7 (999) 123-45-67",
                Email = "ivanov@mail.ru",
                RegistrationNumber = "001/2024"
            };
            Agency.ClientsList.AddClient(client1);

            // Тестовый клиент 2
            var client2 = new Model.Client
            {
                FullName = "Сидорова Мария Петровна",
                Address = "ул. Пушкина, 5",
                Phone = "+7 (999) 555-55-55",
                Email = "sidorova@mail.ru",
                RegistrationNumber = "002/2024"
            };
            Agency.ClientsList.AddClient(client2);

            // Тестовый владелец
            var owner1 = new Model.Owner
            {
                FullName = "Петров Петр Петрович",
                Address = "ул. Мира, 15",
                Phone = "+7 (999) 987-65-43",
                Email = "petrov@mail.ru"
            };
            Agency.OwnersList.AddOwner(owner1);

            // Тестовый объект 1
            var property1 = new Model.Property
            {
                Id = 1,
                Type = "Квартира",
                District = "Центральный",
                Address = "ул. Советская, 25, кв. 12",
                Floor = 3,
                Rooms = 2,
                TotalArea = 55.5,
                LivingArea = 35.0,
                KitchenArea = 10.0,
                HasBalcony = true,
                Price = 5000000,
                DealType = Enums.DealType.Sale,
                IsAvailable = true
            };
            property1.Owner = owner1;
            owner1.Properties.Add(property1);
            Agency.PropertiesList.Add(property1);

            // Тестовая заявка 1
            var order1 = new Model.Order
            {
                Id = 1,
                Client = client1,
                RequestType = Enums.RequestType.Buy,
                RequestDealType = Enums.DealType.Sale,
                District = "Центральный",
                Rooms = 2,
                PriceMax = 6000000,
                Status = Enums.OrderStatus.New
            };
            client1.AddOrder(order1);
            Agency.OrdersList.Add(order1);

            // Тестовая заявка 2
            var order2 = new Model.Order
            {
                Id = 2,
                Client = client2,
                RequestType = Enums.RequestType.Rent,
                RequestDealType = Enums.DealType.Rent,
                District = "Северный",
                Rooms = 1,
                PriceMax = 25000,
                Status = Enums.OrderStatus.InProgress
            };
            client2.AddOrder(order2);
            Agency.OrdersList.Add(order2);
        }

        private void ShowStartPage()
        {
            var startPage = new StartPage();
            MainFrame.Navigate(startPage);
        }

        // Изменяем на public методы
        public void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            var clientsPage = new ClientsPage(Agency);
            MainFrame.Navigate(clientsPage);
        }

        public void BtnOwners_Click(object sender, RoutedEventArgs e)
        {
            var ownersPage = new OwnersPage(Agency);
            MainFrame.Navigate(ownersPage);
        }

        public void BtnProperties_Click(object sender, RoutedEventArgs e)
        {
            var propertiesPage = new PropertiesPage(Agency);
            MainFrame.Navigate(propertiesPage);
        }

        public void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            var ordersPage = new OrdersPage(Agency);
            MainFrame.Navigate(ordersPage);
        }

        public void BtnStatistics_Click(object sender, RoutedEventArgs e)
        {
            var statsPage = new StatisticsPage(Agency.GetStatisticsManager());
            MainFrame.Navigate(statsPage);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dataStorage.Save(Agency);
                MessageBox.Show($"Данные успешно сохранены!\n" +
                              $"Клиентов: {Agency.ClientsList.Clients.Count}\n" +
                              $"Путь: {_filePath}",
                              "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Agency = _dataStorage.Load();
                MessageBox.Show($"Данные успешно загружены!\n" +
                              $"Клиентов: {Agency.ClientsList.Clients.Count}",
                              "Загрузка", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем текущую страницу
                if (MainFrame.Content is IRefreshablePage refreshablePage)
                {
                    refreshablePage.RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке:\n{ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
      

        // Метод для сброса цветов кнопок (если есть)
        private void ResetButtonColors()
        {
            BtnClients.Background = Brushes.Transparent;
            BtnOwners.Background = Brushes.Transparent;
            BtnProperties.Background = Brushes.Transparent;
 
            BtnOrders.Background = Brushes.Transparent;
            BtnStatistics.Background = Brushes.Transparent;
        }
        // Добавим публичные методы для навигации из других страниц
        public void NavigateToClients()
        {
            BtnClients_Click(null, null);
        }

        public void NavigateToProperties()
        {
            BtnProperties_Click(null, null);
        }

        public void NavigateToStatistics()
        {
            BtnStatistics_Click(null, null);
        }
    }
}