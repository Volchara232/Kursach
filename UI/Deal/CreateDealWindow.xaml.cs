using System;
using System.Windows;
using Sem3_kurs.Model;
using Sem3_kurs.Collections;

namespace Sem3_kurs
{
    public partial class CreateDealWindow : Window
    {
        private EstateAgency _agency;
        private Order _order;
        private Property _property;
        private decimal _amount;

        public Deal CreatedDeal { get; private set; }

        public CreateDealWindow(Order order, Property property, EstateAgency agency)
        {
            InitializeComponent();

            if (order == null || property == null || agency == null)
            {
                MessageBox.Show("Недостаточно данных для оформления сделки", "Ошибка");
                Close();
                return;
            }

            _order = order;
            _property = property;
            _agency = agency;

            // Устанавливаем сумму по умолчанию (цена объекта)
            _amount = property.Price;
            TxtAmount.Text = _amount.ToString("N0");

            // Рассчитываем и показываем гонорар
            CalculateCommission();

            // Заполняем информацию
            TxtClientInfo.Text = $"Клиент: {order.Client?.FullName ?? "Не указан"}";
            TxtOrderInfo.Text = $"Тип: {order.RequestType} | Район: {order.District} | Комнат: {order.Rooms}";
            TxtPropertyInfo.Text = $"Адрес: {property.Address} | Тип: {property.Type} | Цена: {property.Price:N0} руб.";

            DpDealDate.SelectedDate = DateTime.Now;
        }

        private void TxtAmount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (decimal.TryParse(TxtAmount.Text.Replace(" ", ""), out decimal amount))
            {
                _amount = amount;
                CalculateCommission();
            }
        }

        private void CalculateCommission()
        {
            decimal commission = _amount * 0.02m; // 2% гонорар
            TxtCommission.Text = $"{commission:N0} руб.";
        }

        private void BtnCreateDeal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация суммы
                if (_amount <= 0)
                {
                    MessageBox.Show("Введите корректную сумму сделки", "Ошибка");
                    TxtAmount.Focus();
                    return;
                }

                // Создаем сделку
                CreatedDeal = new Deal
                {
                    Id = _agency.DealsList.Count > 0 ? _agency.DealsList.Max(d => d.Id) + 1 : 1,
                    Order = _order,
                    Property = _property,
                    DealType = _property.DealType,
                    Amount = _amount,
                    Date = DpDealDate.SelectedDate ?? DateTime.Now,
                    IsCompleted = ChkIsCompleted.IsChecked == true
                };

                // Рассчитываем гонорар
                CreatedDeal.CalculateAgencyFee();

                // Регистрируем сделку в агентстве
                _agency.RegisterDeal(_order, _property, _amount);

                // Формируем сообщение в зависимости от типа сделки
                string message;
                if (_property.DealType == Enums.DealType.Sale)
                {
                    message = $"Сделка купли-продажи успешно оформлена!\n" +
                             $"Новый владелец: {_order.Client.FullName}\n" +
                             $"Сумма: {CreatedDeal.Amount:N0} руб.\n" +
                             $"Гонорар: {CreatedDeal.AgencyFee:N0} руб.";
                }
                else
                {
                    message = $"Договор аренды успешно оформлен!\n" +
                             $"Арендатор: {_order.Client.FullName}\n" +
                             $"Сумма: {CreatedDeal.Amount:N0} руб.\n" +
                             $"Гонорар: {CreatedDeal.AgencyFee:N0} руб.";
                }

                MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении сделки: {ex.Message}", "Ошибка");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}