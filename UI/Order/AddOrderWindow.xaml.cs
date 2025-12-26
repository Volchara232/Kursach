using System;
using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Model;
using Sem3_kurs.Collections;
using System.Linq;

namespace Sem3_kurs
{
    public partial class AddOrderWindow : Window
    {
        private Client _client;
        private EstateAgency _agency;
        public Order NewOrder { get; private set; }

        public AddOrderWindow(Client client, EstateAgency agency)
        {
            InitializeComponent();

            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (agency == null)
                throw new ArgumentNullException(nameof(agency));

            _client = client;
            _agency = agency;

            // Инициализируем новую заявку
            NewOrder = new Order
            {
                Client = _client,
                Status = Enums.OrderStatus.New,
                RequestType = Enums.RequestType.Buy,
                RequestDealType = Enums.DealType.Sale
            };

            // Показываем имя клиента
            TxtClientName.Text = $"Клиент: {_client.FullName}";

            DataContext = NewOrder;

            // Подписываемся на событие изменения типа заявки
            CmbRequestType.SelectionChanged += CmbRequestType_SelectionChanged;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (NewOrder.RequestType == 0)
                {
                    MessageBox.Show("Выберите тип заявки", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    CmbRequestType.Focus();
                    return;
                }

                // Устанавливаем DealType в зависимости от RequestType
                if (NewOrder.RequestType == Enums.RequestType.Buy || NewOrder.RequestType == Enums.RequestType.Sell)
                {
                    NewOrder.RequestDealType = Enums.DealType.Sale;
                }
                else if (NewOrder.RequestType == Enums.RequestType.Rent || NewOrder.RequestType == Enums.RequestType.LeaseOut)
                {
                    NewOrder.RequestDealType = Enums.DealType.Rent;
                }

                // Генерируем ID
                if (_agency.OrdersList.Count > 0)
                {
                    NewOrder.Id = _agency.OrdersList.Max(o => o.Id) + 1;
                }
                else
                {
                    NewOrder.Id = 1;
                }

                // Парсим числовые значения
                ParseNumericValues();

                // Добавляем заявку клиенту и в список агентства
                _client.AddOrder(NewOrder);
                _agency.OrdersList.Add(NewOrder);

                MessageBox.Show("Заявка успешно создана", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании заявки: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ParseNumericValues()
        {
            // Парсим комнаты
            if (int.TryParse(TxtRooms.Text, out int rooms))
            {
                NewOrder.Rooms = rooms;
            }

            // Парсим площадь
            if (double.TryParse(TxtAreaMin.Text, out double areaMin))
            {
                NewOrder.TotalAreaMin = areaMin;
            }
            if (double.TryParse(TxtAreaMax.Text, out double areaMax))
            {
                NewOrder.TotalAreaMax = areaMax;
            }

            // Парсим цены
            if (decimal.TryParse(TxtPriceMin.Text, out decimal priceMin))
            {
                NewOrder.PriceMin = priceMin;
            }
            if (decimal.TryParse(TxtPriceMax.Text, out decimal priceMax))
            {
                NewOrder.PriceMax = priceMax;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Обработчики изменения типа заявки
        private void CmbRequestType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbRequestType.SelectedItem is Enums.RequestType requestType)
            {
                // Автоматически устанавливаем DealType
                if (requestType == Enums.RequestType.Buy || requestType == Enums.RequestType.Sell)
                {
                    NewOrder.RequestDealType = Enums.DealType.Sale;
                    CmbDealType.SelectedItem = Enums.DealType.Sale;
                }
                else if (requestType == Enums.RequestType.Rent || requestType == Enums.RequestType.LeaseOut)
                {
                    NewOrder.RequestDealType = Enums.DealType.Rent;
                    CmbDealType.SelectedItem = Enums.DealType.Rent;
                }
            }
        }
    }
}