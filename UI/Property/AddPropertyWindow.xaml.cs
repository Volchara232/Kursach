using System;
using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Collections;
using Sem3_kurs.Model;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sem3_kurs
{
    public partial class AddPropertyWindow : Window
    {
        private EstateAgency _agency;
        public Property NewProperty { get; private set; }

        public AddPropertyWindow(EstateAgency agency)
        {
            InitializeComponent();
            _agency = agency;
            NewProperty = new Property();

            // Заполняем выпадающий список владельцев
            foreach (var owner in _agency.OwnersList.Owners)
            {
                CmbOwner.Items.Add(owner);
            }

            if (CmbOwner.Items.Count > 0)
            {
                CmbOwner.SelectedIndex = 0;
            }

            // Устанавливаем значения по умолчанию
            CmbDealType.SelectedIndex = 0;
            TxtFloor.Text = "1";
            TxtRooms.Text = "1";
            TxtTotalArea.Text = "30";
            TxtPrice.Text = "1000000";

            // Подписываемся на события изменения текста
            TxtTotalArea.TextChanged += TxtTotalArea_TextChanged_Handler;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем данные из формы
                NewProperty.Type = TxtType.Text.Trim();
                NewProperty.Address = TxtAddress.Text.Trim();
                NewProperty.District = TxtDistrict.Text.Trim();

                if (CmbDealType.SelectedItem is Enums.DealType dealType)
                    NewProperty.DealType = dealType;

                NewProperty.Owner = CmbOwner.SelectedItem as Owner;

                // Парсим числовые значения
                if (int.TryParse(TxtRooms.Text, out int rooms))
                    NewProperty.Rooms = rooms;
                else
                {
                    MessageBox.Show("Введите корректное количество комнат", "Ошибка");
                    TxtRooms.Focus();
                    return;
                }

                if (double.TryParse(TxtTotalArea.Text, out double totalArea))
                    NewProperty.TotalArea = totalArea;
                else
                {
                    MessageBox.Show("Введите корректную общую площадь", "Ошибка");
                    TxtTotalArea.Focus();
                    return;
                }

                if (double.TryParse(TxtLivingArea.Text, out double livingArea))
                    NewProperty.LivingArea = livingArea;
                else
                    NewProperty.LivingArea = totalArea * 0.7; // По умолчанию 70% от общей

                if (double.TryParse(TxtKitchenArea.Text, out double kitchenArea))
                    NewProperty.KitchenArea = kitchenArea;
                else
                    NewProperty.KitchenArea = totalArea * 0.1; // По умолчанию 10% от общей

                if (int.TryParse(TxtFloor.Text, out int floor))
                    NewProperty.Floor = floor;
                else
                {
                    MessageBox.Show("Введите корректный этаж", "Ошибка");
                    TxtFloor.Focus();
                    return;
                }

                if (decimal.TryParse(TxtPrice.Text, out decimal price))
                    NewProperty.Price = price;
                else
                {
                    MessageBox.Show("Введите корректную цену", "Ошибка");
                    TxtPrice.Focus();
                    return;
                }

                NewProperty.HasBalcony = ChkHasBalcony.IsChecked == true;
                NewProperty.HasLoggia = ChkHasLoggia.IsChecked == true;
                NewProperty.HouseDescription = TxtDescription.Text.Trim();

                // Генерируем ID
                if (_agency.PropertiesList.Count > 0)
                {
                    NewProperty.Id = _agency.PropertiesList.Max(p => p.Id) + 1;
                }
                else
                {
                    NewProperty.Id = 1;
                }

                NewProperty.IsAvailable = true;

                // Проверяем валидность
                if (!NewProperty.IsValid)
                {
                    string errorMessage = GetValidationErrors(NewProperty);
                    MessageBox.Show($"Ошибки в данных:\n{errorMessage}", "Ошибка валидации",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Дополнительные логические проверки
                List<string> logicalErrors = new List<string>();

                if (NewProperty.LivingArea > NewProperty.TotalArea)
                {
                    logicalErrors.Add("• Жилая площадь не может быть больше общей площади");
                }

                if (NewProperty.KitchenArea > NewProperty.TotalArea)
                {
                    logicalErrors.Add("• Площадь кухни не может быть больше общей площади");
                }

                if (NewProperty.LivingArea + NewProperty.KitchenArea > NewProperty.TotalArea)
                {
                    logicalErrors.Add("• Сумма жилой и кухонной площади не может быть больше общей площади");
                }

                if (logicalErrors.Any())
                {
                    MessageBox.Show($"Логические ошибки в данных:\n{string.Join("\n", logicalErrors)}",
                                  "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Проверяем, выбран ли владелец
                if (NewProperty.Owner == null)
                {
                    MessageBox.Show("Выберите владельца", "Ошибка");
                    CmbOwner.Focus();
                    return;
                }

                // Добавляем объект к владельцу
                NewProperty.Owner.AddProperty(NewProperty);

                // Добавляем в список агентства
                _agency.PropertiesList.Add(NewProperty);

                MessageBox.Show("Объект недвижимости успешно добавлен", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении объекта: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Вспомогательный метод для получения ошибок валидации
        private string GetValidationErrors(object obj)
        {
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>(); // Явно указываем тип

            if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(obj, context, results, true))
            {
                return string.Join("\n", results.Select(r => $"• {r.ErrorMessage}"));
            }

            return "";
        }

        // Автоматический расчет жилой площади при изменении общей
        private void TxtTotalArea_TextChanged_Handler(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(TxtTotalArea.Text, out double totalArea))
            {
                // Автоматически заполняем жилую площадь, если она пустая
                if (string.IsNullOrWhiteSpace(TxtLivingArea.Text))
                {
                    TxtLivingArea.Text = (totalArea * 0.7).ToString("F1");
                }

                // Автоматически заполняем площадь кухни, если она пустая
                if (string.IsNullOrWhiteSpace(TxtKitchenArea.Text))
                {
                    TxtKitchenArea.Text = (totalArea * 0.1).ToString("F1");
                }
            }
        }
    }
}