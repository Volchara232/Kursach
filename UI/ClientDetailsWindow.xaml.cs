using System.Windows;
using Sem3_kurs.Model;
using System.Linq;

namespace Sem3_kurs
{
    public partial class ClientDetailsWindow : Window
    {
        private Client _client;

        public ClientDetailsWindow(Client client)
        {
            InitializeComponent();
            _client = client;
            LoadClientData();
        }

        private void LoadClientData()
        {
            // Заполняем поля
            TxtClientName.Text = _client.FullName;
            TxtPhone.Text = _client.Phone;
            TxtAddress.Text = _client.Address;
            TxtEmail.Text = _client.Email;
            TxtRegNumber.Text = _client.RegistrationNumber;
            TxtTotalOrders.Text = _client.Orders.Count.ToString();

            // Заполняем списки заявок
            OrdersListView.Items.Clear();
            foreach (var order in _client.Orders)
            {
                OrdersListView.Items.Add(order);
            }

            ActiveOrdersListView.Items.Clear();
            foreach (var order in _client.GetActiveOrders())
            {
                ActiveOrdersListView.Items.Add(order);
            }
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления заявки будет реализована позже", "Информация");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}