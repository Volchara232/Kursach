using System.Windows;
using System.Windows.Controls;
using Sem3_kurs.Collections;
using System.Linq;

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
                ClientsListView.Items.Add(new
                {
                    client.FullName,
                    client.Phone,
                    client.Email,
                    ActiveOrdersCount = client.GetActiveOrders().Count()
                });
            }
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления клиента будет реализована позже", "Информация");
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция поиска будет реализована позже", "Информация");
        }
    }
}