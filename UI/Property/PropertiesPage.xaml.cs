using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sem3_kurs.Collections;
using Sem3_kurs.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sem3_kurs
{
    public partial class PropertiesPage : Page, IRefreshablePage
    {
        private EstateAgency _agency;
        private ObservableCollection<Property> _propertiesCollection;

        public PropertiesPage(EstateAgency agency)
        {
            InitializeComponent();
            _agency = agency;
            RefreshData();
        }

        public void RefreshData()
        {
            _propertiesCollection = new ObservableCollection<Property>(_agency.PropertiesList);
            PropertiesDataGrid.ItemsSource = _propertiesCollection;
        }

        private void BtnAddProperty_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, есть ли владельцы
            if (!_agency.OwnersList.Owners.Any())
            {
                MessageBox.Show("Сначала добавьте хотя бы одного владельца недвижимости",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addWindow = new AddPropertyWindow(_agency);
            if (addWindow.ShowDialog() == true)
            {
                RefreshData();
                MessageBox.Show("Объект недвижимости успешно добавлен", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Обработка клавиши Delete для удаления объекта
        private void PropertiesDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && PropertiesDataGrid.SelectedItem is Property property)
            {
                e.Handled = true; // Отменяем стандартное поведение Delete

                // Проверяем, используется ли объект в сделках
                bool isUsedInDeals = _agency.DealsList.Any(d => d.Property?.Id == property.Id);
                if (isUsedInDeals)
                {
                    MessageBox.Show($"Нельзя удалить объект '{property.Address}', так как он используется в сделках.",
                                  "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Удалить объект недвижимости?\n\n" +
                                           $"Адрес: {property.Address}\n" +
                                           $"Тип: {property.Type}\n" +
                                           $"Цена: {property.Price:N0} руб.",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Удаляем из основной коллекции
                    bool removedFromAgency = _agency.PropertiesList.Remove(property);

                    // Удаляем у владельца
                    bool removedFromOwner = false;
                    if (property.Owner != null)
                    {
                        removedFromOwner = property.Owner.Properties.Remove(property);
                    }

                    // Удаляем из коллекции для отображения
                    bool removedFromCollection = _propertiesCollection.Remove(property);

                    if (removedFromAgency)
                    {
                        MessageBox.Show("Объект недвижимости удален", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить объект", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Автоматическая валидация при изменении
        private void PropertiesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.DataContext is Property property)
            {
                // Даем время на обновление данных
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Проверяем валидность после редактирования
                    if (!property.IsValid)
                    {
                        string errorMessage = GetValidationErrors(property);

                        // Отменяем редактирование путем перезагрузки данных
                        RefreshData();

                        MessageBox.Show($"Ошибки в данных:\n{errorMessage}\n\nИзменения не сохранены.",
                                      "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        // Проверяем логические соотношения площадей
                        List<string> areaErrors = new List<string>();

                        if (property.LivingArea > property.TotalArea)
                        {
                            areaErrors.Add("• Жилая площадь не может быть больше общей");
                        }

                        if (property.KitchenArea > property.TotalArea)
                        {
                            areaErrors.Add("• Площадь кухни не может быть больше общей");
                        }

                        if (property.LivingArea + property.KitchenArea > property.TotalArea)
                        {
                            areaErrors.Add("• Сумма жилой и кухонной площади не может быть больше общей");
                        }

                        if (areaErrors.Any())
                        {
                            // Отменяем редактирование
                            RefreshData();

                            MessageBox.Show($"Логические ошибки в данных:\n{string.Join("\n", areaErrors)}\n\nИзменения не сохранены.",
                                          "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Изменения сохранены", "Успех",
                                          MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }));
            }
        }
        
        // Вспомогательный метод для получения ошибок валидации
        private string GetValidationErrors(object obj)
        {
            var context = new ValidationContext(obj);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!Validator.TryValidateObject(obj, context, results, true))
            {
                return string.Join("\n", results.Select(r => $"• {r.ErrorMessage}"));
            }

            return "";
        }

        // Двойной клик по строке - быстрый просмотр
        private void PropertiesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PropertiesDataGrid.SelectedItem is Property property)
            {
                string propertyInfo = $"Объект недвижимости #{property.Id}\n\n" +
                                    $"Тип: {property.Type}\n" +
                                    $"Адрес: {property.Address}\n" +
                                    $"Район: {property.District}\n" +
                                    $"Комнат: {property.Rooms}\n" +
                                    $"Этаж: {property.Floor}\n" +
                                    $"Общая площадь: {property.TotalArea} м²\n" +
                                    $"Жилая площадь: {property.LivingArea} м²\n" +
                                    $"Площадь кухни: {property.KitchenArea} м²\n" +
                                    $"Балкон: {(property.HasBalcony ? "Да" : "Нет")}\n" +
                                    $"Лоджия: {(property.HasLoggia ? "Да" : "Нет")}\n" +
                                    $"Цена: {property.Price:N0} руб.\n" +
                                    $"Тип сделки: {property.DealType}\n" +
                                    $"Владелец: {property.Owner?.FullName ?? "Не указан"}\n" +
                                    $"Доступен: {(property.IsAvailable ? "Да" : "Нет")}\n" +
                                    $"Описание: {property.HouseDescription}";

                MessageBox.Show(propertyInfo, "Информация об объекте",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Контекстное меню: быстрые действия
        private void ContextMenuToggleAvailability_Click(object sender, RoutedEventArgs e)
        {
            if (PropertiesDataGrid.SelectedItem is Property property)
            {
                property.IsAvailable = !property.IsAvailable;
                string status = property.IsAvailable ? "доступен" : "недоступен";
                MessageBox.Show($"Объект '{property.Address}' теперь {status}", "Статус изменен");
                RefreshData();
            }
        }

        private void ContextMenuViewDetails_Click(object sender, RoutedEventArgs e)
        {
            PropertiesDataGrid_MouseDoubleClick(sender, null);
        }
    }
}