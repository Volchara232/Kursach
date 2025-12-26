using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sem3_kurs.Collections;
using Sem3_kurs.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sem3_kurs
{
    public partial class ClientsPage : Page, IRefreshablePage
    {
        private EstateAgency _agency;
        private ObservableCollection<Client> _clientsCollection;

        public ClientsPage(EstateAgency agency)
        {
            InitializeComponent();
            _agency = agency;
            RefreshData();
        }

        public void RefreshData()
        {
            _clientsCollection = new ObservableCollection<Client>(_agency.ClientsList.Clients);
            ClientsDataGrid.ItemsSource = _clientsCollection;
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddClientWindow();
            if (addWindow.ShowDialog() == true)
            {
                _agency.ClientsList.AddClient(addWindow.NewClient);
                _clientsCollection.Add(addWindow.NewClient);
                MessageBox.Show("Клиент успешно добавлен", "Успех");
            }
        }

        // Обработка клавиши Delete для удаления клиента
        private void ClientsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && ClientsDataGrid.SelectedItem is Client client)
            {
                e.Handled = true; // Отменяем стандартное поведение Delete

                var result = MessageBox.Show($"Удалить клиента '{client.FullName}'?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Удаляем из основной коллекции
                    _agency.ClientsList.Clients.Remove(client);

                    // Удаляем из коллекции для отображения
                    _clientsCollection.Remove(client);

                    MessageBox.Show("Клиент удален", "Успех");
                }
            }
        }

        // Автоматическая валидация при изменении
        private void ClientsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.DataContext is Client client)
            {
                // Проверяем валидность после редактирования
                if (!client.IsValid)
                {
                    MessageBox.Show("Исправьте ошибки в данных", "Ошибка валидации");
                }
            }
        }
    }
}