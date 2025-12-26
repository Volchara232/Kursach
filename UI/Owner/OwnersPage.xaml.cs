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
    public partial class OwnersPage : Page, IRefreshablePage
    {
        private EstateAgency _agency;
        private ObservableCollection<Owner> _ownersCollection;

        public OwnersPage(EstateAgency agency)
        {
            InitializeComponent();
            _agency = agency;
            RefreshData();
        }

        public void RefreshData()
        {
            _ownersCollection = new ObservableCollection<Owner>(_agency.OwnersList.Owners);
            OwnersDataGrid.ItemsSource = _ownersCollection;
        }

        private void BtnAddOwner_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddOwnerWindow();
            if (addWindow.ShowDialog() == true)
            {
                _agency.OwnersList.AddOwner(addWindow.NewOwner);
                _ownersCollection.Add(addWindow.NewOwner);
                MessageBox.Show("Владелец успешно добавлен", "Успех");
            }
        }

        // Обработка клавиши Delete для удаления владельца
        private void OwnersDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && OwnersDataGrid.SelectedItem is Owner owner)
            {
                e.Handled = true; // Отменяем стандартное поведение Delete

                // Проверяем, есть ли у владельца объекты
                if (owner.Properties.Any())
                {
                    MessageBox.Show($"Нельзя удалить владельца {owner.FullName}, так как у него есть объекты недвижимости.\nСначала удалите или передайте его объекты другим владельцам.",
                                  "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Удалить владельца '{owner.FullName}'?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Удаляем из основной коллекции
                    _agency.OwnersList.Owners.Remove(owner);

                    // Удаляем из коллекции для отображения
                    _ownersCollection.Remove(owner);

                    MessageBox.Show("Владелец удален", "Успех");
                }
            }
        }

        // Автоматическая валидация при изменении
        private void OwnersDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.DataContext is Owner owner)
            {
                // Проверяем валидность после редактирования
                if (!owner.IsValid)
                {
                    string errorMessage = GetValidationErrors(owner);
                    MessageBox.Show($"Ошибки в данных:\n{errorMessage}", "Ошибка валидации");
                }
                else
                {
                    // Форматируем телефон после успешного редактирования
                    owner.Phone = Helpers.ValidatorHelper.FormatPhone(owner.Phone);
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