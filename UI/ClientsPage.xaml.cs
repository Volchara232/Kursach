using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Collections;
using System.Linq;
using Sem3_kurs.Model;

namespace Sem3_kurs
{
    public partial class ClientsPage : Page, IRefreshablePage
    {
        private EstateAgency _agency;

        public ClientsPage(EstateAgency agency)
        {
            InitializeComponent();
            _agency = agency;
            RefreshData();
        }

        public void RefreshData()
        {
            ClientsListView.Items.Clear();
            foreach (var client in _agency.ClientsList.Clients)
            {
                ClientsListView.Items.Add(client);
            }
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddClientWindow();
            if (addWindow.ShowDialog() == true)
            {
                _agency.ClientsList.AddClient(addWindow.NewClient);
                RefreshData();
                MessageBox.Show("Клиент успешно добавлен", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var searchWindow = new SearchWindow("Введите фамилию для поиска:");
            if (searchWindow.ShowDialog() == true)
            {
                var lastName = searchWindow.SearchText;
                var clients = _agency.ClientsList.FindByLastName(lastName);

                ClientsListView.Items.Clear();
                foreach (var client in clients)
                {
                    ClientsListView.Items.Add(client);
                }

                if (!clients.Any())
                {
                    MessageBox.Show($"Клиенты с фамилией '{lastName}' не найдены", "Результат поиска");
                }
            }
        }

        private void BtnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsListView.SelectedItem is Client selectedClient)
            {
                var detailsWindow = new ClientDetailsWindow(selectedClient);
                detailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите клиента из списка", "Предупреждение",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClientsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ClientsListView.SelectedItem is Client selectedClient)
            {
                var detailsWindow = new ClientDetailsWindow(selectedClient);
                detailsWindow.ShowDialog();
            }
        }
    }
}