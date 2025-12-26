using System.Windows;
using Sem3_kurs.Collections;
using Sem3_kurs.Model;

namespace Sem3_kurs
{
    public partial class SelectClientWindow : Window
    {
        public Client SelectedClient { get; private set; }

        public SelectClientWindow(EstateAgency agency)
        {
            InitializeComponent();

            // Заполняем список клиентов
            foreach (var client in agency.ClientsList.Clients)
            {
                ClientsListView.Items.Add(client);
            }

            // Выбираем первого клиента, если есть
            if (ClientsListView.Items.Count > 0)
            {
                ClientsListView.SelectedIndex = 0;
            }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectedClient = ClientsListView.SelectedItem as Client;
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}