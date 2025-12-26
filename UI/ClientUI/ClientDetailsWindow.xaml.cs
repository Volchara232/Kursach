using System.Windows;
using Sem3_kurs.Model;

namespace Sem3_kurs
{
    public partial class ClientOrdersWindow : Window
    {
        private Client _client;

        public ClientOrdersWindow(Client client)
        {
            InitializeComponent();
            _client = client;

            TxtClientName.Text = client.FullName;
            OrdersDataGrid.ItemsSource = client.Orders;
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            // Простое добавление заявки
            var order = new Order
            {
                Id = _client.Orders.Count + 1,
                Client = _client,
                RequestType = Enums.RequestType.Buy,
                Status = Enums.OrderStatus.New
            };

            _client.AddOrder(order);
            OrdersDataGrid.Items.Refresh();
            MessageBox.Show("Заявка добавлена", "Успех");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}