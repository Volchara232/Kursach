using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sem3_kurs.Collections;
using Sem3_kurs.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Добавьте это

namespace Sem3_kurs
{
    public partial class OrdersPage : Page, IRefreshablePage
    {
        private EstateAgency _agency;
        private ObservableCollection<Order> _ordersCollection;
        public List<Client> AllClients { get; private set; }

        public OrdersPage(EstateAgency agency)
        {
            InitializeComponent();
            _agency = agency;
            AllClients = _agency.ClientsList.Clients.ToList();
            DataContext = this;
            RefreshData();
        }

        public void RefreshData()
        {
            _ordersCollection = new ObservableCollection<Order>(_agency.OrdersList);
            OrdersDataGrid.ItemsSource = _ordersCollection;
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            // Если нет клиентов - предложить сначала добавить клиента
            if (!AllClients.Any())
            {
                MessageBox.Show("Сначала добавьте хотя бы одного клиента", "Ошибка");
                return;
            }

            // Простой выбор клиента через ComboBox в MessageBox
            var selectWindow = new SelectClientWindow(_agency);
            if (selectWindow.ShowDialog() == true && selectWindow.SelectedClient != null)
            {
                var selectedClient = selectWindow.SelectedClient;

                var newOrder = new Order
                {
                    Id = _agency.OrdersList.Count > 0 ? _agency.OrdersList.Max(o => o.Id) + 1 : 1,
                    Client = selectedClient,
                    RequestType = Enums.RequestType.Buy,
                    Status = Enums.OrderStatus.New,
                    LastUpdateDate = DateTime.Now
                    // CreatedDate устанавливается автоматически в конструкторе
                };

                // Добавляем заявку клиенту
                selectedClient.AddOrder(newOrder);

                // Добавляем в коллекции
                _agency.OrdersList.Add(newOrder);
                _ordersCollection.Add(newOrder);

                MessageBox.Show($"Заявка добавлена для клиента {selectedClient.FullName}", "Успех");
            }
        }
        private void BtnCreateDeal_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selectedOrder)
            {
                // Проверяем, есть ли подобранные объекты
                if (!selectedOrder.MatchedProperties.Any())
                {
                    MessageBox.Show("Сначала подберите объекты для этой заявки", "Информация");
                    return;
                }

                // Показываем список подобранных объектов
                var selectPropertyWindow = new SelectPropertyWindow(selectedOrder.MatchedProperties);
                if (selectPropertyWindow.ShowDialog() == true && selectPropertyWindow.SelectedProperty != null)
                {
                    var property = selectPropertyWindow.SelectedProperty;

                    // Создаем окно оформления сделки
                    var dealWindow = new CreateDealWindow(selectedOrder, property, _agency);
                    if (dealWindow.ShowDialog() == true)
                    {
                        MessageBox.Show("Сделка успешно оформлена!", "Успех");
                        RefreshData();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заявку", "Предупреждение");
            }
        }
        private void BtnMatchProperties_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selectedOrder)
            {
                // Простой подбор объектов
                var availableProperties = _agency.PropertiesList
                    .Where(p => p.IsAvailable && p.MatchesOrder(selectedOrder))
                    .ToList();

                if (availableProperties.Any())
                {
                    // Добавляем найденные объекты к заявке
                    selectedOrder.SetMatchedProperties(availableProperties);

                    string propertiesList = string.Join("\n",
                        availableProperties.Select(p => $"• {p.Address} - {p.Price:N0} руб. ({p.DealType})"));

                    MessageBox.Show($"Подобрано {availableProperties.Count} объектов:\n\n{propertiesList}",
                                  $"Объекты для заявки #{selectedOrder.Id}");
                }
                else
                {
                    MessageBox.Show("Подходящих объектов не найдено", "Информация");
                }
            }
            else
            {
                MessageBox.Show("Выберите заявку", "Предупреждение");
            }
        }

        // Обработка клавиши Delete для удаления заявки
        private void OrdersDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && OrdersDataGrid.SelectedItem is Order order)
            {
                e.Handled = true; // Отменяем стандартное поведение Delete

                var result = MessageBox.Show($"Удалить заявку #{order.Id}?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Удаляем из списка клиента
                    order.Client?.Orders.Remove(order);

                    // Удаляем из основной коллекции
                    _agency.OrdersList.Remove(order);

                    // Удаляем из коллекции для отображения
                    _ordersCollection.Remove(order);

                    MessageBox.Show("Заявка удалена", "Успех");
                }
            }
        }

        // Автоматическая валидация при изменении
        private void OrdersDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.DataContext is Order order)
            {
                // Проверяем валидность после редактирования
                if (!order.IsValid)
                {
                    string errorMessage = GetValidationErrors(order);
                    MessageBox.Show($"Ошибки в данных:\n{errorMessage}", "Ошибка валидации");
                }
                else
                {
                    // Обновляем дату изменения
                    order.LastUpdateDate = DateTime.Now;

                    // Автоматически устанавливаем DealType в зависимости от RequestType
                    if (order.RequestType == Enums.RequestType.Buy || order.RequestType == Enums.RequestType.Sell)
                    {
                        order.RequestDealType = Enums.DealType.Sale;
                    }
                    else if (order.RequestType == Enums.RequestType.Rent || order.RequestType == Enums.RequestType.LeaseOut)
                    {
                        order.RequestDealType = Enums.DealType.Rent;
                    }
                }
            }
        }

        // Вспомогательный метод для получения ошибок валидации
        private string GetValidationErrors(object obj)
        {
            var context = new ValidationContext(obj);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>(); // Явно указываем тип

            if (!Validator.TryValidateObject(obj, context, results, true))
            {
                return string.Join("\n", results.Select(r => r.ErrorMessage));
            }

            return "";
        }
    }
}